//-----------------------------------------------------------------------
// <copyright file="SlaveCollection.cs" company="No Company">
//     No Company. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace SystemConfigurator
{
    using System.Configuration;

    /// <summary>
    /// Represents a configuration element containing a collection of slave service elements.
    /// </summary>
    [ConfigurationCollection(typeof(SlaveElement), AddItemName = "Slave")]
    internal class SlaveCollection : ConfigurationElementCollection
    {
        #region Properties

        /// <summary>
        /// Gets service element based on specified index in collection.
        /// </summary>
        /// <param name="i"> Element index.</param>
        /// <returns> ServiceElement instance.</returns>
        public SlaveElement this[int i] => (SlaveElement)BaseGet(i);

        #endregion

        #region ConfigurationElementCollection Methods

        /// <summary>
        /// Creates new service element.
        /// </summary>
        /// <returns> ConfigurationElement instance.</returns>
        protected override ConfigurationElement CreateNewElement() => new SlaveElement();
 
        /// <summary>
        /// Gets service element key.
        /// </summary>
        /// <param name="element"> ConfigurationElement instance.</param>
        /// <returns> Element key.</returns>
        protected override object GetElementKey(ConfigurationElement element) => ((SlaveElement)element).Port;

        #endregion
    }
}
