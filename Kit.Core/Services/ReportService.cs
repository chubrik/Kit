using System.Collections.Generic;

namespace Kit {
    public class ReportService {

        private ReportService() { }

        private const string reportsDirName = "reports";

        public static readonly List<IReportClient> Clients = new List<IReportClient> {
            FileClient.Instance
        };

        public static void Report(string subject, string body, LogLevel level = LogLevel.Info) {
            LogService.Log($"Report: {subject}", level);

            foreach (var client in Clients)
                client.PushToReport(
                    subject, body, PathHelper.Combine(Kit.DiagnisticsCurrentDirectory, reportsDirName));
        }

        public static void ReportSuccess(string subject, string body) =>
            Report(subject, body, LogLevel.Success);

        public static void ReportWarning(string subject, string body) =>
            Report(subject, body, LogLevel.Warning);

        public static void ReportError(string subject, string body) =>
            Report(subject, body, LogLevel.Error);
    }
}
