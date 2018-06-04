using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
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

        public void PushToReport(
            string subject, string body, IEnumerable<string> attachmentPaths, string targetDirectory) =>
            Send(subject, body, attachmentPaths); //todo queue

        #endregion

        #region Send

        #region Extensions

        public static void Send(string subject, string body) =>
            SendAsync(subject, body, attachmentPaths: null, cancellationToken: null).Wait();

        public static void Send(string subject, string body, string attachmentPath) =>
            SendAsync(subject, body, new List<string> { attachmentPath }, cancellationToken: null).Wait();

        public static void Send(string subject, string body, IEnumerable<string> attachmentPaths) =>
            SendAsync(subject, body, attachmentPaths, cancellationToken: null).Wait();

        public static Task SendAsync(
            string subject, string body, CancellationToken? cancellationToken = null) =>
            SendAsync(subject, body, attachmentPaths: null, cancellationToken: cancellationToken);

        public static Task SendAsync(
            string subject, string body, string attachmentPath, CancellationToken? cancellationToken = null) =>
            SendAsync(subject, body, new List<string> { attachmentPath }, cancellationToken: cancellationToken);

        #endregion

        public static async Task SendAsync(
            string subject, string body, IEnumerable<string> attachmentPaths, CancellationToken? cancellationToken = null)
        {
            if (!_isEnable)
                return;

            var startTime = DateTimeOffset.Now;
            var logLabel = $"Mail send #{++_logCounter}";
            LogService.Log($"{logLabel}: {subject}");

            using (var message = new MailMessage(_from, _to, subject, body))
            {
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
                    using (var client = new SmtpClient(_host, _port))
                    {
                        client.EnableSsl = true;
                        client.Credentials = _credentials;
                        await client.SendMailAsync(message, cancellationToken ?? Kit.CancellationToken);
                    }
                }
                catch (Exception exception)
                {
                    Debug.Assert(exception.IsAllowed());
                    throw;
                }
            }

            LogService.Log($"{logLabel} completed at {TimeHelper.FormattedLatency(startTime)}");
        }

        #endregion
    }
}
