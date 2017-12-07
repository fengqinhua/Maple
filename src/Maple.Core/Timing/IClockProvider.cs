using System;

namespace Maple.Core.Timing
{
    /// <summary>
    /// 定义执行一些常用日期时间操作的接口.
    /// </summary>
    public interface IClockProvider
    {
        /// <summary>
        /// 获取当前时间.
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// 获取时间类型，本地时间 OR UTC时间.
        /// </summary>
        DateTimeKind Kind { get; }

        /// <summary>
        /// 是否支持多个时区.
        /// </summary>
        bool SupportsMultipleTimezone { get; }

        /// <summary>
        /// 时间转换
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        DateTime Normalize(DateTime dateTime);
    }
}