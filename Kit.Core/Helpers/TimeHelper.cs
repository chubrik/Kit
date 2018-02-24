using System;
using System.Globalization;

namespace Kit {
    public class TimeHelper {

        public static string FormattedLatency(DateTimeOffset startTime) {
            var milliseconds = Math.Round((DateTimeOffset.Now - startTime).TotalMilliseconds);

            return milliseconds < 1000
                ? $"{milliseconds} ms"
                : $"{(milliseconds / 1000).ToString(CultureInfo.InvariantCulture)} s";
        }
    }
}
