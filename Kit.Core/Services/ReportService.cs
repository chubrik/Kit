using System.Collections.Generic;

namespace Kit {
    public class ReportService {

        private ReportService() { }

        public static readonly List<IReportClient> Clients = new List<IReportClient> {
            FileClient.Instance
        };

        private static string reportsDirectory = "$reports";

        public static void Setup(string reportsDirectory = null) {

            if (reportsDirectory != null)
                ReportService.reportsDirectory = reportsDirectory;
        }

        public static void Report(string subject, string body, LogLevel level = LogLevel.Info) {
            LogService.Log($"Report: {subject}", level);

            foreach (var client in Clients)
                client.PushToReport(subject, body, reportsDirectory);
        }

        public static void ReportSuccess(string subject, string body) =>
            Report(subject, body, LogLevel.Success);

        public static void ReportWarning(string subject, string body) =>
            Report(subject, body, LogLevel.Warning);

        public static void ReportError(string subject, string body) =>
            Report(subject, body, LogLevel.Error);
    }
}
