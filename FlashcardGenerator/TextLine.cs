using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FlashcardGenerator
{
    public class TextLine
    {
        public TextLine() {
        }

        #region Properties
        public string Text {
            get {
                return text;
            }
            set {
                text = value;
            }
        }

        public Font Font {
            get {
                return font;
            }
            set {
                font = value;
            }
        }

        public int Margin {
            get {
                return margin;
            }
            set {
                margin = value;
            }
        }

        public int FontSize {
            get {
                return fontSize;
            }
            set {
                fontSize = value;
            }
        }

        public int Left {
            get {
                return left;
            }
            set {
                left = value;
            }
        }

        public string TabStops {
            get {
                return tabStops;
            }
            set {
                tabStops = value.Trim();
            }
        }

        public StringFormat StringFormat {
            get {
                var stringFormat = StringFormat.GenericTypographic;

                if (this.TabStops != "") {
                    var tabStopStrings = this.TabStops.Split(new char[] { ',' });
                    var tabStopFloats = new List<float>();

                    foreach (var tabStopString in tabStopStrings) {
                        float tabStopFloat = 0;
                        float.TryParse(tabStopString, out tabStopFloat);
                        tabStopFloats.Add(tabStopFloat);
                    }

                    stringFormat.SetTabStops(0, tabStopFloats.ToArray());
                }

                return stringFormat;
            }
        }
        #endregion

        public override string ToString() {
            return "TextLine: " + this.Text;
        }

        public SizeF ComputeSize(Graphics graphics) {
            return ComputeSize(graphics, this.Font);
        }

        private SizeF ComputeSize(Graphics graphics, Font font) {
            return graphics.MeasureString(this.Text, font, 2000, this.StringFormat);
        }

        public float DetermineMaxFontSize(int baseImageWidth, Graphics graphics) {
            double targetSize = baseImageWidth * 0.95;
            var font = GetBaseFont();

            while (this.ComputeSize(graphics, font).Width < targetSize) {
                font = new Font(font.FontFamily, font.Size + 1, font.Style, font.Unit);
            }

            return font.Size - 1;
        }

        public void AddLineToPath(GraphicsPath graphicsPath, Point origin) {
            string formattedText = this.Text;

            formattedText = formattedText.Replace("\\t", new string('\t', 1));

            graphicsPath.AddString(formattedText, this.Font.FontFamily, (int)this.Font.Style, this.Font.Size, origin, this.StringFormat);
        }

        public static Font GetBaseFont() {
            return GetBaseFont(10);
        }

        public static Font GetBaseFont(float size) {
            var fontFamily = new FontFamily("Verdana");
            return new Font(fontFamily, size, FontStyle.Bold, GraphicsUnit.Pixel);
        }

        private string text = "";
        private Font font = null;
        private int fontSize = 0;
        private int margin = 0;
        private int left = 0;
        private string tabStops = "";
    }
}
