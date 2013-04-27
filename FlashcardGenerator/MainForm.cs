using System;
using System.Drawing;
using System.Windows.Forms;

namespace FlashcardGenerator
{
    public partial class MainForm : Form, IOutputForm
    {
        public MainForm() {
            InitializeComponent();
        }

        private void browseButton_Click(object sender, EventArgs e) {
            var result = folderBrowserDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK) {
                inputDirectoryTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void generateButton_Click(object sender, EventArgs e) {
            outputRichTextBox.Clear();
            FlashcardGenerator.Generate(inputDirectoryTextBox.Text, this);
        }

        public void WriteOutputMessage(string message) {
            WriteOutputMessage(message, outputRichTextBox.ForeColor);
        }

        public void WriteOutputMessage(string message, Color color) {
            message = message.TrimEnd(new char[] { ' ', '\n', '\r', '\t' });
            message += "\n";
            outputRichTextBox.AppendText(message, color);
            outputRichTextBox.ScrollToCaret();
        }

        public void WriteOutputErrorMessage(string message) {
            Color errorColor = Color.FromArgb(192, 0, 0);
            WriteOutputMessage(message, errorColor);
        }
    }
}
