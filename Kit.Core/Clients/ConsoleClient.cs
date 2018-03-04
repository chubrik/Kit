﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Kit {
    public class ConsoleClient : ILogClient {

        private static ConsoleClient instance;
        public static ConsoleClient Instance => instance ?? (instance = new ConsoleClient());
        private ConsoleClient() { }

        private static bool isInitialized;
        private static Queue<Action> queue = new Queue<Action>();
        public static ConsolePosition Position = new ConsolePosition(0, 0);
        private static LogLevel minLevel = LogLevel.Info;

        #region Setup & Initialize

        public static void Setup(LogLevel? minLevel = null) {

            if (minLevel != null)
                ConsoleClient.minLevel = (LogLevel)minLevel;
        }

        private static void Initialize() {
            Debug.Assert(!isInitialized);

            if (isInitialized)
                throw new InvalidOperationException();

            isInitialized = true;

            new Thread(new ThreadStart(async () => {
                try {
                    while (true) {
                        if (queue.Count > 0)
                            queue.Dequeue()?.Invoke();
                        else
                            await Task.Delay(50, Kit.CancellationToken);
                    }
                }
                catch (TaskCanceledException) {
                    LogService.Log("Console thread stopped");
                }
            })).Start();

            LogService.Log("Console thread started");
        }

        #endregion

        #region ILogClient

        public void PushToLog(string message, LogLevel level = LogLevel.Log) {

            if (level < minLevel)
                return;

            ConsoleColor? color;

            switch (level) {

                case LogLevel.Log:
                    color = ConsoleColor.DarkGray;
                    break;

                case LogLevel.Info:
                    color = null;
                    break;

                case LogLevel.Success:
                    color = ConsoleColor.Green;
                    break;

                case LogLevel.Warning:
                    color = ConsoleColor.Yellow;
                    break;

                case LogLevel.Error:
                    color = ConsoleColor.Red;
                    break;

                default:
                    Debug.Fail(string.Empty);
                    throw new ArgumentOutOfRangeException(nameof(level));
            }

            var fullMessage = $"{DateTimeOffset.Now.ToString("HH:mm:ss")} - {message}";
            WriteLine(message, color);
        }

        #endregion

        public static ConsolePosition WriteLine() => WriteLine(string.Empty);

        public static ConsolePosition WriteLine(string text, ConsoleColor? color = null, ConsolePosition position = null) {
            var startPosition = position ?? Position;
            WriteBase($"{text}\r\n", color, startPosition);
            return Position = new ConsolePosition(startPosition.Top + 1, 0);
        }

        public static ConsolePosition Write(string text, ConsoleColor? color = null, ConsolePosition position = null) {
            var startPosition = position ?? Position;
            WriteBase(text, color, startPosition);
            return Position = new ConsolePosition(startPosition.Top, startPosition.Left + text.Length);
        }

        private static void WriteBase(string text, ConsoleColor? color, ConsolePosition position) {

            if (!isInitialized)
                Initialize();

            queue.Enqueue(() => {
                var originalColor = Console.ForegroundColor;
                var originalTop = Console.CursorTop;
                var originalLeft = Console.CursorLeft;

                if (color != null)
                    Console.ForegroundColor = (ConsoleColor)color;

                try {
                    Console.CursorTop = position.Top;
                    Console.CursorLeft = position.Left;
                    Console.Write(text);
                }
                catch (ArgumentOutOfRangeException) {
                    //todo
                }
                finally {
                    //Console.CursorTop = originalTop;
                    //Console.CursorLeft = originalLeft;
                }

                Console.ForegroundColor = originalColor;
            });
        }
    }
}
