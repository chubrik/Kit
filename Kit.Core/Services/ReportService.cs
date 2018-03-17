using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kit {
    public class ReportService {

        private ReportService() { }

        private const string _reportsDirectory = "reports";
        private static int _logCounter = 0;

        public static readonly List<IReportClient> Clients = new List<IReportClient> {
            FileClient.Instance
        };

        #region Extensions

        public static void ReportSuccess(string subject, string body, string attachmentPath = null) =>
            Report(subject, body, attachmentPath, logLevel: LogLevel.Success);

        public static void ReportWarning(string subject, string body, string attachmentPath = null) =>
            Report(subject, body, attachmentPath, logLevel: LogLevel.Warning);

        public static void ReportError(string subject, string body, string attachmentPath = null) =>
            Report(subject, body, attachmentPath, logLevel: LogLevel.Error);

        public static void ReportSuccess(string subject, string body, IEnumerable<string> attachmentPaths) =>
            Report(subject, body, attachmentPaths, logLevel: LogLevel.Success);

        public static void ReportWarning(string subject, string body, IEnumerable<string> attachmentPaths) =>
            Report(subject, body, attachmentPaths, logLevel: LogLevel.Warning);

        public static void ReportError(string subject, string body, IEnumerable<string> attachmentPaths) =>
            Report(subject, body, attachmentPaths, logLevel: LogLevel.Error);

        public static void Report(string subject, string body, string attachmentPath = null, LogLevel logLevel = LogLevel.Info) {
            var attachmentPaths = attachmentPath != null ? new List<string> { attachmentPath } : new List<string>();
            Report(subject, body, attachmentPaths, logLevel: logLevel);
        }

        #endregion

        public static void Report(
            string subject, string body, IEnumerable<string> attachmentPaths, LogLevel logLevel = LogLevel.Info) {

            var startTime = DateTimeOffset.Now;
            var logLabel = $"Report #{++_logCounter}";
            LogService.Log($"{logLabel}: {subject}", logLevel);

            try {
                var targetDirectory = PathHelper.Combine(Kit.DiagnisticsCurrentDirectory, _reportsDirectory);

                foreach (var client in Clients)
                    client.PushToReport(subject, body, attachmentPaths, targetDirectory);

                LogService.Log($"{logLabel} completed at {TimeHelper.FormattedLatency(startTime)}");
            }
            catch (Exception exception) {
                if (!exception.IsCanceled()) Debug.Fail(exception.ToString());
                LogService.Log($"{logLabel} failed at {TimeHelper.FormattedLatency(startTime)}");
                ExceptionHandler.Register(exception);
                throw;
            }
        }
    }
}
