using System.Configuration;

namespace ReactiveServices.Authorization.ConfigurationSections
{
    public class AuthorizationServicesSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired = false, IsKey = false, IsDefaultCollection = true)]
        public AuthorizationServicesCollection Items
        {
            get
            {
                return ((AuthorizationServicesCollection)(base[""]));
            }
            set
            {
                base[""] = value;
            }
        }
    }
}
