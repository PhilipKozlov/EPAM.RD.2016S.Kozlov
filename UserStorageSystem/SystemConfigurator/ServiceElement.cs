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
    public class ServiceElement : ConfigurationElement
    {
        #region Properties
        /// <summary>
        /// Gets or sets service type.
        /// </summary>
        [ConfigurationProperty("serviceType", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string ServiceType
        {
            get
            {
                return ((string)(base["serviceType"]));
            }
            set
            {
                base["serviceType"] = value;
            }
        }

        /// <summary>
        /// Gets or sets number of services.
        /// </summary>
        [ConfigurationProperty("number", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string Number
        {
            get
            {
                return ((string)(base["number"]));
            }
            set
            {
                base["number"] = value;
            }
        }
        #endregion
    }
}
