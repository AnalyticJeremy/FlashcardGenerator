using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FlashcardGenerator
{
    abstract public class GeneratorXmlFile : XmlDocument
    {
        #region Constructors
        private GeneratorXmlFile() {
        }

        public GeneratorXmlFile(string inputDirectory) {
            baseDirectory = inputDirectory;
        }
        #endregion

        #region Properties
        protected string BaseDirectory {
            get {
                return baseDirectory;
            }
        }

        public abstract string FileName { get; }
        #endregion

        protected XmlNode GetXmlNode(string xpath) {
            var output = this.SelectSingleNode(xpath);

            if (output != null) {
                return output;
            }
            else {
                throw new XmlException(string.Format("\"{0}\" does not contain a \"{1}\" node.", this.FileName, xpath));
            }
        }

        public static List<XmlNode> GetXmlNodeList(XmlNode parentNode, string xpath) {
            XmlNodeList xmlNodeList = parentNode.SelectNodes(xpath);
            var output = new List<XmlNode>();

            if (xmlNodeList != null) {
                foreach (XmlNode xmlNode in xmlNodeList) {
                    output.Add(xmlNode);
                }
            }

            return output;
        }

        public static string GetAttributeValue(XmlNode xmlNode, string name) {
            var attribute = xmlNode.Attributes[name];
            if (attribute != null) {
                return attribute.Value.Trim();
            }
            else {
                return "";
            }
        }

        private string baseDirectory = null;
    }
}
