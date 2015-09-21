using System.Collections;
using System.Collections.Generic;
using System.Configuration;

namespace ReactiveServices.Authorization.ConfigurationSections
{
    [ConfigurationCollection(typeof(AuthorizationRulesElement), CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
    public class AuthorizationRulesCollection : ConfigurationElementCollection, IEnumerable<AuthorizationRulesElement>
    {
        internal const string ItemPropertyName = "AuthorizationRule";

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
            return ((AuthorizationRulesElement)element).OperationName + "|" + ((AuthorizationRulesElement)element).OperationValue;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new AuthorizationRulesElement();
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        public new IEnumerator<AuthorizationRulesElement> GetEnumerator()
        {
            return new AuthorizationRulesCollectionEnumerator(this);
        }

        public sealed class AuthorizationRulesCollectionEnumerator : IEnumerator<AuthorizationRulesElement>
        {
            public AuthorizationRulesCollectionEnumerator(AuthorizationRulesCollection collection)
            {
                Collection = collection;
            }

            private readonly AuthorizationRulesCollection Collection;

            public AuthorizationRulesElement Current
            {
                get { return (this as IEnumerator).Current as AuthorizationRulesElement; }
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
