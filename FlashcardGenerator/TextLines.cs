using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Xml;

namespace FlashcardGenerator
{
    public class TextLines : List<TextLine>, IDisposable
    {
        #region Constructors
        private TextLines() {
        }

        private TextLines(Image baseImage) {
            this.baseImage = baseImage;
            this.graphics = Graphics.FromImage(this.baseImage);
        }
        #endregion

        #region Properties
        public Graphics Graphics {
            get {
                return graphics;
            }
        }

        public int Width {
            get {
                return (int) this.Max(t => t.ComputeSize(this.graphics).Width);
            }
        }

        public int Height {
            get {
                return (int)this.Sum(t => t.ComputeSize(this.graphics).Height);
            }
        }

        public Size BaseImageSize {
            get {
                return baseImage.Size;
            }
        }

        public Point Origin {
            get {
                var size = new Size(this.Width, this.Height);
                var output = FlashcardGenerator.ComputeOrigin(this.BaseImageSize, size);

                if (top >= 0) {
                    output.Y = top;
                }

                return output;
            }
        }
        #endregion

        public void Dispose() {
            graphics.Dispose();
            baseImage.Dispose();
        }

        public void DrawPath(GraphicsPath graphicsPath) {
            // Draw the path onto the base image
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.FillPath(Brushes.White, graphicsPath);
            graphics.DrawPath(new Pen(Brushes.Black, 1), graphicsPath);
        }

        public static TextLines ParseXmlNode(XmlNode sideNode, XmlNode itemNode, Image baseImage) {
            var output = new TextLines(baseImage);
            var textNode = sideNode.SelectSingleNode("text");
            var lineNodes = GeneratorXmlFile.GetXmlNodeList(textNode, "line");

            output.top = GeneratorXmlFile.GetIntFromAttribute(textNode, "top", -1);

            foreach (var lineNode in lineNodes) {
                var textLine = new TextLine();

                textLine.Margin = GeneratorXmlFile.GetIntFromAttribute(lineNode, "margin", 0);
                textLine.FontSize = GeneratorXmlFile.GetIntFromAttribute(lineNode, "font-size", 0);
                textLine.Left = GeneratorXmlFile.GetIntFromAttribute(lineNode, "left", -1);
                textLine.TabStops = GeneratorXmlFile.GetAttributeValue(lineNode, "tab-stops");

                foreach (XmlNode childNode in lineNode.ChildNodes) {
                    if (childNode.Name == "element") {
                        string elementName = GeneratorXmlFile.GetAttributeValue(childNode, "name");
                        var elementNode = itemNode.SelectSingleNode(elementName);

                        if (elementNode == null) {
                            throw new XmlException("\"item\" node does not contain a child node named \"" + elementName + "\".");
                        }
                        else {
                            textLine.Text += elementNode.InnerText;
                        }
                    }
                    else if (childNode.Name == "label") {
                        textLine.Text += childNode.InnerText;
                    }
                }


                float size = textLine.FontSize;
                float maxSize = textLine.DetermineMaxFontSize(output.BaseImageSize.Width, output.Graphics);
                if (size <= 0 || size > maxSize) {
                    size = maxSize;
                }
                textLine.Font = TextLine.GetBaseFont(size);

                output.Add(textLine);
            }

            return output;
        }

        private int top = -1;
        private Image baseImage = null;
        private Graphics graphics = null;
    }
}
