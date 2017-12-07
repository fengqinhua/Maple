using System;

namespace Maple.Core.Timing
{
    /// <summary>
    /// 用于执行一些常用的日期时间操做
    /// </summary>
    public static class Clock
    {
        /// <summary>
        /// 
        /// </summary>
        public static IClockProvider Provider
        {
            get { return _provider; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Can not set Clock.Provider to null!");
                }

                _provider = value;
            }
        }

        private static IClockProvider _provider;

        static Clock()
        {
            Provider = ClockProviders.Unspecified;
        }

        /// <summary>
        /// 获取当前时间.
        /// </summary>
        public static DateTime Now => Provider.Now;

        public static DateTimeKind Kind => Provider.Kind;

        /// <summary>
        /// 是否支持多时区.
        /// </summary>
        public static bool SupportsMultipleTimezone => Provider.SupportsMultipleTimezone;

        /// <summary>
        /// 时间转换
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime Normalize(DateTime dateTime)
        {
            return Provider.Normalize(dateTime);
        }
    }
}