using Maple.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core
{
    [System.Diagnostics.DebuggerStepThrough]
    public static class Check
    {
        /// <summary>
        /// 对象不为Null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static T NotNull<T>(T value, string parameterName)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);

            return value;
        }
        /// <summary>
        /// 字符串Null或空字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static string NotNullOrEmpty(string value, string parameterName)
        {
            if (value.IsNullOrEmpty())
                throw new ArgumentException($"{parameterName} can not be null or empty!", parameterName);

            return value;
        }
        /// <summary>
        /// 字符串不为Null、空字符串或者包含空格的空字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static string NotNullOrWhiteSpace(string value, string parameterName)
        {
            if (value.IsNullOrWhiteSpace())
                throw new ArgumentException($"{parameterName} can not be null, empty or white space!", parameterName);

            return value;
        }
        /// <summary>
        /// 判断集合不为Null或无元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static ICollection<T> NotNullOrEmpty<T>(ICollection<T> value, string parameterName)
        {
            if (value.IsNullOrEmpty())
                throw new ArgumentException(parameterName + " can not be null or empty!", parameterName);

            return value;
        }
    }
}
