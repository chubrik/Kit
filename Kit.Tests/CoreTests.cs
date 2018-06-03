using Kit.Azure;
using Kit.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kit.Tests
{
    [TestClass]
    public class CoreTests : TestsBase
    {
        [TestMethod]
        public void BaseRun()
        {
            var testName = $"{GetType().Name}.{nameof(BaseRun)}";
            var diagnosticsDir = TestUtils.GetDiagnosticsDir(testName);
            var reportsDir = $"{diagnosticsDir}/reports";

            TestExecute(testName, () =>
            {
                LogService.LogInfo("Hello World!");
            });

            Assert.IsTrue(Directory.Exists(diagnosticsDir));
            var diagnosticsFileNames = Directory.GetFiles(diagnosticsDir).Select(Path.GetFileName).ToList();
            Assert.IsTrue(diagnosticsFileNames.Count == 2);
            Assert.IsTrue(diagnosticsFileNames[0] == "$log.txt");
            Assert.IsTrue(Regex.IsMatch(diagnosticsFileNames[1], @"^001 Test exception \(Kit\.cs_\d+\)\.txt$"));
            Assert.IsTrue(Directory.Exists(reportsDir));
            var reportFileNames = Directory.GetFiles(reportsDir).Select(Path.GetFileName).ToList();
            Assert.IsTrue(reportFileNames.Count == 1);
            Assert.IsTrue(reportFileNames[0] == "001 Test exception.txt");
        }

        //[TestMethod]
        public void Azure()
        {
            using (var http = new HttpClient())
            {
                var html = http.GetText("https://www.google.com/");
            }

            using (var stream = FileClient.OpenRead("../Kit.Tests.runtimeconfig.json"))
                AzureBlobClient.Write("file.ext", stream);

            using (var stream = FileClient.OpenWrite("test.json"))
                AzureBlobClient.Read("file.ext", stream);

            ReportService.ReportSuccess("My report", "My report body", "../Kit.Core.dll");
            throw new Exception("My exception");
        }
    }
}
