using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace FlashcardGenerator
{
    public class DeckFile : GeneratorXmlFile
    {
        #region Constructors
        public DeckFile(string inputDirectory) : base(inputDirectory) {
            this.Load(BuildFilePath(inputDirectory));
        }
        #endregion

        #region Properties
        public override string FileName {
            get {
                return DeckFileName;
            }
        }

        public string DeckName {
            get {
                return GetAttributeValue(this.DeckNode, "name");
            }
        }

        public string MediaDirectoryName {
            get {
                return this.DeckName + " Media";
            }
        }

        public string DefaultBackground {
            get {
                return GetPathFromAttribute(this.DeckNode, "background");
            }
        }

        private XmlNode DeckNode {
            get {
                return GetXmlNode("/deck");
            }
        }

        public List<XmlNode> Cards {
            get {
                return GetXmlNodeList(this.DeckNode, "cards/card");
            }
        }

        public List<XmlNode> Templates {
            get {
                return GetXmlNodeList(this.DeckNode, "templates/template");
            }
        }
        #endregion

        public string GetPathFromAttribute(XmlNode xmlNode, string name) {
            string output = GetAttributeValue(xmlNode, name);

            if (output != "") {
                output = Path.Combine(this.BaseDirectory, output);
            }

            return output;
        }

        public static string BuildFilePath(string inputDirectory) {
            return Path.Combine(inputDirectory, DeckFileName);
        }

        public const string DeckFileName = "deck.xml";
    }
}
