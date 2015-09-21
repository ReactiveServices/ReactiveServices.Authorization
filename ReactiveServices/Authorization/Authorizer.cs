using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using NLog;
using ReactiveServices.Authorization.ConfigurationSections;
using ReactiveServices.Configuration;
using ReactiveServices.MessageBus;

namespace ReactiveServices.Authorization
{
    public enum AuthorizedMessageOperation
    {
        Publish,
        Subscribe,
        Request,
        Respond,
        Send,
        Receive
    }

    public static class Authorizer
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public const string AuthenticationTokenKey = "AuthenticationToken";
        public const string AuthorizationFileName = "Authorization.config";
        private static System.Configuration.Configuration _authorizationFile;
        private static System.Configuration.Configuration AuthorizationFile
        {
            get
            {
                if (_authorizationFile == null)
                {
                    if (!File.Exists(AuthorizationFileName))
                        throw new FileNotFoundException("File Authorization.config could not be found!");

                    var authorizationConfigFile = new ExeConfigurationFileMap
                    {
                        ExeConfigFilename = AuthorizationFileName
                    };
                    _authorizationFile = ConfigurationManager.OpenMappedExeConfiguration(authorizationConfigFile, ConfigurationUserLevel.None);
                }
                return _authorizationFile;
            }
        }

        internal static IEnumerable<AuthorizationServicesElement> AuthorizationServices { get; private set; }
        internal static IEnumerable<AuthorizationRulesElement> AuthorizationRules { get; private set; }

        public static string AuthorizationDomain { get; private set; }
        internal static bool AllowByDefault { get; private set; }
        internal static bool DenyByDefault { get; private set; }

        /// <summary>
        /// LoadConfiguration reference assemblies and dependency injection mapping from the configuration file
        /// </summary>
        public static void LoadConfiguration()
        {
            ResetConfiguration();
            LoadAuthorizationServices();
            LoadAuthorizationRules();
        }

        private static void ResetConfiguration()
        {
            _authorizationFile = null;
            AllowByDefault = false;
            DenyByDefault = true;
            AuthorizationDomain = null;
            AuthorizationRules = null;
            AuthorizationServices = null;
        }

        /// <summary>
        /// Inform if the reference assemblies and dependency injection mapping was already loaded from the configuraiton file
        /// </summary>
        public static bool Loaded
        {
            get
            {
                return AuthorizationDomain != null &&
                       AuthorizationRules != null &&
                       AuthorizationServices != null;
            }
        }

        private static void LoadAuthorizationServices()
        {
            var authorizationServicesSection = AuthorizationFile.GetSection("AuthorizationServices") as AuthorizationServicesSection;

            if (authorizationServicesSection == null)
                throw new ConfigurationErrorsException(String.Format("AuthorizationServices section not found in the {0} file!", AuthorizationFileName));

            AuthorizationServices = authorizationServicesSection.Items;

            ValidateAuthorizationServices();
        }

        private static void ValidateAuthorizationServices()
        {
            if (AuthorizationServices.Any(authorizationService => String.IsNullOrEmpty(authorizationService.ServiceName) ||
                                                                  String.IsNullOrEmpty(authorizationService.AuthorizationRequestSubscriptionId)))
            {
                throw new ConfigurationErrorsException("Authorization service's service name and authorization request queue must be informed!");
            }
        }

        private static void LoadAuthorizationRules()
        {
            var authorizationRulesSection = AuthorizationFile.GetSection("AuthorizationRules") as AuthorizationRulesSection;

            if (authorizationRulesSection == null)
                throw new ConfigurationErrorsException(String.Format("AuthorizationRules section not found in the {0} file!", AuthorizationFileName));

            AuthorizationDomain = authorizationRulesSection.AuthorizationDomain;
            AllowByDefault = authorizationRulesSection.AllowByDefault;
            DenyByDefault = authorizationRulesSection.DenyByDefault;

            AuthorizationRules = authorizationRulesSection.Items;

            ValidateAuthorizationRules();
        }

