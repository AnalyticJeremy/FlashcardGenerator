using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FlashcardGenerator
{
    public class OutputDeckFile : Dictionary<string, List<string>>
    {
        public List<string> OrderedKeys {
            get {
                return orderedKeys;
            }
        }

        public void Add(string cardID, string keyValue, string imageName) {
            string fullKey = string.Format("{0}-{1}", cardID, keyValue);

            if (this.ContainsKey(fullKey) == false) {
                this.Add(fullKey, new List<string>());
                orderedKeys.Add(fullKey);
            }

            this[fullKey].Add(imageName);
        }

        public void WriteFile(string outputDirectory, string deckName) {
            string fullOutputFileName = Path.Combine(outputDirectory, deckName + ".txt");
            string fileText = BuildFileText();
            File.WriteAllText(fullOutputFileName, fileText);
        }

        private string BuildFileText() {
            StringBuilder output = new StringBuilder();
            int maxCount = this.Values.Max(v => v.Count);

            output.AppendLine(BuildHeader(maxCount));

            foreach (string key in orderedKeys) {
                output.AppendLine(BuildLine(this[key]));
            }

            return output.ToString();
        }

        private static string BuildHeader(int numberOfColumns) {
            List<string> headings = new List<string>();

            for (int i = 1; i <= numberOfColumns; i++) {
                headings.Add("Picture " + i);
            }

            return BuildLine(headings);
        }

        private static string BuildLine(List<string> items) {
            StringBuilder output = new StringBuilder();

            foreach (var item in items) {
                output.Append(item);
                output.Append("\t");
            }

            return output.ToString().TrimEnd(new char[] { '\t' });
        }

        private List<string> orderedKeys = new List<string>();
    }
}
