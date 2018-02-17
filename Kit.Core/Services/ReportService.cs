using System.Collections.Generic;

namespace Kit {
    public class ReportService {

        private ReportService() { }

        public static readonly List<IReportClient> ReportClients = new List<IReportClient> {
            FileClient.Instance
        };

        private static string targetDirectory = "$reports";

        public static void Setup(string targetDirectory = null) {

            if (targetDirectory != null)
                ReportService.targetDirectory = targetDirectory;
        }

        public static void Report(string subject, string body, LogLevel level = LogLevel.Info) {
            LogService.Log($"Report: {subject}", level);

            foreach (var reportClient in ReportClients)
                reportClient.PushToReport(subject, body, targetDirectory);
        }
    }
}
