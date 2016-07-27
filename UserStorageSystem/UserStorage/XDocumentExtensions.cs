using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;

namespace UserStorage
{
    internal static class XDocumentExtensions
    {
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
