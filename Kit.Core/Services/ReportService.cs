using System;
using System.Collections.Generic;

namespace Kit {
    public class ReportService {

        private ReportService() { }

        private const string reportsDirectory = "reports";

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
            LogService.Log($"Report: {subject}", logLevel);
            LogService.Log($"Report started");

            foreach (var client in Clients)
                client.PushToReport(
                    subject, body, attachmentPaths, PathHelper.Combine(Kit.DiagnisticsCurrentDirectory, reportsDirectory));

            LogService.Log($"Report completed at {TimeHelper.FormattedLatency(startTime)}");
        }
    }
}
