using System;

namespace Maple.Core.Timing
{
    /// <summary>
    /// 本地时间 .
    /// </summary>
    public class LocalClockProvider : IClockProvider
    {
        public DateTime Now => DateTime.Now;

        public DateTimeKind Kind => DateTimeKind.Local;

        public bool SupportsMultipleTimezone => false;

        public DateTime Normalize(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                return DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
            }

            if (dateTime.Kind == DateTimeKind.Utc)
            {
                return dateTime.ToLocalTime();
            }

            return dateTime;
        }

        internal LocalClockProvider()
        {

        }
    }
}