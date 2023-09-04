using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;


namespace NaptienMaster
{
   
        public class AppSettingElement : ConfigurationElement
        {
            [ConfigurationProperty("com", IsRequired = true)]
            public string Com
            {
                get
                {
                    return (string)base["com"];
                }
                set
                {
                    base["com"] = value;
                }
            }
        }
        public class AppSettingCollection : ConfigurationElementCollection
        {
            // Create a property that lets us access an element in the
            // collection with the int index of the element
            public AppSettingElement this[int index]
            {
                get
                {
                    // Gets the SageCRMInstanceElement at the specified
                    // index in the collection
                    return (AppSettingElement)BaseGet(index);
                }
                set
                {
                    // Check if a SageCRMInstanceElement exists at the
                    // specified index and delete it if it does
                    if (BaseGet(index) != null)
                        BaseRemoveAt(index);

                    // Add the new SageCRMInstanceElement at the specified
                    // index
                    BaseAdd(index, value);
                }
            }

            // Create a property that lets us access an element in the
            // colleciton with the name of the element
            public new AppSettingElement this[string key]
            {
                get
                {
                    // Gets the SageCRMInstanceElement where the name
                    // matches the string key specified
                    return (AppSettingElement)BaseGet(key);
                }
                set
                {
                    // Checks if a SageCRMInstanceElement exists with
                    // the specified name and deletes it if it does
                    if (BaseGet(key) != null)
                        BaseRemoveAt(BaseIndexOf(BaseGet(key)));

                    // Adds the new SageCRMInstanceElement
                    BaseAdd(value);
                }
            }

            // Method that must be overriden to create a new element
            // that can be stored in the collection
            protected override ConfigurationElement CreateNewElement()
            {
                return new AppSettingElement();
            }

            // Method that must be overriden to get the key of a
            // specified element
            protected override object GetElementKey(ConfigurationElement element)
            {
                return ((AppSettingElement)element).Com;
            }
        }
        public class AppSettingSection : ConfigurationSection
        {
            [ConfigurationProperty("setting")]
            [ConfigurationCollection(typeof(AppSettingCollection))]
            public AppSettingCollection collect
            {
                get
                {
                    return (AppSettingCollection)this["setting"];
                }
                set
                {
                    this["setting"] = value;
                }
            }
        }
        public class InforSettingSection : ConfigurationSection
        {
          
            [ConfigurationProperty("baud_rate", IsRequired = true)]
            public string Baudrate
            {
                get { return (string)this["baud_rate"]; }
                set
                {
                    this["baud_rate"] = value;
                }
            }
           
        }
    public class TokenSettingSection : ConfigurationSection
    {
        [ConfigurationProperty("access_token",IsRequired =true)]
        public string Token
        {
            get { return (string)this["access_token"]; }
            set
            {
                this["access_token"] = value;
            }
        }
        [ConfigurationProperty("status_save",IsRequired =true)]
        public bool StatusLogin
        {
            get { return (bool)this["status_save"]; }
            set
            {
                this["status_save"] = value;
            }
        }
    }
    }

