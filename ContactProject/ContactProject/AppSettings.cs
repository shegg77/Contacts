using System;
using System.Diagnostics;
using System.IO.IsolatedStorage;

namespace ContactProject
{
    public class AppSettings
    {
        // Isoloated storage settings
        IsolatedStorageSettings isoloatedStore;

        // Contact Sources
        const string UsesFacebookKeyName = "UsesFacebook";
        const string UsesSkypeKeyName = "UsesSkype";
        const string UsesPhoneBookKeyName = "UsesPhoneBook";
        const string IsLoadingContentKeyName = "IsLoadingContent";
        const string SearchItemKeyName = "SearchItem";
        const string FirstTimeKeyName = "FirstTime";
        // GridView
        const string HeaderKeyName = "UsesHeader";
        const string GridKeyName = "UsesGrid";

        // Default value of our settings
        const bool UsesFacebookDefault = false;
        const bool UsesSkypeDefault = false;
        const bool UsesPhoneBook = true;
        const bool IsLoadingContent = false;
        const string SearchItemDefault = null;
        const bool HeaderDefault = true;
        const bool GridDefault = false;
        const bool FirstTimeDefault = true;

        /// <summary>
        /// Constructor that gets the application settings.
        /// </summary>
        public AppSettings()
        {
            try
            {
                // Get the settings for this application.
                isoloatedStore = IsolatedStorageSettings.ApplicationSettings;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception while using IsloatedStorage: " + e.ToString());
            }
        }

        ///<summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            if (isoloatedStore.Contains(Key))
            {
                // If the value has changed
                if (isoloatedStore[Key] != value)
                {
                    // Store the new value
                    isoloatedStore[Key] = value;
                    value = true;
                }
            }
            // Create Key
            else
            {
                isoloatedStore.Add(Key, value);
                valueChanged = true;
            }

            return valueChanged;
        }

        ///<summary>
        ///Get the current value of the setting, or if it is not found,
        /// set the setting to the default setting.
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public ValueType GetValueOrDefault<ValueType>(string key, ValueType defaultValue)
        {
            ValueType value;

            // if the key exists, retrieve the value.
            if (isoloatedStore.Contains(key))
            {
                value = (ValueType)isoloatedStore[key];
            }
            // Use default
            else
            {
                value = defaultValue;
            }

            return value;
        }

        ///<summary>
        /// Save the settings.
        /// </summary>
        public void Save()
        {
            isoloatedStore.Save();
        }

        ///<summary>
        /// Property if content is loading
        ///</summary>
        public bool IsLoadingContentSetting
        {
            get
            {
                return GetValueOrDefault<bool>(IsLoadingContentKeyName, IsLoadingContent);
            }
            set
            {
                AddOrUpdateValue(IsLoadingContentKeyName, value);
                Save();
            }
        }

        public bool FirstTimeSetting
        {
            get
            {
                return GetValueOrDefault<bool>(FirstTimeKeyName, FirstTimeDefault);
            }
            set
            {
                AddOrUpdateValue(FirstTimeKeyName, value);
                Save();
            }
        }

        public string SearchItem
        {
            get
            {
                return GetValueOrDefault<string>(SearchItemKeyName, SearchItemDefault);
            }
            set
            {
                AddOrUpdateValue(SearchItemKeyName, value);
                Save();
            }
        }
        
        /// Property to get and set a CheckBox Setting Key.
        public bool UsesFacebookSetting
        {
            get
            {
                return GetValueOrDefault<bool>(UsesFacebookKeyName, UsesFacebookDefault);
            }
            set
            {
                AddOrUpdateValue(UsesFacebookKeyName, value);
                Save();
            }
        }

        public bool UsesHeaderSetting
        {
            get
            {
                return GetValueOrDefault<bool>(HeaderKeyName, HeaderDefault);
            }
            set
            {
                AddOrUpdateValue(HeaderKeyName, value);
                Save();
            }
        }

        public bool UsesSkypeSetting
        {
            get
            {
                return GetValueOrDefault<bool>(UsesSkypeKeyName, UsesSkypeDefault);
            }
            set
            {
                AddOrUpdateValue(UsesSkypeKeyName, value);
                Save();
            }
        }

        public bool UsesGridSetting
        {
            get
            {
                return GetValueOrDefault<bool>(GridKeyName, GridDefault);
            }
            set
            {
                AddOrUpdateValue(GridKeyName, value);
                Save();
            }
        }

        public bool UsesPhoneSetting
        {
            get
            {
                return GetValueOrDefault<bool>(UsesPhoneBookKeyName,
                    UsesPhoneBook);
            }
            set
            {
                AddOrUpdateValue(UsesPhoneBookKeyName, UsesPhoneBook);
                Save();
            }
        }
    }
}
