using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Kit.Mail
{
    public class MailClient : IReportClient
    {
        private static MailClient _instance;
        public static MailClient Instance => _instance ?? (_instance = new MailClient());
        private MailClient() { }

        private static bool _isEnable = true;
        private static string _host;
        private static int _port;
        private static NetworkCredential _credentials;
        private static string _from;
        private static string _to;
        private static int _logCounter = 0;

        #region Setup

        public static void Setup(
            bool? isEnable = null,
            string host = null,
            int? port = null,
            string userName = null,
            string password = null,
            string from = null,
            string to = null)
        {
            if (isEnable != null)
                _isEnable = (bool)isEnable;

            if (host != null)
                _host = host;

            if (port != null)
                _port = (int)port;

            if (userName != null && password != null)
                _credentials = new NetworkCredential(userName, password);

            if (from != null)
                _from = from;

            if (to != null)
                _to = to;

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
        public static async Task SendAsync(string subject, string body, IEnumerable<string> attachmentPaths)
        {
            if (!_isEnable)
                return;

            var startTime = DateTimeOffset.Now;
            var logLabel = $"Mail send #{++_logCounter}";
            LogService.Log($"{logLabel}: {subject}");
            var message = new MailMessage(_from, _to, subject, body);

            if (attachmentPaths != null)
                foreach (var attachmentPath in attachmentPaths)
                {
                    var attachment = new Attachment(FileClient.FullPath(attachmentPath), "application/octet-stream")
                    {
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

            try
            {
                using (var smtp = new SmtpClient(_host, _port))
                {
                    smtp.EnableSsl = true;
                    smtp.Credentials = _credentials;
                    await smtp.SendMailAsync(message); //todo cancellationToken
                }
            }
            catch (Exception exception)
            {
                if (exception.IsCanceled())
                    LogService.Log($"{logLabel} canceled at {TimeHelper.FormattedLatency(startTime)}");
                else
                {
                    Debug.Fail(exception.ToString());
                    LogService.LogError($"{logLabel} failed at {TimeHelper.FormattedLatency(startTime)}");
                }

                ExceptionHandler.Register(exception);
                throw;
            }

            foreach (var attachment in message.Attachments)
                attachment.Dispose();

            LogService.Log($"{logLabel} completed at {TimeHelper.FormattedLatency(startTime)}");
        }

        #endregion
    }
}
