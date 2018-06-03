using Kit.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace Kit.Tests
{
    [TestClass]
    public class MailTests : TestsBase
    {
        [TestMethod]
        public void Succeeded()
        {
            var testName = $"{GetType().Name}.{nameof(Succeeded)}";

            TestExecute(testName, () =>
            {
                MailSetup();
                MailClient.Send("Kit.Test", $"{GetType().Name}.{nameof(Succeeded)}");
                MailReset();
            });
        }

        [TestMethod]
        public void Failed()
        {
            var testName = $"{GetType().Name}.{nameof(Failed)}";
            Exception testException = null;

            TestExecute(testName, () =>
            {
                MailSetup();
                MailClient.Setup(userName: "wrong", password: "wrong");

                try
                {
                    MailClient.Send("Kit.Test", $"{GetType().Name}.{nameof(Failed)}");
                }
                catch (Exception exception)
                {
                    testException = exception;
                }

                MailReset();
            });

            Assert.IsTrue(testException is AggregateException);
            var innerExceptions = (testException as AggregateException).InnerExceptions;
            Assert.IsTrue(innerExceptions.Count == 1);
            Assert.IsTrue(innerExceptions[0] is SmtpException);
            Assert.IsTrue(innerExceptions[0].Message == "Failure sending mail.");
        }

        [TestMethod]
        public void Canceled()
        {
            var testName = $"{GetType().Name}.{nameof(Canceled)}";
            Exception testException = null;

            TestExecute(testName, async () =>
            {
                MailSetup();

                try
                {
                    var cts = new CancellationTokenSource();
                    var mailTask = MailClient.SendAsync("Kit.Test", $"{GetType().Name}.{nameof(Failed)}", cts.Token);
                    await Task.Delay(100);
                    cts.Cancel();
                    await mailTask;
                }
                catch (Exception exception)
                {
                    testException = exception;
                }

                MailReset();
            });

            Assert.IsTrue(testException is TaskCanceledException);
            Assert.IsTrue(testException.Message == "A task was canceled.");
        }

        #region Utils

        private void MailSetup()
        {
            var mailCredentials = File.ReadAllLines("../../../../../mail-credentials.txt");

            MailClient.Setup(
                host: mailCredentials[0],
                port: int.Parse(mailCredentials[1]),
                userName: mailCredentials[2],
                password: mailCredentials[3],
                from: mailCredentials[4],
                to: mailCredentials[5]
            );
        }

        private void MailReset()
        {
            MailClient.Setup(
                host: default(string),
                port: default(int),
                userName: default(string),
                password: default(string),
                from: default(string),
                to: default(string)
            );

            ReportService.Clients.Remove(MailClient.Instance);
        }

        #endregion
    }
}
