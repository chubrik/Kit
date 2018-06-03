using System;

namespace Kit.Tests
{
    internal class TestUtils
    {
        public static string GetDiagnosticsDir(string testName) =>
            $"$tests/{testName}/$diagnostics/" + DateTimeOffset.Now.ToString("dd.MM.yyyy HH.mm.ss");
    }
}
