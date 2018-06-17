using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Kit.Tests
{
    public abstract class TestsBase
    {
        public void TestInitialize(string testName)
        {
            var baseDirectory = $"$tests/{testName}";

            if (Directory.Exists(baseDirectory))
                Directory.Delete(baseDirectory, recursive: true);

            Assert.IsFalse(Directory.Exists(baseDirectory));

            Kit.Setup(test: true, baseDirectory: baseDirectory);
            ConsoleClient.Setup(minLevel: LogLevel.Log);
        }
    }
}
