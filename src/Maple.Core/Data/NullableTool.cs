using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Collections.Specialized;

namespace Maple.Core.Data
{
    public class NullableTool
    {
        private static readonly HybridDictionary NullableTypesToDataType;
        private static readonly HybridDictionary NullableTypesToUnderlyingType;

        static NullableTool()
        {
            NullableTypesToDataType = new HybridDictionary();
            NullableTypesToDataType[typeof(Nullable<bool>)] = DbType.Boolean;
            NullableTypesToDataType[typeof(Nullable<byte>)] = DbType.Byte;
            NullableTypesToDataType[typeof(Nullable<DateTime>)] = DbType.DateTime;
            NullableTypesToDataType[typeof(Nullable<decimal>)] = DbType.Decimal;
            NullableTypesToDataType[typeof(Nullable<double>)] = DbType.Double;
            NullableTypesToDataType[typeof(Nullable<float>)] = DbType.Single;
            NullableTypesToDataType[typeof(Nullable<int>)] = DbType.Int32;
            NullableTypesToDataType[typeof(Nullable<long>)] = DbType.Int64;
            NullableTypesToDataType[typeof(Nullable<sbyte>)] = DbType.SByte;
            NullableTypesToDataType[typeof(Nullable<short>)] = DbType.Int16;
            NullableTypesToDataType[typeof(Nullable<uint>)] = DbType.UInt32;
            NullableTypesToDataType[typeof(Nullable<ulong>)] = DbType.UInt64;
            NullableTypesToDataType[typeof(Nullable<ushort>)] = DbType.UInt16;
            NullableTypesToDataType[typeof(Nullable<Guid>)] = DbType.Guid;

            NullableTypesToUnderlyingType = new HybridDictionary();
            NullableTypesToUnderlyingType[typeof(Nullable<bool>)] = typeof(Boolean);
            NullableTypesToUnderlyingType[typeof(Nullable<byte>)] = typeof(Byte);
            NullableTypesToUnderlyingType[typeof(Nullable<DateTime>)] = typeof(DateTime);
            NullableTypesToUnderlyingType[typeof(Nullable<decimal>)] = typeof(Decimal);
            NullableTypesToUnderlyingType[typeof(Nullable<double>)] = typeof(Double);
            NullableTypesToUnderlyingType[typeof(Nullable<float>)] = typeof(Single);
            NullableTypesToUnderlyingType[typeof(Nullable<int>)] = typeof(Int32);
            NullableTypesToUnderlyingType[typeof(Nullable<long>)] = typeof(Int64);
            NullableTypesToUnderlyingType[typeof(Nullable<sbyte>)] = typeof(SByte);
            NullableTypesToUnderlyingType[typeof(Nullable<short>)] = typeof(Int16);
            NullableTypesToUnderlyingType[typeof(Nullable<uint>)] = typeof(UInt32);
            NullableTypesToUnderlyingType[typeof(Nullable<ulong>)] = typeof(UInt64);
            NullableTypesToUnderlyingType[typeof(Nullable<ushort>)] = typeof(UInt16);
            NullableTypesToUnderlyingType[typeof(Nullable<Guid>)] = typeof(Guid);
        }

        public static bool IsNullableType(Type t)
        {
            //return type == null || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
            return NullableTypesToDataType.Contains(t);
        }

        public static DbType GetDataType(Type nullableType)
        {
            return (DbType)NullableTypesToDataType[nullableType];
        }

        public static Type GetUnderlyingType(Type nullableType)
        {
            return (Type)NullableTypesToUnderlyingType[nullableType];
        }

        public static ConstructorInfo GetConstructorInfo(Type nullableType)
        {
            if (!IsNullableType(nullableType))
            {
                throw new ArgumentOutOfRangeException();
            }
            ConstructorInfo ci = nullableType.GetConstructor(
                new[] { GetUnderlyingType(nullableType) });
            return ci;
        }

        public static object CreateNullableObject(Type nullableType, object value)
        {
            ConstructorInfo ci = GetConstructorInfo(nullableType);
            return ci.Invoke(new[] { value });
        }
    }
}
