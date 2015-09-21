using System.Collections;
using System.Collections.Generic;
using System.Configuration;

namespace ReactiveServices.Authorization.ConfigurationSections
{
    [ConfigurationCollection(typeof(AuthorizationServicesElement), CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
    public class AuthorizationServicesCollection : ConfigurationElementCollection, IEnumerable<AuthorizationServicesElement>
    {
        internal const string ItemPropertyName = "AuthorizationService";

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMapAlternate; }
        }

        protected override string ElementName
        {
            get { return ItemPropertyName; }
        }

        protected override bool IsElementName(string elementName)
        {
            return (elementName == ItemPropertyName);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AuthorizationServicesElement)element).ServiceName;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new AuthorizationServicesElement();
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        public new IEnumerator<AuthorizationServicesElement> GetEnumerator()
        {
            return new AuthorizationServicesCollectionEnumerator(this);
        }

        public sealed class AuthorizationServicesCollectionEnumerator : IEnumerator<AuthorizationServicesElement>
        {
            public AuthorizationServicesCollectionEnumerator(AuthorizationServicesCollection collection)
            {
                Collection = collection;
            }

            private readonly AuthorizationServicesCollection Collection;

            public AuthorizationServicesElement Current
            {
                get { return (this as IEnumerator).Current as AuthorizationServicesElement; }
            }

            public void Dispose()
            {
            }

            private int EnumeratorIndex = -1;

            object IEnumerator.Current
            {
                get { return Collection.BaseGet(EnumeratorIndex); }
            }

            public bool MoveNext()
            {
                if (EnumeratorIndex < Collection.Count-1)
                {
                    EnumeratorIndex++;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                EnumeratorIndex = -1;
            }
        }
    }  
}
