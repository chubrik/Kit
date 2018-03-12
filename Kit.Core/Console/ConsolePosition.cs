using System;

namespace Kit {
    public class ConsolePosition {

        public int Top { get; }
        public int Left { get; }

        public ConsolePosition(int top, int left) {
            Top = top;
            Left = left;
        }

        public bool Equals(ConsolePosition other) =>
            Top == other.Top && Left == other.Left;

        public ConsolePosition Write(string text, ConsoleColor? color = null) =>
            ConsoleClient.Write(text, color: color, position: this);
    }
}
