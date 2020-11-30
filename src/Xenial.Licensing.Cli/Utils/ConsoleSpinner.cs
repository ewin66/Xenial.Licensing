using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Console;

namespace Xenial.Licensing.Cli.Utils
{
    /// <summary>
    /// A simple tool that creates a Console based waiting animation
    /// </summary>
    public class ConsoleSpinner
    {
        private int counter;

        /// <summary>
        /// Turns this instance.
        /// </summary>
        public void Turn()
        {
            AlignCursor();

            switch (counter % 4)
            {
                case 0: DoWrite("/"); counter = 0; break;
                case 1: DoWrite("-"); break;
                case 2: DoWrite("\\"); break;
                case 3: DoWrite("|"); break;
            }

            counter++;
        }

        /// <summary>
        /// Aligns the cursor to the Top position.
        /// </summary>
        protected virtual void AlignCursor()
            => SetCursorPosition(0, CursorTop);

        /// <summary>
        /// Writes the string to the Console.
        /// </summary>
        /// <param name="stringToWrite">The string to write.</param>
        protected virtual void DoWrite(string stringToWrite)
            => Write(stringToWrite);

        /// <summary>
        /// Clears the current console line.
        /// </summary>
        public void ClearLine()
        {
            var currentLineCursor = CursorTop;
            SetCursorPosition(0, CursorTop);
            Write(new string(' ', WindowWidth));
            SetCursorPosition(0, currentLineCursor);
        }
    }
}
