using System;

namespace Utils {
    public class Utils {

        public static string BaseDirectory { get; private set; } = string.Empty;
        private static bool isInitialized;

        public static void Initialize(string baseDirectory) {
            if (isInitialized)
                throw new InvalidOperationException();

            if (baseDirectory != null)
                BaseDirectory = baseDirectory;

            isInitialized = true;
        }
    }
}
