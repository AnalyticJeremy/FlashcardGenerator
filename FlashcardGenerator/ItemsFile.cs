using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace FlashcardGenerator
{
    public class ItemsFile : GeneratorXmlFile
    {
        #region Constructors
        public ItemsFile(string inputDirectory) : base(inputDirectory) {
            this.Load(BuildFilePath(inputDirectory));
        }
        #endregion

        #region Properties
        public override string FileName {
            get {
                return ItemsFileName;
            }
        }

        public string KeyElementName {
            get {
                string output = GetAttributeValue(this.DocumentElement, "key");

                if (output == "") {
                    throw new XmlException("\"items\" element does not contain a \"key\" attribute.");
                }
                else {
                    return output;
                }
            }
        }

        public List<XmlNode> Items {
            get {
                return GetXmlNodeList(this.ItemsNode, "item");
            }
        }

        private XmlNode ItemsNode {
            get {
                return GetXmlNode("/items");
            }
        }
        #endregion

        public string GetKeyValueFromItemNode(XmlNode itemNode) {
            return itemNode.SelectSingleNode(this.KeyElementName).InnerText;
        }

        public string ApplyKeyValueToString(string formatString, XmlNode itemNode) {
            string keyValue = GetKeyValueFromItemNode(itemNode);
            return formatString.Replace(KeyValueMarker, keyValue);
        }

        public static string BuildFilePath(string inputDirectory) {
            return Path.Combine(inputDirectory, ItemsFileName);
        }

        public const string ItemsFileName = "items.xml";
        public const string KeyValueMarker = "{key}";
    }
}
