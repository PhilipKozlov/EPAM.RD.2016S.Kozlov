//-----------------------------------------------------------------------
// <copyright file="ServiceElement.cs" company="No Company">
//     No Company. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace SystemConfigurator
{
    using System.Configuration;

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
                return (string)base["type"];
            }

            set
            {
                base["type"] = value;
            }
        }

        /// <summary>
        /// Gets or sets service Role.
        /// </summary>
        [ConfigurationProperty("role", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string Role
        {
            get
            {
                return (string)base["role"];
            }

            set
            {
                base["role"] = value;
            }
        }

        /// <summary>
        /// Gets or sets service host.
        /// </summary>
        [ConfigurationProperty("host", DefaultValue = "", IsKey = false, IsRequired = true)]
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

        /// <summary>
        /// Gets or sets service port to receive messages from the master.
        /// </summary>
        [ConfigurationProperty("internalCommunicationPort", DefaultValue = "", IsKey = true, IsRequired = false)]
        public string InternalCommunicationPort
        {
            get
            {
                return (string)base["internalCommunicationPort"];
            }

            set
            {
                base["internalCommunicationPort"] = value;
            }
        }

        /// <summary>
        /// Gets service Slaves collection.
        /// </summary>
        [ConfigurationProperty("Slaves", IsDefaultCollection = false, IsRequired = false)]
        public SlaveCollection Slaves => (SlaveCollection)base["Slaves"];

        #endregion
    }
}
