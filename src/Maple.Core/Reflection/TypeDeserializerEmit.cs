using Maple.Core.Data.DbMappers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Maple.Core.Reflection
{
    public delegate object DataReaderDeserializer(IDataReader dataReader);

    public static class TypeDeserializerEmit
    {
        #region Static Readonly Fields

        private static readonly MethodInfo DataRecord_IsDBNull = typeof(IDataRecord).GetMethod("IsDBNull");
        private static readonly MethodInfo DataRecord_GetString = typeof(IDataRecord).GetMethod("GetString");
        private static readonly MethodInfo DataRecord_GetInt16 = typeof(IDataRecord).GetMethod("GetInt16");
        private static readonly MethodInfo DataRecord_GetInt32 = typeof(IDataRecord).GetMethod("GetInt32");
        private static readonly MethodInfo DataRecord_GetInt64 = typeof(IDataRecord).GetMethod("GetInt64");
        private static readonly MethodInfo DataRecord_GetDouble = typeof(IDataRecord).GetMethod("GetDouble");
        private static readonly MethodInfo DataRecord_GetDecimal = typeof(IDataRecord).GetMethod("GetDecimal");
        private static readonly MethodInfo DataRecord_GetDateTime = typeof(IDataRecord).GetMethod("GetDateTime");
        private static readonly MethodInfo DataRecord_GetGuid = typeof(IDataRecord).GetMethod("GetGuid");
        private static readonly MethodInfo DataRecord_GetByte = typeof(IDataRecord).GetMethod("GetByte");
        private static readonly MethodInfo DataRecord_GetValue = typeof(IDataRecord).GetMethod("GetValue");

        #endregion

        public static DataReaderDeserializer CreateDataReaderDeserializer(IEntityMapper entityInfo, IDataReader dataReader)
        {
            Check.NotNull(entityInfo, nameof(entityInfo));
            Check.NotNull(dataReader, nameof(dataReader));

            DynamicMethod dm = new DynamicMethod("DR_Deserializer_" + Guid.NewGuid().ToString(), entityInfo.EntityType, new Type[] { typeof(IDataReader) });
            ILGenerator il = dm.GetILGenerator();

            BuildFunction(entityInfo, dataReader, il);

            return (DataReaderDeserializer)dm.CreateDelegate(typeof(DataReaderDeserializer));
        }

        public static void BuildFunction(IEntityMapper entityInfo, IDataReader dataReader, ILGenerator il)
        {
            ConstructorInfo entityConstructor = entityInfo.EntityType.GetConstructor(Type.EmptyTypes);
            //定义变量，存储Entity对象
            LocalBuilder local0 = il.DeclareLocal(entityInfo.EntityType);

            //********************
            //创建对象实例
            //********************
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Newobj, entityConstructor);
            il.Emit(OpCodes.Stloc_0);
            //********************
            //创建DataObject实例
            //********************
            foreach (IDataObjectMapper item in entityInfo.DataObjectProperties)
            {
                if (item.PropertyInfo != null)
                {
                    ConstructorInfo dataObjectConstructor = item.PropertyInfo.PropertyType.GetConstructor(Type.EmptyTypes);
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Newobj, dataObjectConstructor);
                    il.EmitCall(OpCodes.Callvirt, item.PropertyInfo.GetSetMethod(true), null);
                }
            }
            //********************
            //属性赋值
            //********************
            foreach (IPropertyMapper item in entityInfo.PKeyProperties)
            {
                int index = dataReader.GetOrdinal(item.ColumnName);
                ReadValue(il, item, index);
            }

            foreach (IPropertyMapper item in entityInfo.OtherProperties)
            {
                int index = dataReader.GetOrdinal(item.ColumnName);
                ReadValue(il, item, index);
            }

            //********************
            //返回结果
            //********************
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);
        }

        private static void ReadValue(ILGenerator il, IPropertyMapper item, int index)
        {
            MethodInfo propertyMethodInfo = item.PropertyInfo.GetSetMethod(true);
            MethodInfo dataObjectPropertyMethodInfo = item.DataObjectPropertyInfo == null ? null : item.DataObjectPropertyInfo.GetGetMethod(true);
            switch (item.DbType)
            {
                case DbType.Binary:
                    ReadBinary(il, index, propertyMethodInfo, dataObjectPropertyMethodInfo);
                    break;
                case DbType.Boolean:
                    if (item.AllowsNulls)
                        ReadNullableBoolean(il, index, item.PropertyInfo.PropertyType, propertyMethodInfo, dataObjectPropertyMethodInfo);
                    else
                        ReadBoolean(il, index, item.PropertyInfo.PropertyType, propertyMethodInfo, dataObjectPropertyMethodInfo);
                    break;
                case DbType.Byte:
                    if (item.AllowsNulls)
                        ReadCommonNullableValue(il, index, item.PropertyInfo.PropertyType, propertyMethodInfo, dataObjectPropertyMethodInfo, DataRecord_GetByte);
                    else
                        ReadCommonValue(il, index, item.PropertyInfo.PropertyType, propertyMethodInfo, dataObjectPropertyMethodInfo, DataRecord_GetByte);
                    break;
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                    if (item.AllowsNulls)
                        ReadCommonNullableValue(il, index, item.PropertyInfo.PropertyType, propertyMethodInfo, dataObjectPropertyMethodInfo, DataRecord_GetDateTime);
                    else
                        ReadCommonValue(il, index, item.PropertyInfo.PropertyType, propertyMethodInfo, dataObjectPropertyMethodInfo, DataRecord_GetDateTime);
                    break;
                case DbType.Decimal:
                    if (item.AllowsNulls)
                        ReadCommonNullableValue(il, index, item.PropertyInfo.PropertyType, propertyMethodInfo, dataObjectPropertyMethodInfo, DataRecord_GetDecimal);
                    else
                        ReadCommonValue(il, index, item.PropertyInfo.PropertyType, propertyMethodInfo, dataObjectPropertyMethodInfo, DataRecord_GetDecimal);
                    break;
                case DbType.Double:
                    if (item.AllowsNulls)
                        ReadCommonNullableValue(il, index, item.PropertyInfo.PropertyType, propertyMethodInfo, dataObjectPropertyMethodInfo, DataRecord_GetDouble);
                    else
                        ReadCommonValue(il, index, item.PropertyInfo.PropertyType, propertyMethodInfo, dataObjectPropertyMethodInfo, DataRecord_GetDouble);
                    break;
                case DbType.Guid:
                    if (item.AllowsNulls)
                        ReadCommonNullableValue(il, index, item.PropertyInfo.PropertyType, propertyMethodInfo, dataObjectPropertyMethodInfo, DataRecord_GetGuid);
                    else
                        ReadCommonValue(il, index, item.PropertyInfo.PropertyType, propertyMethodInfo, dataObjectPropertyMethodInfo, DataRecord_GetGuid);
                    break;
                case DbType.Int16:
                    if (item.AllowsNulls)
                        ReadCommonNullableValue(il, index, item.PropertyInfo.PropertyType, propertyMethodInfo, dataObjectPropertyMethodInfo, DataRecord_GetInt16);
                    else
                        ReadCommonValue(il, index, item.PropertyInfo.PropertyType, propertyMethodInfo, dataObjectPropertyMethodInfo, DataRecord_GetInt16);
                    break;
                case DbType.Int32:
                    if (item.AllowsNulls)
                        ReadCommonNullableValue(il, index, item.PropertyInfo.PropertyType, propertyMethodInfo, dataObjectPropertyMethodInfo, DataRecord_GetInt32);
                    else
                        ReadCommonValue(il, index, item.PropertyInfo.PropertyType, propertyMethodInfo, dataObjectPropertyMethodInfo, DataRecord_GetInt32);
                    break;
                case DbType.Int64:
                    if (item.AllowsNulls)
                        ReadCommonNullableValue(il, index, item.PropertyInfo.PropertyType, propertyMethodInfo, dataObjectPropertyMethodInfo, DataRecord_GetInt64);
                    else
                        ReadCommonValue(il, index, item.PropertyInfo.PropertyType, propertyMethodInfo, dataObjectPropertyMethodInfo, DataRecord_GetInt64);
                    break;
                case DbType.String:
                    ReadCommonNullableValue(il, index, item.PropertyInfo.PropertyType, propertyMethodInfo, dataObjectPropertyMethodInfo, DataRecord_GetString);
                    //if (item.AllowsNulls)
                    //    ReadCommonNullableValue(il, index, typeof(string), propertyMethodInfo, dataObjectPropertyMethodInfo, DataRecord_GetString);
                    //else
                    //    ReadCommonValue(il, index, typeof(string), propertyMethodInfo, dataObjectPropertyMethodInfo, DataRecord_GetString);
                    break;
                default:
                    break;
            }
        }
        private static void ReadCommonValue(ILGenerator il, int i, Type propertyType, MethodInfo propertySetMethod, MethodInfo dataObjectGetMethod, MethodInfo dataReadGetValueMethod)
        {
            //将索引 0 处的局部变量 ( Entity ) 加载到计算堆栈上。
            il.Emit(OpCodes.Ldloc_0);
            //判断是否为DataObject赋值
            if (dataObjectGetMethod != null)
                il.Emit(OpCodes.Callvirt, dataObjectGetMethod);
            //将索引为 1 的参数（IDataread）加载到计算堆栈上
            il.Emit(OpCodes.Ldarg_0);
            ldc_num(il, i);
            il.Emit(OpCodes.Callvirt, dataReadGetValueMethod);

            //如果是可空类型，那么要实例化
            if (propertyType.IsIncludingNullable())
            {
                var nullUnderlyingType = Nullable.GetUnderlyingType(propertyType);
                ConstructorInfo nullableConstructor = propertyType.GetConstructor(new[] { nullUnderlyingType });
                il.Emit(OpCodes.Newobj, nullableConstructor);
            }

            il.EmitCall(OpCodes.Callvirt, propertySetMethod, null);
        }
        private static void ReadCommonNullableValue(ILGenerator il, int i, Type propertyType, MethodInfo propertySetMethod, MethodInfo dataObjectGetMethod, MethodInfo dataReadGetValueMethod)
        {
            Label isNull = il.DefineLabel();

            //将索引为 1 的参数（IDataread）加载到计算堆栈上
            il.Emit(OpCodes.Ldarg_0);
            ldc_num(il, i);
            il.Emit(OpCodes.Callvirt, DataRecord_IsDBNull);
            //如果 value 为 true、非空或非零，则将控制转移到目标指令（短格式）
            il.Emit(OpCodes.Brtrue_S, isNull);

            //将索引 0 处的局部变量 ( Entity ) 加载到计算堆栈上。
            il.Emit(OpCodes.Ldloc_0);
            //判断是否为DataObject赋值
            if (dataObjectGetMethod != null)
                il.Emit(OpCodes.Callvirt, dataObjectGetMethod);
            //将索引为 1 的参数（IDataread）加载到计算堆栈上
            il.Emit(OpCodes.Ldarg_0);
            ldc_num(il, i);
            il.Emit(OpCodes.Callvirt, dataReadGetValueMethod);

            //如果是可空类型，那么要实例化
            if (propertyType.IsIncludingNullable())
            {
                Type nullUnderlyingType = Nullable.GetUnderlyingType(propertyType);
                ConstructorInfo nullableConstructor = propertyType.GetConstructor(new[] { nullUnderlyingType });
                il.Emit(OpCodes.Newobj, nullableConstructor);
            }
            il.EmitCall(OpCodes.Callvirt, propertySetMethod, null);

            il.MarkLabel(isNull);
        }

        private static void ReadBinary(ILGenerator il, int i, MethodInfo propertySetMethod, MethodInfo dataObjectGetMethod)
        {
            var type = typeof(byte[]);
            var local = il.DeclareLocal(type);
            //将索引 0 处的局部变量 ( Entity ) 加载到计算堆栈上。
            il.Emit(OpCodes.Ldloc_0);
            //判断是否为DataObject赋值
            if (dataObjectGetMethod != null)
                il.Emit(OpCodes.Callvirt, dataObjectGetMethod);
            //将索引为 1 的参数（IDataread）加载到计算堆栈上
            il.Emit(OpCodes.Ldarg_0);
            ldc_num(il, i);
            il.Emit(OpCodes.Callvirt, DataRecord_GetValue);
            il.Emit(OpCodes.Castclass, type);
            il.EmitCall(OpCodes.Callvirt, propertySetMethod, null);

            //IL_019d: nop
            //IL_019e: ldloc.0
            //IL_019f: ldarg.1
            //IL_01a0: ldc.i4 1028
            //IL_01a5: callvirt instance object [System.Data]System.Data.IDataRecord::GetValue(int32)
            //IL_01aa: castclass uint8[]
            //IL_01af: callvirt instance void ILTEST.User::set_Picture(uint8[])

        }
        private static void ReadBoolean(ILGenerator il, int i, Type propertyType, MethodInfo propertySetMethod, MethodInfo dataObjectGetMethod)
        {
            Label IL_01c6 = il.DefineLabel();
            Label IL_01c7 = il.DefineLabel();

            //将索引 0 处的局部变量 ( Entity ) 加载到计算堆栈上。
            il.Emit(OpCodes.Ldloc_0);
            //判断是否为DataObject赋值
            if (dataObjectGetMethod != null)
                il.Emit(OpCodes.Callvirt, dataObjectGetMethod);
            //将索引为 1 的参数（IDataread）加载到计算堆栈上
            il.Emit(OpCodes.Ldarg_0);
            ldc_num(il, i);
            il.Emit(OpCodes.Callvirt, DataRecord_GetInt16);
            il.Emit(OpCodes.Brfalse_S, IL_01c6);


            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Br_S, IL_01c7);

            il.MarkLabel(IL_01c6);
            il.Emit(OpCodes.Ldc_I4_0);

            il.MarkLabel(IL_01c7);

            //如果是可空类型，那么要实例化
            if (propertyType.IsIncludingNullable())
            {
                var nullUnderlyingType = Nullable.GetUnderlyingType(propertyType);
                ConstructorInfo nullableConstructor = propertyType.GetConstructor(new[] { nullUnderlyingType });
                il.Emit(OpCodes.Newobj, nullableConstructor);
            }
            il.EmitCall(OpCodes.Callvirt, propertySetMethod, null);

            //IL_01b4: nop
            //IL_01b5: ldloc.0
            //IL_01b6: ldarg.1
            //IL_01b7: ldc.i4 1029
            //IL_01bc: callvirt instance int32 [System.Data]System.Data.IDataRecord::GetInt32(int32)
            //IL_01c1: brfalse.s IL_01c6

            //IL_01c3: ldc.i4.1
            //IL_01c4: br.s IL_01c7

            //IL_01c6: ldc.i4.0

            //IL_01c7: nop
            //IL_01c8: callvirt instance void ILTEST.User::set_IsDeleted(bool)

        }
        private static void ReadNullableBoolean(ILGenerator il, int i, Type propertyType, MethodInfo propertySetMethod, MethodInfo dataObjectGetMethod)
        {
            Label IL_01c6 = il.DefineLabel();
            Label IL_01c7 = il.DefineLabel();
            Label isNull = il.DefineLabel();

            //将索引为 1 的参数（IDataread）加载到计算堆栈上
            il.Emit(OpCodes.Ldarg_0);
            ldc_num(il, i);
            il.Emit(OpCodes.Callvirt, DataRecord_IsDBNull);
            //如果 value 为 true、非空或非零，则将控制转移到目标指令（短格式）
            il.Emit(OpCodes.Brtrue_S, isNull);


            //将索引 0 处的局部变量 ( Entity ) 加载到计算堆栈上。
            il.Emit(OpCodes.Ldloc_0);
            //判断是否为DataObject赋值
            if (dataObjectGetMethod != null)
                il.Emit(OpCodes.Callvirt, dataObjectGetMethod);
            //将索引为 1 的参数（IDataread）加载到计算堆栈上
            il.Emit(OpCodes.Ldarg_0);
            ldc_num(il, i);
            il.Emit(OpCodes.Callvirt, DataRecord_GetInt16);
            il.Emit(OpCodes.Brfalse_S, IL_01c6);


            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Br_S, IL_01c7);

            il.MarkLabel(IL_01c6);
            il.Emit(OpCodes.Ldc_I4_0);

            il.MarkLabel(IL_01c7);

            //如果是可空类型，那么要实例化
            if (propertyType.IsIncludingNullable())
            {
                var nullUnderlyingType = Nullable.GetUnderlyingType(propertyType);
                ConstructorInfo nullableConstructor = propertyType.GetConstructor(new[] { nullUnderlyingType });
                il.Emit(OpCodes.Newobj, nullableConstructor);
            }
            il.EmitCall(OpCodes.Callvirt, propertySetMethod, null);

            il.MarkLabel(isNull);
        }
        private static void ldc_num(ILGenerator il, int i)
        {
            //将整数值推送到计算堆栈上。
            if (i <= 8)
            {
                switch (i)
                {
                    case 0:
                        il.Emit(OpCodes.Ldc_I4_0);
                        break;
                    case 1:
                        il.Emit(OpCodes.Ldc_I4_1);
                        break;
                    case 2:
                        il.Emit(OpCodes.Ldc_I4_2);
                        break;
                    case 3:
                        il.Emit(OpCodes.Ldc_I4_3);
                        break;
                    case 4:
                        il.Emit(OpCodes.Ldc_I4_4);
                        break;
                    case 5:
                        il.Emit(OpCodes.Ldc_I4_5);
                        break;
                    case 6:
                        il.Emit(OpCodes.Ldc_I4_6);
                        break;
                    case 7:
                        il.Emit(OpCodes.Ldc_I4_7);
                        break;
                    case 8:
                        il.Emit(OpCodes.Ldc_I4_8);
                        break;
                    default:
                        il.Emit(OpCodes.Ldc_I4, i);
                        break;
                }
            }
            else if (i <= 127)
                il.Emit(OpCodes.Ldc_I4_S, i);
            else
                il.Emit(OpCodes.Ldc_I4, i);
        }
    }
}
