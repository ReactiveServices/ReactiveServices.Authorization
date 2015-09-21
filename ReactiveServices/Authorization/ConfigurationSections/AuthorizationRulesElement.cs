using System.Configuration;

namespace ReactiveServices.Authorization.ConfigurationSections
{
    public class AuthorizationRulesElement : ConfigurationElement
    {
        [ConfigurationProperty("OperationName", DefaultValue = null, IsRequired = false, IsKey = true)]
        public string OperationName
        {
            get { return (string)this["OperationName"]; }
            set { this["OperationName"] = value; }
        }
        [ConfigurationProperty("OperationValue", DefaultValue = null, IsRequired = true, IsKey = true)]
        public string OperationValue
        {
            get { return (string)this["OperationValue"]; }
            set { this["OperationValue"] = value; }
        }
        [ConfigurationProperty("AuthorizationServiceName", DefaultValue = null, IsRequired = true, IsKey = true)]
        public string AuthorizationServiceName
        {
            get { return (string)this["AuthorizationServiceName"]; }
            set { this["AuthorizationServiceName"] = value; }
        }
    }
}
