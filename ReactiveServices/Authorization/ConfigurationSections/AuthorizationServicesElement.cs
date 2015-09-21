using System.Configuration;

namespace ReactiveServices.Authorization.ConfigurationSections
{
    public class AuthorizationServicesElement : ConfigurationElement
    {
        [ConfigurationProperty("ServiceName", DefaultValue = null, IsRequired = false, IsKey = true)]
        public string ServiceName
        {
            get { return (string)this["ServiceName"]; }
            set { this["ServiceName"] = value; }
        }
        [ConfigurationProperty("AuthorizationRequestSubscriptionId", DefaultValue = null, IsRequired = true, IsKey = true)]
        public string AuthorizationRequestSubscriptionId
        {
            get { return (string)this["AuthorizationRequestSubscriptionId"]; }
            set { this["AuthorizationRequestSubscriptionId"] = value; }
        }
    }
}
