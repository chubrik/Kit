using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace Kit.Mail {
    public class MailClient : IReportClient {

        private static MailClient instance;
        public static MailClient Instance => instance ?? (instance = new MailClient());
        private MailClient() { }

        private static bool isEnable = true;
        private static string host;
        private static int port;
        private static NetworkCredential credentials;
        private static string from;
        private static string to;

        public static void Setup(
            bool? isEnable = null,
            string host = null,
            int? port = null,
            string userName = null,
            string password = null,
            string from = null,
            string to = null) {

            if (isEnable != null)
                MailClient.isEnable = (bool)isEnable;

            if (host != null)
                MailClient.host = host;

            if (port != null)
                MailClient.port = (int)port;

            if (userName != null && password != null)
                credentials = new NetworkCredential(userName, password);

            if (from != null)
                MailClient.from = from;

            if (to != null)
                MailClient.to = to;

            ReportService.Clients.Add(Instance);
        }

        #region IReportClient

        public async void PushToReport(string subject, string body, string targetDirectory) =>
            await SendAsync(subject, body, CancellationToken.None);

        #endregion

        public static async Task SendAsync(
            string subject, string body, CancellationToken cancellationToken, List<string> imagePaths = null) {

            if (!isEnable)
                return;

            LogService.Log($"Send email \"{subject}\"");
            var message = new MailMessage(from, to, subject, body);

            if (imagePaths != null)
                foreach (var imagePath in imagePaths) {

                    var attachment = new Attachment(imagePath, "image/jpg") {
                        ContentId = new Guid().ToString(),
                        ContentDisposition = {
                            Inline = true,
                            DispositionType = DispositionTypeNames.Inline
                        },
                        ContentType = {
                            Name = PathHelper.FileName(imagePath)
                        }
                    };

                    message.Attachments.Add(attachment);
                }

            try {
                using (var smtp = new SmtpClient(host, port)) {
                    smtp.EnableSsl = true;
                    smtp.Credentials = credentials;
                    await smtp.SendMailAsync(message); //todo cancellationToken
                }
            }
            catch (Exception exception) {
                Debug.Fail(exception.ToString());
                ExceptionHandler.Register(exception);
                throw;
            }

            foreach (var attachment in message.Attachments)
                attachment.Dispose();

            //LogService.Log($"End sending email \"{subject}\"");
        }
    }
}
