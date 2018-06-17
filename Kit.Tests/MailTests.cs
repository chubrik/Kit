using Kit.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace Kit.Tests
{
    [TestClass]
    public class MailTests : TestsBase
    {
        private static readonly string[] Credentials = File.ReadAllLines("../../../../../mail-credentials.txt");

        [TestMethod]
        public async Task Succeeded()
        {
            TestInitialize($"{GetType().Name}.{nameof(Succeeded)}");
            Setup();
            await MailClient.SendAsync("Kit.Test", $"{GetType().Name}.{nameof(Succeeded)}");
        }

        [TestMethod]
        public async Task Failed()
        {
            TestInitialize($"{GetType().Name}.{nameof(Failed)}");

            MailClient.Setup(
                host: "wrong-host",
                port: 0,
                userName: "wrong-user",
                password: "wrong-password",
                from: Credentials[4],
                to: Credentials[5]
            );

            try
            {
                await MailClient.SendAsync("Kit.Test", $"{GetType().Name}.{nameof(Failed)}");
                Assert.Fail();
            }
            catch (SmtpException exception)
            {
                Assert.IsTrue(exception.Message == "Failure sending mail.");
            }
        }

        [TestMethod]
        public async Task Canceled()
        {
            TestInitialize($"{GetType().Name}.{nameof(Canceled)}");
            Setup();

            try
            {
                var cts = new CancellationTokenSource();
                var mailTask = MailClient.SendAsync("Kit.Test", $"{GetType().Name}.{nameof(Failed)}", cts.Token);
                await Task.Delay(100);
                cts.Cancel();
                await mailTask;
                Assert.Fail();
            }
            catch (TaskCanceledException)
            {
            }
        }

        #region Utils

        private void Setup() =>
            MailClient.Setup(
                host: Credentials[0],
                port: int.Parse(Credentials[1]),
                userName: Credentials[2],
                password: Credentials[3],
                from: Credentials[4],
                to: Credentials[5]
            );

        #endregion
    }
}
