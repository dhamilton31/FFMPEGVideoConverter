using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMPEGVIdeoConverter
{
    public class OutputTextEventArgs : EventArgs
    {
        private List<string> outputText;

        public OutputTextEventArgs()
        {
            outputText = new List<string>();
        }

        public void AddTextToOutput(string text)
        {
            outputText.Add(text);
        }

        public List<string> ReadOutputText()
        {
            return outputText;
        }
    }
}
