using System;

namespace ReactiveServices.Authorization
{
    public class AuthorizationException : Exception
    {
        public AuthorizationException(string message)
            : base(message)
        {
        }

        public AuthorizationException(string authorizationDomain, string operationName, string operationValue, AuthorizationError authorizationError)
            : this(String.Format(
                "Authorization failed for operation '{0}' with value '{1}' in the authorization domain '{2}', due to authorization error '{3}'!",
                operationName, operationValue, authorizationDomain, authorizationError
            ))
        {
            AuthorizationError = authorizationError;
        }

        public AuthorizationError AuthorizationError { get; private set; }
    }
}
