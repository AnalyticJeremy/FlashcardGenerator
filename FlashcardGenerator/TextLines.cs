using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml;
using System.Drawing.Drawing2D;

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
                int x = (this.BaseImageSize.Width - this.Width) / 2;
                int y = (this.BaseImageSize.Height - this.Height) / 2;
                return new Point(x, y);
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
            graphics.DrawPath(new Pen(Brushes.Black, 2), graphicsPath);
        }

        public static TextLines ParseXmlNode(XmlNode sideNode, XmlNode itemNode, Image baseImage) {
            var output = new TextLines(baseImage);
            var lineNodes = GeneratorXmlFile.GetXmlNodeList(sideNode, "text/line");

            foreach (var lineNode in lineNodes) {
                var textLine = new TextLine();

                foreach (XmlNode childNode in lineNode.ChildNodes) {
                    if (childNode.Name == "element") {
                        string elementName = GeneratorXmlFile.GetAttributeValue(childNode, "name");
                        textLine.Text += itemNode.SelectSingleNode(elementName).InnerText;
                    }
                    else if (childNode.Name == "label") {
                        textLine.Text += childNode.InnerText;
                    }
                }

                float size = textLine.DetermineMaxFontSize(output.BaseImageSize.Width, output.Graphics);
                textLine.Font = TextLine.GetBaseFont(size);

                output.Add(textLine);
            }

            return output;
        }

        private Image baseImage = null;
        private Graphics graphics = null;
    }
}
