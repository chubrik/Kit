﻿using System;

namespace Kit
{
    public static class TimeHelper
    {
        public static string FormattedLatency(DateTimeOffset startTime) =>
            FormattedLatency(DateTimeOffset.Now - startTime);

        public static string FormattedLatency(TimeSpan timeSpan) =>
            timeSpan.Hours > 0
                ? $@"{timeSpan:h\:mm\:ss}"
                : timeSpan.Minutes > 0
                    ? $@"{timeSpan:m\:ss}"
                    : timeSpan.Seconds > 0
                        ? $@"{timeSpan:s\.fff} s"
                        : timeSpan.Milliseconds + " ms";
    }
}
