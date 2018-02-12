using System;

namespace Utils {
    public class Utils {

        private static bool isInitialized;

        public static void Initialize(string workingDirectory = "workDir") {

            if (isInitialized)
                throw new InvalidOperationException();

            isInitialized = true;
        }
    }
}
