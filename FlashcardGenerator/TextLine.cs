using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
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
        #endregion

        public SizeF ComputeSize(Graphics graphics) {
            return graphics.MeasureString(this.Text, this.Font);
        }

        public float DetermineMaxFontSize(int baseImageWidth, Graphics graphics) {
            double targetSize = baseImageWidth * 0.80;
            var font = GetBaseFont();

            while (graphics.MeasureString(text, font).Width < targetSize) {
                font = new Font(font.FontFamily, font.Size + 1, font.Style, font.Unit);
            }

            return font.Size - 5;
        }

        public void AddLineToPath(GraphicsPath graphicsPath, Point origin) {
            graphicsPath.AddString(this.Text, this.Font.FontFamily, (int) this.Font.Style, this.Font.Size, origin, StringFormat.GenericTypographic);
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
    }
}
