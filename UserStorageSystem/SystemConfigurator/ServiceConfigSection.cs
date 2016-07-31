//-----------------------------------------------------------------------
// <copyright file="ServiceConfigSection.cs" company="No Company">
//     No Company. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace SystemConfigurator
{
    using System.Configuration;

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
        public ServicesCollection ServiceItems => (ServicesCollection)base["Services"];

        #endregion
    }
}
