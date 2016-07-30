using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemConfigurator
{
    internal class SlaveElement : ConfigurationElement
    {
        #region Properties

        /// <summary>
        /// Gets or sets service host.
        /// </summary>
        [ConfigurationProperty("host", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Host
        {
            get
            {
                return (string)base["host"];
            }
            set
            {
                base["host"] = value;
            }
        }

        /// <summary>
        /// Gets or sets service port.
        /// </summary>
        [ConfigurationProperty("port", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Port
        {
            get
            {
                return (string)base["port"];
            }
            set
            {
                base["port"] = value;
            }
        }
        #endregion
    }
}
