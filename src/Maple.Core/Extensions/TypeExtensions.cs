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
        /// 类型是否为值类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsValueType(this Type type)
        {
#if NETSTANDARD1_3 || NETCOREAPP1_0
            return type.GetTypeInfo().IsValueType;
#else
            return type.IsValueType;
#endif
        }
        public static bool IsEnum(this Type type) =>
#if NETSTANDARD1_3 || NETCOREAPP1_0
            type.GetTypeInfo().IsEnum;
#else
            type.IsEnum;
#endif
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

        /// <summary>
        ///     判断指定类型是否为数值类型
        /// </summary>
        /// <param name="type">要检查的类型</param>
        /// <returns>是否是数值类型</returns>
        public static bool IsNumeric(this Type type)
        {
            return type == typeof(Byte)
                || type == typeof(Int16)
                || type == typeof(Int32)
                || type == typeof(Int64)
                || type == typeof(SByte)
                || type == typeof(UInt16)
                || type == typeof(UInt32)
                || type == typeof(UInt64)
                || type == typeof(Decimal)
                || type == typeof(Double)
                || type == typeof(Single);
        }


#if NETSTANDARD1_3 || NETCOREAPP1_0
        public static TypeCode GetTypeCode(Type type)
        {
            if (type == null) return TypeCode.Empty;
            if (typeCodeLookup.TryGetValue(type, out TypeCode result)) return result;

            if (type.IsEnum())
            {
                type = Enum.GetUnderlyingType(type);
                if (typeCodeLookup.TryGetValue(type, out result)) return result;
            }
            return TypeCode.Object;
        }
#else
        public static TypeCode GetTypeCode(Type type) => Type.GetTypeCode(type);
#endif

        /// <summary>
        /// 深度查找基类是否派生自某个泛型类
        /// </summary>
        /// <param name="typeToCheck"></param>
        /// <param name="genericType"></param>
        /// <returns></returns>
        public static bool IsTypeDerivedFromGenericType(this Type typeToCheck, Type genericType)
        {
            if (typeToCheck == typeof(object))
            {
                return false;
            }
            else if (typeToCheck == null)
            {
                return false;
            }
            else if (typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }
            else
            {
                return IsTypeDerivedFromGenericType(typeToCheck.BaseType, genericType);
            }
        }
    }
}
