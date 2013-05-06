using System;
using System.Drawing;

namespace FlashcardGenerator
{
    public interface IOutputForm
    {
        void WriteOutputMessage(string message);
        void WriteOutputMessage(string message, Color color);
        void WriteOutputErrorMessage(string message);
    }
}
