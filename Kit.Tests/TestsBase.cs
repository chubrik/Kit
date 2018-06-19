﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Kit.Tests
{
    public abstract class TestsBase
    {
        protected const string ProjectNativePath = "../../..";

        [TestInitialize]
        public void BaseInitialize()
        {
            Kit.Setup(test: true, baseDirectory: ProjectNativePath);
            ConsoleClient.Setup(minLevel: LogLevel.Log);
        }

        public void TestInitialize(string testName)
        {
            var workingDir = "$tests/" + testName;
            Kit.Setup(workingDirectory: workingDir, diagnosticsDirectory: workingDir);
            ConsoleClient.Setup(minLevel: LogLevel.Log);

            var nativeWorkingDir = ProjectNativePath + "/" + workingDir;

            if (Directory.Exists(nativeWorkingDir))
            {
                foreach (var file in Directory.GetFiles(nativeWorkingDir))
                    File.Delete(file);

                Assert.IsTrue(Directory.GetFiles(nativeWorkingDir).Length == 0);
            }
        }
    }
}
