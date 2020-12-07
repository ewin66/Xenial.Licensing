using System;
using System.Xml.Linq;

namespace Xenial.Licensing.Cli.Utils
{
    internal static class XmlUtils
    {
        internal static string FormatXml(string xml)
        {
            try
            {
                var doc = XDocument.Parse(xml);
                return doc.ToString();
            }
            catch (Exception)
            {
                // Handle and throw if fatal exception here; don't just ignore them
                return xml;
            }
        }
    }
}
