
using System.Runtime.Serialization;
using ReactiveServices.MessageBus;

namespace ReactiveServices.Authorization
{
    [DataContract]
    public class AuthorizationRequest : Request
    {
        public AuthorizationRequest(string operationName, string operationValue, string authenticationToken)
            : base(RequesterId.FromString("Authorization"), RequestId.New())
        {
            OperationName = operationName;
            OperationValue = operationValue;
            AuthenticationToken = authenticationToken;
        }

        [DataMember]
        public string OperationName { get; private set; }
        [DataMember]
        public string OperationValue { get; private set; }
        [DataMember]
        public string AuthenticationToken { get; private set; }
    }

    public enum AuthorizationError
    {
        None,
        InvalidAuthorizationSettings,
        InvalidAuthenticationToken,
        NoResponseFromAuthorizationServer,
        UnauthorizedToken,
        UnknownError
    }

    [DataContract]
    public class AuthorizationResponse : Response
    {
        public AuthorizationResponse(IRequest request, AuthorizationError authorizationError)
            : base(ResponderId.FromString("Authorization"), ResponseId.New(), request)
        {
            AuthorizationError = authorizationError;
        }

        [DataMember]
        public AuthorizationError AuthorizationError { get; private set; }
    }
}
