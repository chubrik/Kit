using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Kit.Tests
{
    public abstract class TestsBase
    {
        public void TestExecute(string testName, Action @delegate) =>
            TestExecute(testName, cancellationToken =>
            {
                @delegate();
                return Task.CompletedTask;
            });

        public void TestExecute(string testName, Func<Task> delegateAsync) =>
            TestExecute(testName, cancellationToken => delegateAsync());

        public void TestExecute(string testName, Func<CancellationToken, Task> delegateAsync)
        {
            var baseDirectory = $"$tests/{testName}";

            if (Directory.Exists(baseDirectory))
                Directory.Delete(baseDirectory, recursive: true);

            Assert.IsFalse(Directory.Exists(baseDirectory));
            Kit.Setup(baseDirectory: baseDirectory, isTest: true);
            Kit.Execute(delegateAsync);
        }
    }
}
