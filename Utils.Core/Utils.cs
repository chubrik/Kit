namespace Utils {
    public class Utils {

        public static string BaseDirectory { get; private set; } = string.Empty;

        public static void Setup(string baseDirectory = null) {

            if (baseDirectory != null)
                BaseDirectory = baseDirectory;
        }
    }
}
