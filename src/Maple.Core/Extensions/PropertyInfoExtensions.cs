using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Maple.Core
{
    /// <summary>
    /// Object的扩展方法
    /// </summary>
    public static class PropertyInfoExtensions
    {
        public static bool HasAttribute<T>(this PropertyInfo info, bool inherit) where T : Attribute
        {
            return GetAttribute<T>(info, inherit) != null;
        }

        public static T GetAttribute<T>(this PropertyInfo info, bool inherit) where T : Attribute
        {
            var ts = GetAttributes<T>(info, inherit);
            if (ts != null && ts.Length > 0)
                return ts[0];
            return null;
        }

        public static T[] GetAttributes<T>(this PropertyInfo info, bool inherit) where T : Attribute
        {
            var os = info.GetCustomAttributes(typeof(T), inherit);
            return (T[])os;
        }

        /// <summary>
        /// PropertyInfo对应的类型是否为Nullable
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool IsIncludingNullable(this PropertyInfo info)
        {
            return info.PropertyType.IsIncludingNullable();
        }

        /// <summary>
        /// 获取PropertyInfo对应的类型是否为基元类型或Nullable的基元类型
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool IsPrimitiveExtendedIncludingNullableOrEnum(this PropertyInfo info) 
        {
            return info.PropertyType.IsEnum || info.PropertyType.IsPrimitiveExtendedIncludingNullable();
        }

    }
}
