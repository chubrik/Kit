using System.Collections.Generic;

namespace Kit
{
    public interface IReportClient
    {
        void PushToReport(string subject, string body, IEnumerable<string> attachmentPaths, string targetDirectory);
    }
}
