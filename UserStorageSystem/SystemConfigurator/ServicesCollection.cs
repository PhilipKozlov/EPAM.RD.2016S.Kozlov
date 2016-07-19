using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemConfigurator
{
    /// <summary>
    /// Represents a configuration element containing a collection of service elements.
    /// </summary>
    public class ServicesCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Gets service element based on specified index in collection.
        /// </summary>
        /// <param name="i"> Element index.</param>
        /// <returns> ServiceElement instance.</returns>
        public ServiceElement this[int i]
        {
            get
            {
                return (ServiceElement)BaseGet(i);
            }
        }

        #region ConfigurationElementCollection Methods
        /// <summary>
        /// Creates new service element.
        /// </summary>
        /// <returns> ConfigurationElement instance.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceElement();
        }

        /// <summary>
        /// Gets service element key.
        /// </summary>
        /// <param name="element"> ConfigurationElement instance.</param>
        /// <returns> Element key.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServiceElement)(element)).Role;
        }
        #endregion
    }
}
