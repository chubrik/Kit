using System;

namespace Chubrik.Kit
{
    public class ConsolePosition
    {
        public int Top { get; }
        public int Left { get; }

        public ConsolePosition(int top, int left)
        {
            Top = top;
            Left = left;
        }

        public bool Equals(ConsolePosition other) => Top == other.Top && Left == other.Left;

        public bool HasEnoughWidthFor(string text) => Left + text.Length <= Console.WindowWidth;

        public ConsolePosition Write(string text, ConsoleColor? color = null) =>
            ConsoleClient.Write(text, color: color, position: this);
    }
}
