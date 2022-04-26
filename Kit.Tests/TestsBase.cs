using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Chubrik.Kit.Tests
{
    public abstract class TestsBase
    {
        [TestInitialize]
        public void BaseInitialize()
        {
            Kit.Setup(isTest: true, useFileDiagnostics: true);
            ConsoleClient.Setup(minLevel: LogLevel.Log);
        }

        public static void TestInitialize(string testName)
        {
            var workingDir = "$work/" + testName;
            Kit.Setup(workingDirectory: workingDir, diagnosticsDirectory: "$diagnostics");
            ConsoleClient.Setup(minLevel: LogLevel.Log);

            var nativeWorkingDir = "../../../" + workingDir;

            if (Directory.Exists(nativeWorkingDir))
            {
                foreach (var file in Directory.GetFiles(nativeWorkingDir))
                    File.Delete(file);

                Assert.IsTrue(Directory.GetFiles(nativeWorkingDir).Length == 0);
            }
        }
    }
}
