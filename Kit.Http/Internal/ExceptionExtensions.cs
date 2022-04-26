using System;
using System.ComponentModel;

namespace Chubrik.Kit.Http
{
    internal static class ExceptionExtensions
    {
        /// <summary>
        /// WinHttpException: The connection with the server was terminated abnormally
        /// </summary>
        public static bool Has12030(this Exception exception) =>
            (exception.FirstInnerestException() as Win32Exception)?.NativeErrorCode == 12030;

        public static bool IsHttpAllowed(this Exception exception) =>
            exception.IsAllowed() || exception.Has12030();
    }
}
