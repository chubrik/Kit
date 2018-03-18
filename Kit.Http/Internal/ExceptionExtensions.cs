using System;
using System.ComponentModel;

namespace Kit.Http {
    internal static class ExceptionExtensions {

        /// <summary>
        /// WinHttpException: The connection with the server was terminated abnormally
        /// </summary>
        public static bool Has12030(this Exception exception) =>
            (exception.SingleMostInnerExceptionOrNull() as Win32Exception)?.NativeErrorCode == 12030;
    }
}
