using System;

namespace Kit {
    public class ConsolePosition {

        public int Top { get; }
        public int Left { get; }

        public ConsolePosition() {
            Top = ConsoleClient.Position.Top;
            Left = ConsoleClient.Position.Left;
        }

        public ConsolePosition(int top, int left) {
            Top = top;
            Left = left;
        }
        
        public ConsolePosition Write(string text, ConsoleColor? color = null) =>
            ConsoleClient.Write(text, color: color, position: this);
    }
}
