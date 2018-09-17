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
        public void Execute()
        {
            var testName = $"{GetType().Name}.{nameof(Execute)}";
            TestInitialize(testName);
            var formattedTime = DateTimeOffset.Now.ToString("dd.MM.yyyy HH.mm.ss");
            var diagnosticsDir = $"{ProjectNativePath}/$work/$diagnostics/{formattedTime}";
            var reportsDir = $"{diagnosticsDir}/reports";

            Kit.Setup(useFileDiagnostics: true);

            Kit.Execute(() =>
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

        [TestMethod]
        public void PathCombine()
        {
            Assert.IsTrue(PathHelper.Combine("") == "");
            Assert.IsTrue(PathHelper.Combine("/") == "/");
            Assert.IsTrue(PathHelper.Combine("//") == "/");
            Assert.IsTrue(PathHelper.Combine("/./") == "/");
            Assert.IsTrue(PathHelper.Combine(".") == "");
            Assert.IsTrue(PathHelper.Combine("..") == "..");
            Assert.IsTrue(PathHelper.Combine("one") == "one");
            Assert.IsTrue(PathHelper.Combine("one/") == "one");
            Assert.IsTrue(PathHelper.Combine("one//") == "one");
            Assert.IsTrue(PathHelper.Combine(@"one\") == "one");
            Assert.IsTrue(PathHelper.Combine(@"one\\") == "one");
            Assert.IsTrue(PathHelper.Combine(@"one\/\") == "one");
            Assert.IsTrue(PathHelper.Combine("one/.") == "one");
            Assert.IsTrue(PathHelper.Combine("one/..") == "");
            Assert.IsTrue(PathHelper.Combine("one/two") == "one/two");
            Assert.IsTrue(PathHelper.Combine("one//two") == "one/two");
            Assert.IsTrue(PathHelper.Combine("one/./two") == "one/two");
            Assert.IsTrue(PathHelper.Combine("one/../two") == "two");
            Assert.IsTrue(PathHelper.Combine("/one") == "/one");
            Assert.IsTrue(PathHelper.Combine("/one/") == "/one");
            Assert.IsTrue(PathHelper.Combine("/one/.") == "/one");
            Assert.IsTrue(PathHelper.Combine("/one/..") == "/");
            Assert.IsTrue(PathHelper.Combine("./one") == "one");
            Assert.IsTrue(PathHelper.Combine("../one") == "../one");
            Assert.IsTrue(PathHelper.Combine("../one/..") == "..");
            Assert.IsTrue(PathHelper.Combine("one", "two") == "one/two");
            Assert.IsTrue(PathHelper.Combine(@"/one\/\", "two//") == "/one/two");
            Assert.IsTrue(PathHelper.Combine("one/", @"\two") == "/two");
            Assert.IsTrue(PathHelper.Combine("./one/.././two/./") == "two");
            Assert.IsTrue(PathHelper.Combine("../one/../../two/..") == "../..");

            try
            {
                PathHelper.Combine("/..");
                Assert.Fail();
            }
            catch (InvalidOperationException) { }

            try
            {
                PathHelper.Combine("/one", "../../two");
                Assert.Fail();
            }
            catch (InvalidOperationException) { }
        }
    }
}
