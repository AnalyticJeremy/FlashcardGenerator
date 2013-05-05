using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing.Drawing2D;

namespace FlashcardGenerator
{
    public static class FlashcardGenerator
    {
        #region Properties
        private static string InputDirectory { get; set; }

        private static string OutputDirectory {
            get {
                return Path.Combine(InputDirectory, "output");
            }
        }
        #endregion

        public static void Generate(string inputDirectory, IOutputForm outputForm) {
            try {
                InputDirectory = inputDirectory;

                ValidateInput();
                var itemsFile = new ItemsFile(inputDirectory);
                var deckFile = new DeckFile(inputDirectory);

                CreateOutputDirectory(deckFile);

                foreach (var card in deckFile.Cards) {
                    GenerateCardImages(card, itemsFile, deckFile, outputForm);
                }
            }
            catch (Exception exception) {
                outputForm.WriteOutputErrorMessage("");
                outputForm.WriteOutputErrorMessage(new string('=', 40));
                outputForm.WriteOutputErrorMessage("");
                outputForm.WriteOutputErrorMessage("ERROR: " + exception.Message);
                outputForm.WriteOutputErrorMessage("");
            }
        }

        private static void ValidateInput() {
            if (Directory.Exists(InputDirectory) == false) {
                throw new FileNotFoundException(string.Format("Input directory \"{0}\" does not exist.", InputDirectory));
            }

            if (File.Exists(ItemsFile.BuildFilePath(InputDirectory)) == false) {
                throw new FileNotFoundException(string.Format("Input directory \"{0}\" does not contain an items file ({2}).", InputDirectory, ItemsFile.ItemsFileName));
            }

            if (File.Exists(DeckFile.BuildFilePath(InputDirectory)) == false) {
                throw new FileNotFoundException(string.Format("Input directory \"{0}\" does not contain a deck file ({2}).", InputDirectory, DeckFile.DeckFileName));
            }
        }

        private static void CreateOutputDirectory(DeckFile deckFile) {
            if (Directory.Exists(OutputDirectory) == true) {
                Directory.Delete(OutputDirectory, true);
            }

            Directory.CreateDirectory(OutputDirectory);
            Directory.CreateDirectory(Path.Combine(OutputDirectory, deckFile.MediaDirectoryName));
        }

        private static void GenerateCardImages(XmlNode cardNode, ItemsFile itemsFile, DeckFile deckFile, IOutputForm outputForm) {
            string cardID = GeneratorXmlFile.GetAttributeValue(cardNode, "id");
            int sideCounter = 0;

            foreach (XmlNode sideNode in cardNode.SelectNodes("side")) {
                sideCounter++;

                outputForm.WriteOutputMessage(string.Format("Generating side {0} of card \"{1}\"", sideCounter, cardID));
                GenerateCardSideImage(sideNode, sideCounter, cardNode, cardID, itemsFile, deckFile, outputForm);
            }
        }

        private static void GenerateCardSideImage(XmlNode sideNode, int sideCounter, XmlNode cardNode, string cardID, ItemsFile itemsFile, DeckFile deckFile, IOutputForm outputForm) {
            var cardSideTypeID = DetermineCardSideType(sideNode);
            string baseImagePath = GetBaseImagePathForCard(sideNode, cardNode, deckFile);
            var baseImage = Image.FromFile(baseImagePath);

            foreach (var item in itemsFile.Items) {
                string keyValue = item.SelectSingleNode(itemsFile.KeyElementName).InnerText;
                string outputFilename = string.Format("{0}-{1}-{2}.png", cardID, keyValue, sideCounter).ToLower();
                string fullOutputFileName = Path.Combine(OutputDirectory, deckFile.MediaDirectoryName, outputFilename);

                if (cardSideTypeID == CardSideType.Image) {
                    outputForm.WriteOutputMessage("   - Generating image card side file " + outputFilename);
                    GenerateCardImageSideImage(sideNode, item, (Image) baseImage.Clone(), fullOutputFileName);
                }
                else if (cardSideTypeID == CardSideType.Text) {
                    outputForm.WriteOutputMessage("   - Generating text card side file " + outputFilename);
                    GenerateCardTextSideImage(sideNode, item, (Image) baseImage.Clone(), fullOutputFileName);
                }
            }
        }

        private static void GenerateCardImageSideImage(XmlNode sideNode, XmlNode itemNode, Image baseImage, string fullOutputFileName) {
        }

        private static void GenerateCardTextSideImage(XmlNode sideNode, XmlNode itemNode, Image baseImage, string fullOutputFileName) {
            var textLines = TextLines.ParseXmlNode(sideNode, itemNode, baseImage);
            var origin = textLines.Origin;

            using (var graphicsPath = new GraphicsPath()) {
                foreach (var textLine in textLines) {
                    textLine.AddLineToPath(graphicsPath, origin);

                    var size = textLine.ComputeSize(textLines.Graphics);
                    origin.Y += (int) size.Height;
                }

                textLines.DrawPath(graphicsPath);
            }

            baseImage.Save(fullOutputFileName, ImageFormat.Png);
            textLines.Dispose();
        }

        private static CardSideType DetermineCardSideType(XmlNode xmlNode) {
            var firstChild = xmlNode.FirstChild;

            if (firstChild == null) {
                throw new XmlException(string.Format("\"{0}\" node does not have any children.", xmlNode.Name));
            }
            else {
                switch (firstChild.Name) {
                    case "text":
                        return CardSideType.Text;

                    case "image":
                        return CardSideType.Image;

                    case "template":
                        return CardSideType.Templated;

                    default:
                        throw new XmlException(string.Format("\"{0}\" node has an unknown child node (\"{1}\").", xmlNode.Name, firstChild.Name));
                }
            }
        }

        private static string GetBaseImagePathForCard(XmlNode sideNode, XmlNode cardNode, DeckFile deckFile) {
            string output = deckFile.GetPathFromAttribute(sideNode, "background");

            if (output == "") {
                output = deckFile.GetPathFromAttribute(cardNode, "background");
            }

            if (output == "") {
                output = deckFile.DefaultBackground;
            }

            if (output == "") {
                throw new XmlException("No background image specified for card");
            }
            else if (File.Exists(output) == false) {
                throw new FileNotFoundException(string.Format("Background image \"{0}\" could not be found.", output), output);
            }

            return output;
        }
    }

    public enum CardSideType
    {
        Text,
        Image,
        Templated
    }
}
