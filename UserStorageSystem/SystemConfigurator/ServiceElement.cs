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
        ///// <summary>
        ///// Gets or sets service type.
        ///// </summary>
        //[ConfigurationProperty("type", DefaultValue = "", IsKey = true, IsRequired = true)]
        //public string Type
        //{
        //    get
        //    {
        //        return ((string)(base["type"]));
        //    }
        //    set
        //    {
        //        base["type"] = value;
        //    }
        //}

        /// <summary>
        /// Gets or sets service type.
        /// </summary>
        [ConfigurationProperty("role", DefaultValue = "", IsKey = true, IsRequired = true)]
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
