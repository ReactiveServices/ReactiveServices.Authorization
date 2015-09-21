using System.Configuration;

namespace ReactiveServices.Authorization.ConfigurationSections
{
    public class AuthorizationRulesSection : ConfigurationSection
    {
        [ConfigurationProperty("AuthorizationDomain", IsRequired = true)]
        public string AuthorizationDomain
        {
            get
            {
                return ((string)(base["AuthorizationDomain"]));
            }
            set
            {
                base["AuthorizationDomain"] = value;
            }
        }
        [ConfigurationProperty("AllowByDefault", IsRequired = false)]
        public bool AllowByDefault
        {
            get
            {
                return ((bool)(base["AllowByDefault"]));
            }
            set
            {
                base["AllowByDefault"] = value; 
            }
        }
        [ConfigurationProperty("DenyByDefault", IsRequired = false)]
        public bool DenyByDefault
        {
            get
            {
                return ((bool)(base["DenyByDefault"]));
            }
            set
            {
                base["DenyByDefault"] = value;
            }
        }
        [ConfigurationProperty("", IsRequired = false, IsKey = false, IsDefaultCollection = true)]
        public AuthorizationRulesCollection Items
        {
            get
            {
                return ((AuthorizationRulesCollection)(base[""]));
            }
            set
            {
                base[""] = value;
            }
        }
    }
}
