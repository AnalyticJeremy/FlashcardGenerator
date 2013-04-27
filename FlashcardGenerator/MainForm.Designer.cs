namespace FlashcardGenerator
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.inputDirectoryLabel = new System.Windows.Forms.Label();
            this.inputDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.generateButton = new System.Windows.Forms.Button();
            this.outputRichTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // inputDirectoryLabel
            // 
            this.inputDirectoryLabel.AutoSize = true;
            this.inputDirectoryLabel.Location = new System.Drawing.Point(12, 9);
            this.inputDirectoryLabel.Name = "inputDirectoryLabel";
            this.inputDirectoryLabel.Size = new System.Drawing.Size(79, 13);
            this.inputDirectoryLabel.TabIndex = 0;
            this.inputDirectoryLabel.Text = "Input Directory:";
            // 
            // inputDirectoryTextBox
            // 
            this.inputDirectoryTextBox.Location = new System.Drawing.Point(97, 6);
            this.inputDirectoryTextBox.Name = "inputDirectoryTextBox";
            this.inputDirectoryTextBox.Size = new System.Drawing.Size(334, 20);
            this.inputDirectoryTextBox.TabIndex = 1;
            // 
            // browseButton
            // 
            this.browseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.browseButton.Image = global::FlashcardGenerator.Properties.Resources.file_open;
            this.browseButton.Location = new System.Drawing.Point(437, 1);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(28, 28);
            this.browseButton.TabIndex = 2;
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.ShowNewFolderButton = false;
            // 
            // generateButton
            // 
            this.generateButton.Location = new System.Drawing.Point(485, 4);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(73, 23);
            this.generateButton.TabIndex = 4;
            this.generateButton.Text = "&Generate!";
            this.generateButton.UseVisualStyleBackColor = true;
            this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
            // 
            // outputRichTextBox
            // 
            this.outputRichTextBox.Location = new System.Drawing.Point(12, 32);
            this.outputRichTextBox.Name = "outputRichTextBox";
            this.outputRichTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.outputRichTextBox.Size = new System.Drawing.Size(546, 208);
            this.outputRichTextBox.TabIndex = 5;
            this.outputRichTextBox.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(570, 252);
            this.Controls.Add(this.outputRichTextBox);
            this.Controls.Add(this.generateButton);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.inputDirectoryTextBox);
            this.Controls.Add(this.inputDirectoryLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "Flashcard Generator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label inputDirectoryLabel;
        private System.Windows.Forms.TextBox inputDirectoryTextBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Button generateButton;
        private System.Windows.Forms.RichTextBox outputRichTextBox;
    }
}

