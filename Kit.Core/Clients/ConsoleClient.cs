using Kit.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kit.Clients {
    public class ConsoleClient : ILogClient {

        #region ILogClient

        public Task LogAsync(string message, CancellationToken cancellationToken, LogLevel level = LogLevel.Log) {
            WriteLine(message, ConsoleColor.DarkGray);
            return Task.CompletedTask;
        }

        public Task LogInfoAsync(string message, CancellationToken cancellationToken) {
            WriteLine(message);
            return Task.CompletedTask;
        }

        public Task LogWarningAsync(string message, CancellationToken cancellationToken) {
            WriteLine(message, ConsoleColor.Yellow);
            return Task.CompletedTask;
        }

        public Task LogErrorAsync(string message, CancellationToken cancellationToken) {
            WriteLine(message, ConsoleColor.Red);
            return Task.CompletedTask;
        }

        #endregion

        public void WriteLine(string message, ConsoleColor? color = null) =>
            Write($"{message}\r\n", color);

        public void Write(string message, ConsoleColor? color = null) {
            var time = DateTime.Now.ToString("HH:mm:ss");
            var origColor = Console.ForegroundColor;

            if (color != null)
                Console.ForegroundColor = (ConsoleColor)color;

            if (message.StartsWith("\r")) {
                message = message.Substring(1);
                Console.Write($"\r{time} - {message}");
            }
            else
                Console.Write($"{time} - {message}");

            Console.ForegroundColor = origColor;
        }
    }
}
