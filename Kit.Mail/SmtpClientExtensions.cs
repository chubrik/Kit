using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace Kit.Mail
{
    public static class SmtpClientExtensions
    {
        public static async Task SendMailAsync(
            this SmtpClient client, string from, string recipients, string subject, string body,
            CancellationToken cancellationToken)
        {
            using (var message = new MailMessage(from, recipients, subject, body))
                await client.SendMailAsync(message, cancellationToken);
        }

        public static async Task SendMailAsync(
            this SmtpClient client, MailMessage message, CancellationToken cancellationToken)
        {
            var ctsWrapper = new[] { cancellationToken.GetNestedSource() };
            var mailSent = false;
            Exception exception = null;

            void onCompleted(object sender, AsyncCompletedEventArgs e)
            {
                if (e.Cancelled)
                    exception = new TaskCanceledException();

                if (e.Error != null)
                    exception = e.Error;

                mailSent = true;
                ctsWrapper[0].Cancel();
            };

            client.SendCompleted += onCompleted;
            client.SendAsync(message, new object());

            try
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, ctsWrapper[0].Token);
            }
            catch (OperationCanceledException)
            {
                // no throw for canceled delay
            }

            if (!mailSent)
            {
                ctsWrapper[0] = new CancellationTokenSource();
                client.SendAsyncCancel();

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), ctsWrapper[0].Token);
                    Debug.Fail(string.Empty);
                    throw new InvalidOperationException();
                }
                catch (OperationCanceledException)
                {
                    // no throw for canceled delay
                }
            }

            client.SendCompleted -= onCompleted;

            if (exception != null)
                throw exception;
        }
    }
}
