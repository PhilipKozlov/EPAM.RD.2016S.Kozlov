using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemConfigurator
{
    /// <summary>
    /// Represents a service configuration element within a configuration file.
    /// </summary>
    internal class ServiceElement : ConfigurationElement
    {
        #region Properties
        /// <summary>
        /// Gets or sets service type.
        /// </summary>
        [ConfigurationProperty("type", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string Type
        {
            get
            {
                return ((string)(base["type"]));
            }
            set
            {
                base["type"] = value;
            }
        }

        /// <summary>
        /// Gets or sets service id.
        /// </summary>
        [ConfigurationProperty("id", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Id
        {
            get
            {
                return ((string)(base["id"]));
            }
            set
            {
                base["id"] = value;
            }
        }

        /// <summary>
        /// Gets or sets service type.
        /// </summary>
        [ConfigurationProperty("role", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string Role
        {
            get
            {
                return ((string)(base["role"]));
            }
            set
            {
                base["role"] = value;
            }
        }

        #endregion
    }
}
