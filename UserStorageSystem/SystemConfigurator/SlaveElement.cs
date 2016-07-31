//-----------------------------------------------------------------------
// <copyright file="SlaveElement.cs" company="No Company">
//     No Company. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace SystemConfigurator
{
    using System.Configuration;

    /// <summary>
    /// Represents a service configuration element within a configuration file.
    /// </summary>
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
