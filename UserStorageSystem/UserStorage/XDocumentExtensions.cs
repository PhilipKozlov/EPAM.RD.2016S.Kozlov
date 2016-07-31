//-----------------------------------------------------------------------
// <copyright file="XDocumentExtensions.cs" company="No Company">
//     No Company. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace UserStorage
{
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// Provides additional functionality for XDocument type.
    /// </summary>
    internal static class XDocumentExtensions
    {
        /// <summary>
        /// Loads XDocument.
        /// </summary>
        /// <param name="doc"> XDocument instance.</param>
        /// <param name="reader"> XmlReader instance.</param>
        /// <param name="result"> Properly loaded <see cref="XDocument"/> instance or null.</param>
        /// <returns> True if xml is valid; otherwise - false.</returns>
        public static bool TryLoad(this XDocument doc, XmlReader reader, out XDocument result)
        {
            try
            {
                result = XDocument.Load(reader);
            }
            catch
            {
                result = null;
                return false;
            }
            finally
            {
                reader.Dispose();
            }

            return true;
        }
    }
}
