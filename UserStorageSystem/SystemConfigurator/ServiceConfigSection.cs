using System.Configuration;

namespace SystemConfigurator
{
    /// <summary>
    /// Represents a service config section within a configuration file.
    /// </summary>
    internal class ServiceConfigSection : ConfigurationSection
    {
        #region Properties
        /// <summary>
        /// Gets collection of service items from custom section within a configuration file.
        /// </summary>
        [ConfigurationProperty("Services")]
        //[ConfigurationProperty("", IsDefaultCollection =true)]
        public ServicesCollection ServiceItems
        {
            get { return ((ServicesCollection)(base["Services"])); }
        }
        #endregion
    }
}
