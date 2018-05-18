using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Maple.Core
{
    /// <summary>
    /// Type的扩展方法
    /// </summary>
    public static class TypeExtensions
    {
        public static T[] GetAttributes<T>(this Type type, bool inherit) where T : Attribute
        {
            var os = type.GetCustomAttributes(typeof(T), inherit);
            return (T[])os;
        }

        public static T GetAttribute<T>(this Type type, bool inherit) where T : Attribute
        {
            var ts = GetAttributes<T>(type, inherit);
            if (ts != null && ts.Length > 0)
                return ts[0];
            return null;
        }

        /// <summary>
        /// 判断是否为基元类型
        /// </summary>
        /// <param name="info"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static bool IsPrimitiveExtended(this Type type) 
        {
            if (type.IsPrimitive)
                return true;

            return type == typeof(string) ||
                   type == typeof(decimal) ||
                   type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset) ||
                   type == typeof(TimeSpan) ||
                   type == typeof(Guid);
        }

        /// <summary>
        /// 判断是否为基元类型或Nullable的基元类型
        /// </summary>
        /// <param name="info"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static bool IsPrimitiveExtendedIncludingNullable(this Type type)
        {
            if (type.IsPrimitiveExtended())
                return true;

            if (type.IsIncludingNullable())
                return type.GenericTypeArguments[0].IsPrimitiveExtended();

            return false;
        }
        /// <summary>
        /// 判断是否为Nullable
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsIncludingNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
