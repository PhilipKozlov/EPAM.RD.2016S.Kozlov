using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public ServicesCollection ServiceItems
        {
            get { return ((ServicesCollection)(base["Services"])); }
        }
        #endregion
    }
}
