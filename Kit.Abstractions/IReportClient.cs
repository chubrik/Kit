namespace Kit {
    public interface IReportClient {

        void PushToReport(string subject, string body, string targetDirectory);
    }
}
