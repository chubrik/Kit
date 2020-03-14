using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Kit
{
    public class FileKit : ILogClient, IReportClient
    {
        public static readonly string BaseDirectory =
            Environment.GetEnvironmentVariables().Contains("VisualStudioDir")
                ? PathHelper.Combine(Environment.CurrentDirectory, "../../..")
                : Regex.Replace(Environment.CurrentDirectory, @"[\\/]+", "/");

        private static readonly string _formattedStartTime = DateTimeOffset.Now.ToString("dd.MM.yyyy HH.mm.ss");

        public string WorkingDirectory { get; set; } = "$work";
        public string DiagnosticsDirectory { get; set; } = "$diagnostics";
        public string LogFileName { get; set; } = "$log.txt";
        public string ReportsDirectory { get; set; } = "$reports";
        //public string DiagnisticsWorkingDirectory => PathHelper.Combine(DiagnosticsBaseDirectory, _formattedStartTime);

        public void PushToLog(string message, LogLevel level = default)
        {
            throw new NotImplementedException();
        }

        public void PushToReport(string subject, string body, IEnumerable<string> attachmentPaths, string targetDirectory)
        {
            throw new NotImplementedException();
        }

        public string ReadAllText(string path) => Handle(nameof(ReadAllText) + ": " + path, () => File.ReadAllText(NativePath(path)));
        public string ReadAllText(string path, Encoding encoding) => Handle(nameof(ReadAllText) + ": " + path, () => File.ReadAllText(NativePath(path), encoding));

        private string NativePath(string path) => PathHelper.Combine(BaseDirectory, WorkingDirectory, path);
        private T Handle<T>(string message, Func<T> block) => ToolKit.Handle(nameof(FileKit) + "." + message, block);
    }
}
