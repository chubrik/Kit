using Chubrik.Kit.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace Chubrik.Kit.Tests
{
    [TestClass]
    public class MailTests : TestsBase
    {
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
            Setup();

            MailClient.Setup(
                host: "wrong-host",
                port: 0,
                userName: "wrong-user",
                password: "wrong-password"
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
            catch (TaskCanceledException) { }
        }

        #region Utils

        private void Setup()
        {
            var credentials = FileClient.ReadLines("../../../../mail-credentials.txt");

            MailClient.Setup(
                host: credentials[0],
                port: int.Parse(credentials[1]),
                userName: credentials[2],
                password: credentials[3],
                from: credentials[4],
                to: credentials[5]
            );
        }

        #endregion
    }
}
