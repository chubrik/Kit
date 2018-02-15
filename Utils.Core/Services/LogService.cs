using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Utils.Abstractions;

namespace Utils.Services {
    public class LogService {

        private static LogService instance;
        public static LogService Instance => instance ?? (instance = new LogService());
        private LogService() { }

        public readonly List<ILogClient> LogClients = new List<ILogClient>();

        private bool isEnable = true;
        private bool isInitialized = false;

        public void Setup(bool? isEnable = null) {

            if (isEnable != null)
                this.isEnable = (bool)isEnable;
        }

        private async Task InitializeAsync(CancellationToken cancellationToken) {
            Debug.Assert(!isInitialized);

            if (isInitialized)
                throw new InvalidOperationException();
            
            isInitialized = true;
            await LogAsync("Initialize LogService", cancellationToken);
        }

        public Task LogAsync(string message, CancellationToken cancellationToken) =>
            PushAsync(message, cancellationToken, LogLevel.Log);

        public Task InfoAsync(string message, CancellationToken cancellationToken) =>
            PushAsync(message, cancellationToken, LogLevel.Info);

        public Task WarningAsync(string message, CancellationToken cancellationToken) =>
            PushAsync(message, cancellationToken, LogLevel.Warning);

        public Task ErrorAsync(string message, CancellationToken cancellationToken) =>
            PushAsync(message, cancellationToken, LogLevel.Error);

        public async Task PushAsync(string message, CancellationToken cancellationToken, LogLevel level = LogLevel.Log) {

            if (!isEnable)
                return;

            if (!isInitialized)
                await InitializeAsync(cancellationToken);

            foreach (var logClient in LogClients)
                await logClient.LogAsync(message, cancellationToken, level);
        }
    }
}
