using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemConfigurator
{
    [ConfigurationCollection(typeof(SlaveElement), AddItemName = "Slave")]
    internal class SlaveCollection : ConfigurationElementCollection
    {
        #region Properties
        /// <summary>
        /// Gets service element based on specified index in collection.
        /// </summary>
        /// <param name="i"> Element index.</param>
        /// <returns> ServiceElement instance.</returns>
        public SlaveElement this[int i]
        {
            get
            {
                return (SlaveElement)BaseGet(i);
            }
        }
        #endregion

        #region ConfigurationElementCollection Methods
        /// <summary>
        /// Creates new service element.
        /// </summary>
        /// <returns> ConfigurationElement instance.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new SlaveElement();
        }

        /// <summary>
        /// Gets service element key.
        /// </summary>
        /// <param name="element"> ConfigurationElement instance.</param>
        /// <returns> Element key.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SlaveElement)element).Port;
        }
        #endregion
    }
}