        private static void ValidateAuthorizationRules()
        {
            if (AuthorizationRules.Any(authorizationRule => !AuthorizationServices.Any(s => s.ServiceName == authorizationRule.AuthorizationServiceName)))
            {
                throw new ConfigurationErrorsException("Invalid authorization service!");
            }

            if (AuthorizationRules.Any(authorizationRule => String.IsNullOrEmpty(authorizationRule.OperationName) ||
                                                            String.IsNullOrEmpty(authorizationRule.OperationValue)))
            {
                throw new ConfigurationErrorsException("Authorization rule's operation name and operation value must be informed!");
            }


            if (AllowByDefault && DenyByDefault)
                throw new ConfigurationErrorsException("AllowByDefault and DenyByDefault cannot be both true");
        }

        private static readonly Type AuthorizationRequestType = typeof(AuthorizationRequest);
        private static readonly Type AuthorizationResponseType = typeof(AuthorizationResponse);

        public static void Verify(string operationName, string operationValue, string authenticationToken)
        {
            if (String.IsNullOrWhiteSpace(operationName)) throw new ArgumentNullException("operationName");
            if (String.IsNullOrWhiteSpace(operationValue)) throw new ArgumentNullException("operationValue");
            if (authenticationToken == null) throw new ArgumentNullException("authenticationToken");

            // Ensure the configuration is loaded and updated
            LoadConfiguration();

            var authorizationError = Authorize(operationName, operationValue, authenticationToken);

            if (authorizationError != AuthorizationError.None)
                throw new AuthorizationException(AuthorizationDomain, operationName, operationValue, authorizationError);
        }

        private static AuthorizationError Authorize(string operationName, string operationValue, string authenticationToken)
        {
            try
            {
                // Ensure will not block the request and response operations for authorization messages
                if (((operationName == AuthorizedMessageOperation.Request.ToString()) || (operationName == AuthorizedMessageOperation.Respond.ToString())) &&
                    ((operationValue == AuthorizationRequestType.FullName) || (operationValue == AuthorizationResponseType.FullName)))
                {
                    return AuthorizationError.None;
                }

                var authorizationRule = AuthorizationRules.FirstOrDefault(
                    ar => ar.OperationName == operationName && ar.OperationValue == operationValue
                    );

                // If found an authorization rule, authenticate againts it
                if (authorizationRule != null)
                {
                    using (var requestBus = DependencyResolver.Get<IRequestBus>())
                    {
                        var authorizationService = AuthorizationServices.First(
                            s => s.ServiceName == authorizationRule.AuthorizationServiceName
                            );

                        var authorizationRequestSubscriptionId = authorizationService.AuthorizationRequestSubscriptionId;

                        var authorizationRequest = new AuthorizationRequest(operationName, operationValue, authenticationToken);

                        var authorizationResponse = requestBus.Request<AuthorizationRequest, AuthorizationResponse>(
                            authorizationRequest, SubscriptionId.FromString(authorizationRequestSubscriptionId)
                            );

                        if (authorizationResponse == null)
                            return AuthorizationError.NoResponseFromAuthorizationServer;

                        return authorizationResponse.AuthorizationError;
                    }
                }

                // If did not find an authorization rule, authenticate against the default settings
                if (AllowByDefault)
                    return AuthorizationError.None;

                if (DenyByDefault)
                    return AuthorizationError.UnauthorizedToken;

                // If did not find an authorization rule and the default settings where not informed, return InvalidAuthenticationToken error
                return AuthorizationError.InvalidAuthenticationToken;
            } 
            catch (Exception e)
            {
                // If some exception run, return InvalidAuthorizationSettings error
                Log.Error(e, "Exception processing authorization request: {0}", e.Message);
                return AuthorizationError.InvalidAuthorizationSettings;
            }
        }
    }
}
