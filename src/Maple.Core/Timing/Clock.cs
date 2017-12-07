using System;

namespace Maple.Core.Timing
{
    /// <summary>
    /// ����ִ��һЩ���õ�����ʱ�����
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
        /// ��ȡ��ǰʱ��.
        /// </summary>
        public static DateTime Now => Provider.Now;

        public static DateTimeKind Kind => Provider.Kind;

        /// <summary>
        /// �Ƿ�֧�ֶ�ʱ��.
        /// </summary>
        public static bool SupportsMultipleTimezone => Provider.SupportsMultipleTimezone;

        /// <summary>
        /// ʱ��ת��
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime Normalize(DateTime dateTime)
        {
            return Provider.Normalize(dateTime);
        }
    }
}