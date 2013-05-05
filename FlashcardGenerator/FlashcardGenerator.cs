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
                outputForm.WriteOutputMessage("Starting flashcard generation...");
                outputForm.WriteOutputMessage("");

                InputDirectory = inputDirectory;

                ValidateInput();
                var itemsFile = new ItemsFile(inputDirectory);
                var deckFile = new DeckFile(inputDirectory);

                CreateOutputDirectory(deckFile);

                foreach (var card in deckFile.Cards) {
                    GenerateCardImages(card, itemsFile, deckFile, outputForm);
                }

                outputForm.WriteOutputMessage("");
                outputForm.WriteOutputMessage("Done!", Color.Green);
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
                string keyValue = itemsFile.GetKeyValueFromItemNode(item);
                string outputFilename = string.Format("{0}-{1}-{2}.png", cardID, keyValue, sideCounter).ToLower();
                string fullOutputFileName = Path.Combine(OutputDirectory, deckFile.MediaDirectoryName, outputFilename);

                if (cardSideTypeID == CardSideType.Image) {
                    outputForm.WriteOutputMessage("   - Generating image card side file " + outputFilename);
                    GenerateCardImageSideImage(sideNode, item, itemsFile, (Image) baseImage.Clone(), fullOutputFileName);
                }
                else if (cardSideTypeID == CardSideType.Text) {
                    outputForm.WriteOutputMessage("   - Generating text card side file " + outputFilename);
                    GenerateCardTextSideImage(sideNode, item, (Image) baseImage.Clone(), fullOutputFileName);
                }
            }

            baseImage.Dispose();
        }

        #region GenerateCardImageSideImage
        private static void GenerateCardImageSideImage(XmlNode sideNode, XmlNode itemNode, ItemsFile itemsFile, Image baseImage, string fullOutputFileName) {
            var imageNode = sideNode.SelectSingleNode("image");
            string imageNameFormat = GeneratorXmlFile.GetAttributeValue(imageNode, "name");

            if (imageNameFormat == "") {
                throw new XmlException("\"image\" node does not have \"name\" attribute.");
            }

            string imageName = itemsFile.ApplyKeyValueToString(imageNameFormat, itemNode);
            string fullImageFileName = Path.Combine(itemsFile.BaseDirectory, imageName);

            var itemImage = Image.FromFile(fullImageFileName);
            itemImage = AddBorderToImage(itemImage);
            itemImage = ScaleImage(itemImage, new Size(baseImage.Width - 10, baseImage.Height - 10));

            using (var graphics = Graphics.FromImage(baseImage)) {
                var point = ComputeOrigin(baseImage.Size, itemImage.Size);
                var rectangle = new Rectangle(point, itemImage.Size);

                graphics.DrawImage(itemImage, rectangle);
            }

            baseImage.Save(fullOutputFileName, ImageFormat.Png);

            baseImage.Dispose();
        }

        private static Bitmap AddBorderToImage(Image inputImage) {
            Bitmap newImage = new Bitmap(inputImage.Width + 2, inputImage.Height + 2);
            var rectangle = new Rectangle(new Point(1, 1), inputImage.Size);
        
            using (Graphics graphics = Graphics.FromImage(newImage)) {
                graphics.Clear(Color.Black);
                graphics.DrawImage(inputImage, rectangle);
            }

            return newImage;
        }

        private static Image ScaleImage(Image image, Size maxSize) {
            if (image.Width <= maxSize.Width && image.Height <= maxSize.Height) {
                return image;
            }
            else {
                var ratioX = (double) maxSize.Width / image.Width;
                var ratioY = (double) maxSize.Height / image.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var newWidth = (int)(image.Width * ratio);
                var newHeight = (int)(image.Height * ratio);

                var newImage = new Bitmap(newWidth, newHeight);
                Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);

                return newImage;
            }
        }
        #endregion

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

        public static Point ComputeOrigin(Size baseSize, Size itemSize) {
            int x = (baseSize.Width - itemSize.Width) / 2;
            int y = (baseSize.Height - itemSize.Height) / 2;
            return new Point(x, y);
        }
    }

    public enum CardSideType
    {
        Text,
        Image,
        Templated
    }
}
