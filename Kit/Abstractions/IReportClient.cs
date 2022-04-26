using System.Collections.Generic;

namespace Chubrik.Kit
{
    public interface IReportClient
    {
        void PushToReport(string subject, string body, IEnumerable<string> attachmentPaths, string targetDirectory);
    }
}
