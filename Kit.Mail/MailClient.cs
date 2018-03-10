using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
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

        #region Setup

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

        #endregion

        #region IReportClient

        public void PushToReport(string subject, string body, IEnumerable<string> attachmentPaths, string targetDirectory) =>
            SendAsync(subject, body, attachmentPaths).Wait(); //todo queue

        #endregion

        #region Send

        #region Extensions

        public static void Send(string subject, string body, string attachmentPath = null) =>
            SendAsync(subject, body, attachmentPath).Wait();

        public static void Send(string subject, string body, IEnumerable<string> attachmentPaths) =>
            SendAsync(subject, body, attachmentPaths).Wait();

        public static Task SendAsync(string subject, string body, string attachmentPath = null) =>
            SendAsync(subject, body, attachmentPath == null ? new List<string>() : new List<string> { attachmentPath });

        #endregion

        //todo attachments
        public static async Task SendAsync(string subject, string body, IEnumerable<string> attachmentPaths) {

            if (!isEnable)
                return;

            var startTime = DateTimeOffset.Now;
            LogService.Log($"Mail send started: {subject}");
            var message = new MailMessage(from, to, subject, body);

            if (attachmentPaths != null)
                foreach (var attachmentPath in attachmentPaths) {

                    var attachment = new Attachment(FileClient.FullPath(attachmentPath), "application/octet-stream") {
                        ContentId = new Guid().ToString(),
                        ContentDisposition = {
                            Inline = true,
                            DispositionType = DispositionTypeNames.Inline
                        },
                        ContentType = {
                            Name = PathHelper.FileName(attachmentPath)
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

            LogService.Log($"Mail send completed at {TimeHelper.FormattedLatency(startTime)}");
        }

        #endregion
    }
}
