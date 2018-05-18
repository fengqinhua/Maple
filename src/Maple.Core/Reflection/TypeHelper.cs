using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Maple.Core.Reflection
{
    /// <summary>
    /// 一些简单的在内部使用的类型检查方法
    /// </summary>
    internal static class TypeHelper
    {
        public static bool IsFunc(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var type = obj.GetType();
            if (!type.GetTypeInfo().IsGenericType)
            {
                return false;
            }

            return type.GetGenericTypeDefinition() == typeof(Func<>);
        }

        public static bool IsFunc<TReturn>(object obj)
        {
            return obj != null && obj.GetType() == typeof(Func<TReturn>);
        }
    }
}
