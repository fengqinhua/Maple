//using Maple.Core.Data.DbMappers;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Linq;
//using System.Reflection;
//using System.Reflection.Emit;
//using System.Text;
//using System.Xml;
//using System.Xml.Linq;

//namespace Maple.Core.Data.SqlMapper
//{
//    internal class DataReaderDeserializer
//    {
//        static DataReaderDeserializer()
//        {
//             ResetTypeHandlers(false);
//        }

//        private static Dictionary<Type, ITypeHandler> typeHandlers;
//        internal const string LinqBinary = "System.Data.Linq.Binary"; 
//        private static ConstructorInfo GetDefaultConstructorInfo(Type type)
//        {
//            ConstructorInfo result = null;
//            var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
//            foreach (ConstructorInfo ctor in constructors)
//            {
//                ParameterInfo[] ctorParameters = ctor.GetParameters();
//                if (ctorParameters.Length == 0)
//                    result = ctor;
//            }

//            if (result == null)
//                throw new Exception("实体类必须包含共有的无参构造函数");
//            return result;
//        }
//        private static IList<IPropertyMapper> GetMapperByNames(IEntityMapper entityInfo, string[] names)
//        {
//            IList<IPropertyMapper> mappers = new List<IPropertyMapper>();

//            for (int i = 0; i < names.Length; i++)
//            {
//                string name = names[i];
//                IPropertyMapper pm = entityInfo.PKeyProperties.FirstOrDefault(f => f.ColumnName == name);
//                if (pm == null)
//                    pm = entityInfo.OtherProperties.FirstOrDefault(f => f.ColumnName == name);

//                mappers.Add(pm);
//            }
//            return mappers;
//        }
//        private static void EmitInt32(ILGenerator il, int value)
//        {
//            switch (value)
//            {
//                case -1: il.Emit(OpCodes.Ldc_I4_M1); break;
//                case 0: il.Emit(OpCodes.Ldc_I4_0); break;
//                case 1: il.Emit(OpCodes.Ldc_I4_1); break;
//                case 2: il.Emit(OpCodes.Ldc_I4_2); break;
//                case 3: il.Emit(OpCodes.Ldc_I4_3); break;
//                case 4: il.Emit(OpCodes.Ldc_I4_4); break;
//                case 5: il.Emit(OpCodes.Ldc_I4_5); break;
//                case 6: il.Emit(OpCodes.Ldc_I4_6); break;
//                case 7: il.Emit(OpCodes.Ldc_I4_7); break;
//                case 8: il.Emit(OpCodes.Ldc_I4_8); break;
//                default:
//                    if (value >= -128 && value <= 127)
//                    {
//                        il.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
//                    }
//                    else
//                    {
//                        il.Emit(OpCodes.Ldc_I4, value);
//                    }
//                    break;
//            }
//        }
//        private static void StoreLocal(ILGenerator il, int index)
//        {
//            if (index < 0 || index >= short.MaxValue) throw new ArgumentNullException(nameof(index));
//            switch (index)
//            {
//                case 0: il.Emit(OpCodes.Stloc_0); break;
//                case 1: il.Emit(OpCodes.Stloc_1); break;
//                case 2: il.Emit(OpCodes.Stloc_2); break;
//                case 3: il.Emit(OpCodes.Stloc_3); break;
//                default:
//                    if (index <= 255)
//                    {
//                        il.Emit(OpCodes.Stloc_S, (byte)index);
//                    }
//                    else
//                    {
//                        il.Emit(OpCodes.Stloc, (short)index);
//                    }
//                    break;
//            }
//        }
//        private static void LoadLocal(ILGenerator il, int index)
//        {
//            if (index < 0 || index >= short.MaxValue) throw new ArgumentNullException(nameof(index));
//            switch (index)
//            {
//                case 0: il.Emit(OpCodes.Ldloc_0); break;
//                case 1: il.Emit(OpCodes.Ldloc_1); break;
//                case 2: il.Emit(OpCodes.Ldloc_2); break;
//                case 3: il.Emit(OpCodes.Ldloc_3); break;
//                default:
//                    if (index <= 255)
//                    {
//                        il.Emit(OpCodes.Ldloc_S, (byte)index);
//                    }
//                    else
//                    {
//                        il.Emit(OpCodes.Ldloc, (short)index);
//                    }
//                    break;
//            }
//        }
//        private static void FlexibleConvertBoxedFromHeadOfStack(ILGenerator il, Type from, Type to, Type via)
//        {
//            MethodInfo op;
//            if (from == (via ?? to))
//            {
//                il.Emit(OpCodes.Unbox_Any, to); // stack is now [target][target][typed-value]
//            }
//            else if ((op = GetOperator(from, to)) != null)
//            {
//                // this is handy for things like decimal <===> double
//                il.Emit(OpCodes.Unbox_Any, from); // stack is now [target][target][data-typed-value]
//                il.Emit(OpCodes.Call, op); // stack is now [target][target][typed-value]
//            }
//            else
//            {
//                bool handled = false;
//                OpCode opCode = default(OpCode);
//                switch (TypeExtensions.GetTypeCode(from))
//                {
//                    case TypeCode.Boolean:
//                    case TypeCode.Byte:
//                    case TypeCode.SByte:
//                    case TypeCode.Int16:
//                    case TypeCode.UInt16:
//                    case TypeCode.Int32:
//                    case TypeCode.UInt32:
//                    case TypeCode.Int64:
//                    case TypeCode.UInt64:
//                    case TypeCode.Single:
//                    case TypeCode.Double:
//                        handled = true;
//                        switch (TypeExtensions.GetTypeCode(via ?? to))
//                        {
//                            case TypeCode.Byte:
//                                opCode = OpCodes.Conv_Ovf_I1_Un; break;
//                            case TypeCode.SByte:
//                                opCode = OpCodes.Conv_Ovf_I1; break;
//                            case TypeCode.UInt16:
//                                opCode = OpCodes.Conv_Ovf_I2_Un; break;
//                            case TypeCode.Int16:
//                                opCode = OpCodes.Conv_Ovf_I2; break;
//                            case TypeCode.UInt32:
//                                opCode = OpCodes.Conv_Ovf_I4_Un; break;
//                            case TypeCode.Boolean: // boolean is basically an int, at least at this level
//                            case TypeCode.Int32:
//                                opCode = OpCodes.Conv_Ovf_I4; break;
//                            case TypeCode.UInt64:
//                                opCode = OpCodes.Conv_Ovf_I8_Un; break;
//                            case TypeCode.Int64:
//                                opCode = OpCodes.Conv_Ovf_I8; break;
//                            case TypeCode.Single:
//                                opCode = OpCodes.Conv_R4; break;
//                            case TypeCode.Double:
//                                opCode = OpCodes.Conv_R8; break;
//                            default:
//                                handled = false;
//                                break;
//                        }
//                        break;
//                }
//                if (handled)
//                {
//                    il.Emit(OpCodes.Unbox_Any, from); // stack is now [target][target][col-typed-value]
//                    il.Emit(opCode); // stack is now [target][target][typed-value]
//                    if (to == typeof(bool))
//                    { // compare to zero; I checked "csc" - this is the trick it uses; nice
//                        il.Emit(OpCodes.Ldc_I4_0);
//                        il.Emit(OpCodes.Ceq);
//                        il.Emit(OpCodes.Ldc_I4_0);
//                        il.Emit(OpCodes.Ceq);
//                    }
//                }
//                else
//                {
//                    il.Emit(OpCodes.Ldtoken, via ?? to); // stack is now [target][target][value][member-type-token]
//                    il.EmitCall(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle)), null); // stack is now [target][target][value][member-type]
//                    il.EmitCall(OpCodes.Call, typeof(Convert).GetMethod(nameof(Convert.ChangeType), new Type[] { typeof(object), typeof(Type) }), null); // stack is now [target][target][boxed-member-type-value]
//                    il.Emit(OpCodes.Unbox_Any, to); // stack is now [target][target][typed-value]
//                }
//            }
//        }
//        private static MethodInfo GetOperator(Type from, Type to)
//        {
//            if (to == null) return null;
//            MethodInfo[] fromMethods, toMethods;
//            return ResolveOperator(fromMethods = from.GetMethods(BindingFlags.Static | BindingFlags.Public), from, to, "op_Implicit")
//                ?? ResolveOperator(toMethods = to.GetMethods(BindingFlags.Static | BindingFlags.Public), from, to, "op_Implicit")
//                ?? ResolveOperator(fromMethods, from, to, "op_Explicit")
//                ?? ResolveOperator(toMethods, from, to, "op_Explicit");
//        }
//        private static MethodInfo ResolveOperator(MethodInfo[] methods, Type from, Type to, string name)
//        {
//            for (int i = 0; i < methods.Length; i++)
//            {
//                if (methods[i].Name != name || methods[i].ReturnType != to) continue;
//                var args = methods[i].GetParameters();
//                if (args.Length != 1 || args[0].ParameterType != from) continue;
//                return methods[i];
//            }
//            return null;
//        }
        
//        private static readonly MethodInfo
//            enumParse = typeof(Enum).GetMethod(nameof(Enum.Parse), new Type[] { typeof(Type), typeof(string), typeof(bool) }),
//            getItem = typeof(IDataRecord).GetProperties(BindingFlags.Instance | BindingFlags.Public)
//                .Where(p => p.GetIndexParameters().Length > 0 && p.GetIndexParameters()[0].ParameterType == typeof(int))
//                .Select(p => p.GetGetMethod()).First();
//        /// <summary>
//        /// Internal use only.
//        /// </summary>
//        /// <param name="value">The object to convert to a character.</param>
//#if !NETSTANDARD1_3
//        [Browsable(false)]
//#endif
//        [EditorBrowsable(EditorBrowsableState.Never)]
//        [Obsolete("This method is for internal use only", false)]
//        public static char ReadChar(object value)
//        {
//            if (value == null || value is DBNull) throw new ArgumentNullException(nameof(value));
//            var s = value as string;
//            if (s == null || s.Length != 1) throw new ArgumentException("A single-character was expected", nameof(value));
//            return s[0];
//        }

//        /// <summary>
//        /// Internal use only.
//        /// </summary>
//        /// <param name="value">The object to convert to a character.</param>
//#if !NETSTANDARD1_3
//        [Browsable(false)]
//#endif
//        [EditorBrowsable(EditorBrowsableState.Never)]
//        [Obsolete("This method is for internal use only", false)]
//        public static char? ReadNullableChar(object value)
//        {
//            if (value == null || value is DBNull) return null;
//            var s = value as string;
//            if (s == null || s.Length != 1) throw new ArgumentException("A single-character was expected", nameof(value));
//            return s[0];
//        }

//        /// <summary>
//        /// Clear the registered type handlers.
//        /// </summary>
//        public static void ResetTypeHandlers() => ResetTypeHandlers(true);
//        /// <summary>
//        /// Configure the specified type to be processed by a custom handler.
//        /// </summary>
//        /// <param name="type">The type to handle.</param>
//        /// <param name="handler">The handler to process the <paramref name="type"/>.</param>
//        /// <param name="clone">Whether to clone the current type handler map.</param>
//        public static void AddTypeHandlerImpl(Type type, ITypeHandler handler, bool clone)
//        {
//            if (type == null) throw new ArgumentNullException(nameof(type));

//            Type secondary = null;
//            if (type.IsValueType())
//            {
//                var underlying = Nullable.GetUnderlyingType(type);
//                if (underlying == null)
//                {
//                    secondary = typeof(Nullable<>).MakeGenericType(type); // the Nullable<T>
//                    // type is already the T
//                }
//                else
//                {
//                    secondary = type; // the Nullable<T>
//                    type = underlying; // the T
//                }
//            }

//            var snapshot = typeHandlers;
//            if (snapshot.TryGetValue(type, out ITypeHandler oldValue) && handler == oldValue) return; // nothing to do

//            var newCopy = clone ? new Dictionary<Type, ITypeHandler>(snapshot) : snapshot;

//#pragma warning disable 618
//            typeof(TypeHandlerCache<>).MakeGenericType(type).GetMethod(nameof(TypeHandlerCache<int>.SetHandler), BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { handler });
//            if (secondary != null)
//            {
//                typeof(TypeHandlerCache<>).MakeGenericType(secondary).GetMethod(nameof(TypeHandlerCache<int>.SetHandler), BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { handler });
//            }
//#pragma warning restore 618
//            if (handler == null)
//            {
//                newCopy.Remove(type);
//                if (secondary != null) newCopy.Remove(secondary);
//            }
//            else
//            {
//                newCopy[type] = handler;
//                if (secondary != null) newCopy[secondary] = handler;
//            }
//            typeHandlers = newCopy;
//        }

//        private static void ResetTypeHandlers(bool clone)
//        {
//            typeHandlers = new Dictionary<Type, ITypeHandler>();

//            AddTypeHandlerImpl(typeof(XmlDocument), new XmlDocumentHandler(), clone);
//            AddTypeHandlerImpl(typeof(XDocument), new XDocumentHandler(), clone);
//            AddTypeHandlerImpl(typeof(XElement), new XElementHandler(), clone);
//        }

//        /// <summary>
//        /// 将IDataReader转换为实体对象
//        /// </summary>
//        /// <param name="type">实体对象类型</param>
//        /// <param name="reader">IDataReader</param>
//        /// <param name="entityInfo">实体类IEntity与数据库对应的映射信息</param>
//        /// <param name="startBound">起始字段的索引</param>
//        /// <param name="length">读取字段的总数</param>
//        /// <param name="returnNullIfFirstMissing"></param>
//        /// <returns></returns>
//        protected Func<IDataReader, object> GetTypeDeserializerImpl(Type type, IDataReader reader, IEntityMapper entityInfo, int startBound = 0, int length = -1, bool returnNullIfFirstMissing = false)
//        {
//            //创建一个动态函数
//            var dm = new DynamicMethod("Deserialize" + Guid.NewGuid().ToString(), type, new[] { typeof(IDataReader) }, type, true);
//            var il = dm.GetILGenerator();
//            il.DeclareLocal(typeof(int));
//            il.DeclareLocal(type);
//            il.Emit(OpCodes.Ldc_I4_0);
//            il.Emit(OpCodes.Stloc_0);

//            if (length == -1)
//                length = reader.FieldCount - startBound;
//            if (reader.FieldCount <= startBound)
//                throw new Exception("起始字段的索引超出了IDataReader字段总数！");

//            //获取需读取的字段名称集合
//            var names = Enumerable.Range(startBound, length).Select(i => reader.GetName(i)).ToArray();
//            //起始字段索引
//            int index = startBound;

//#if !NETSTANDARD1_3
//            bool supportInitialize = false;
//#endif
//            //获取各数据库字段类型
//            var types = new Type[length];
//            for (int i = startBound; i < startBound + length; i++)
//            {
//                types[i - startBound] = reader.GetFieldType(i);
//            }
//            //获取实体类的无参构造函数
//            var ctor = type.GetConstructor(Type.EmptyTypes);  //GetDefaultConstructorInfo(type)
//            il.Emit(OpCodes.Newobj, ctor);
//            il.Emit(OpCodes.Stloc_1);
//#if !NETSTANDARD1_3
//            supportInitialize = typeof(ISupportInitialize).IsAssignableFrom(type);
//            if (supportInitialize)
//            {
//                il.Emit(OpCodes.Ldloc_1);
//                il.EmitCall(OpCodes.Callvirt, typeof(ISupportInitialize).GetMethod(nameof(ISupportInitialize.BeginInit)), null);
//            }
//#endif
//            il.BeginExceptionBlock();

//            //根据字段名称获取 IPropertyMapper
//            var members = GetMapperByNames(entityInfo, names);

//            //开始创建实体类
//            // stack is now [target]

//            bool first = true;
//            var allDone = il.DefineLabel();
//            //定义第二个本地变量,object类型的, 然后返回此本地变量的index值, 其实就是截止目前, 定义了本地变量的个数
//            int enumDeclareLocal = -1, valueCopyLocal = il.DeclareLocal(typeof(object)).LocalIndex;
//            bool applyNullSetting = false;                  //标识是否忽略为空的数据
//            foreach (var item in members)
//            {
//                if (item != null)
//                {
//                    il.Emit(OpCodes.Dup); // stack is now [target][target]
//                    Label isDbNullLabel = il.DefineLabel();
//                    Label finishLabel = il.DefineLabel();

//                    il.Emit(OpCodes.Ldarg_0); // stack is now [target][target][reader]
//                    EmitInt32(il, index); // stack is now [target][target][reader][index]
//                    il.Emit(OpCodes.Dup);// stack is now [target][target][reader][index][index]
//                    il.Emit(OpCodes.Stloc_0);// stack is now [target][target][reader][index]
//                    il.Emit(OpCodes.Callvirt, getItem); // stack is now [target][target][value-as-object]
//                    il.Emit(OpCodes.Dup); // stack is now [target][target][value-as-object][value-as-object]
//                    StoreLocal(il, valueCopyLocal);
//                    Type colType = reader.GetFieldType(index);
//                    Type memberType = item.PropertyInfo.PropertyType;

//                    if (memberType == typeof(char) || memberType == typeof(char?))
//                    {
//                        il.EmitCall(OpCodes.Call, typeof(DataReaderDeserializer).GetMethod(
//                            memberType == typeof(char) ? nameof(DataReaderDeserializer.ReadChar) : nameof(DataReaderDeserializer.ReadNullableChar), BindingFlags.Static | BindingFlags.Public), null); // stack is now [target][target][typed-value]
//                    }
//                    else
//                    {
//                        il.Emit(OpCodes.Dup); // stack is now [target][target][value][value]
//                        il.Emit(OpCodes.Isinst, typeof(DBNull)); // stack is now [target][target][value-as-object][DBNull or null]
//                        il.Emit(OpCodes.Brtrue_S, isDbNullLabel); // stack is now [target][target][value-as-object]

//                        // unbox nullable enums as the primitive, i.e. byte etc

//                        var nullUnderlyingType = Nullable.GetUnderlyingType(memberType);
//                        var unboxType = nullUnderlyingType?.IsEnum() == true ? nullUnderlyingType : memberType;

//                        if (unboxType.IsEnum())
//                        {
//                            Type numericType = Enum.GetUnderlyingType(unboxType);
//                            if (colType == typeof(string))
//                            {
//                                if (enumDeclareLocal == -1)
//                                {
//                                    enumDeclareLocal = il.DeclareLocal(typeof(string)).LocalIndex;
//                                }
//                                il.Emit(OpCodes.Castclass, typeof(string)); // stack is now [target][target][string]
//                                StoreLocal(il, enumDeclareLocal); // stack is now [target][target]
//                                il.Emit(OpCodes.Ldtoken, unboxType); // stack is now [target][target][enum-type-token]
//                                il.EmitCall(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle)), null);// stack is now [target][target][enum-type]
//                                LoadLocal(il, enumDeclareLocal); // stack is now [target][target][enum-type][string]
//                                il.Emit(OpCodes.Ldc_I4_1); // stack is now [target][target][enum-type][string][true]
//                                il.EmitCall(OpCodes.Call, enumParse, null); // stack is now [target][target][enum-as-object]
//                                il.Emit(OpCodes.Unbox_Any, unboxType); // stack is now [target][target][typed-value]
//                            }
//                            else
//                            {
//                                FlexibleConvertBoxedFromHeadOfStack(il, colType, unboxType, numericType);
//                            }

//                            if (nullUnderlyingType != null)
//                            {
//                                il.Emit(OpCodes.Newobj, memberType.GetConstructor(new[] { nullUnderlyingType })); // stack is now [target][target][typed-value]
//                            }
//                        }
//                        else if (memberType.FullName == LinqBinary)
//                        {
//                            il.Emit(OpCodes.Unbox_Any, typeof(byte[])); // stack is now [target][target][byte-array]
//                            il.Emit(OpCodes.Newobj, memberType.GetConstructor(new Type[] { typeof(byte[]) }));// stack is now [target][target][binary]
//                        }
//                        else
//                        {
//                            TypeCode dataTypeCode = TypeExtensions.GetTypeCode(colType), 
//                                     unboxTypeCode = TypeExtensions.GetTypeCode(unboxType);
//                            bool hasTypeHandler;
//                            if ((hasTypeHandler = typeHandlers.ContainsKey(unboxType)) || colType == unboxType || dataTypeCode == unboxTypeCode || dataTypeCode == TypeExtensions.GetTypeCode(nullUnderlyingType))
//                            {
//                                if (hasTypeHandler)
//                                {
//#pragma warning disable 618
//                                    il.EmitCall(OpCodes.Call, typeof(TypeHandlerCache<>).MakeGenericType(unboxType).GetMethod(nameof(TypeHandlerCache<int>.Parse)), null); // stack is now [target][target][typed-value]
//#pragma warning restore 618
//                                }
//                                else
//                                {
//                                    il.Emit(OpCodes.Unbox_Any, unboxType); // stack is now [target][target][typed-value]
//                                }
//                            }
//                            else
//                            {
//                                // not a direct match; need to tweak the unbox
//                                FlexibleConvertBoxedFromHeadOfStack(il, colType, nullUnderlyingType ?? unboxType, null);
//                                if (nullUnderlyingType != null)
//                                {
//                                    il.Emit(OpCodes.Newobj, unboxType.GetConstructor(new[] { nullUnderlyingType })); // stack is now [target][target][typed-value]
//                                }
//                            }
//                        }
//                    }

//                    if (specializedConstructor == null)
//                    {
//                        // Store the value in the property/field
//                        if (item.Property != null)
//                        {
//                            il.Emit(type.IsValueType() ? OpCodes.Call : OpCodes.Callvirt, DefaultTypeMap.GetPropertySetter(item.Property, type));
//                        }
//                        else
//                        {
//                            il.Emit(OpCodes.Stfld, item.Field); // stack is now [target]
//                        }
//                    }

//                    il.Emit(OpCodes.Br_S, finishLabel); // stack is now [target]

//                    il.MarkLabel(isDbNullLabel); // incoming stack: [target][target][value]
//                    if (specializedConstructor != null)
//                    {
//                        il.Emit(OpCodes.Pop);
//                        if (item.MemberType.IsValueType())
//                        {
//                            int localIndex = il.DeclareLocal(item.MemberType).LocalIndex;
//                            LoadLocalAddress(il, localIndex);
//                            il.Emit(OpCodes.Initobj, item.MemberType);
//                            LoadLocal(il, localIndex);
//                        }
//                        else
//                        {
//                            il.Emit(OpCodes.Ldnull);
//                        }
//                    }
//                    else if (applyNullSetting && (!memberType.IsValueType() || Nullable.GetUnderlyingType(memberType) != null))
//                    {
//                        il.Emit(OpCodes.Pop); // stack is now [target][target]
//                        // can load a null with this value
//                        if (memberType.IsValueType())
//                        { // must be Nullable<T> for some T
//                            GetTempLocal(il, ref structLocals, memberType, true); // stack is now [target][target][null]
//                        }
//                        else
//                        { // regular reference-type
//                            il.Emit(OpCodes.Ldnull); // stack is now [target][target][null]
//                        }

//                        // Store the value in the property/field
//                        if (item.Property != null)
//                        {
//                            il.Emit(type.IsValueType() ? OpCodes.Call : OpCodes.Callvirt, DefaultTypeMap.GetPropertySetter(item.Property, type));
//                            // stack is now [target]
//                        }
//                        else
//                        {
//                            il.Emit(OpCodes.Stfld, item.Field); // stack is now [target]
//                        }
//                    }
//                    else
//                    {
//                        il.Emit(OpCodes.Pop); // stack is now [target][target]
//                        il.Emit(OpCodes.Pop); // stack is now [target]
//                    }

//                    if (first && returnNullIfFirstMissing)
//                    {
//                        il.Emit(OpCodes.Pop);
//                        il.Emit(OpCodes.Ldnull); // stack is now [null]
//                        il.Emit(OpCodes.Stloc_1);
//                        il.Emit(OpCodes.Br, allDone);
//                    }

//                    il.MarkLabel(finishLabel);
//                }
//                first = false;
//                index++;
//            }
//            if (type.IsValueType())
//            {
//                il.Emit(OpCodes.Pop);
//            }
//            else
//            {
//                if (specializedConstructor != null)
//                {
//                    il.Emit(OpCodes.Newobj, specializedConstructor);
//                }
//                il.Emit(OpCodes.Stloc_1); // stack is empty
//#if !NETSTANDARD1_3
//                if (supportInitialize)
//                {
//                    il.Emit(OpCodes.Ldloc_1);
//                    il.EmitCall(OpCodes.Callvirt, typeof(ISupportInitialize).GetMethod(nameof(ISupportInitialize.EndInit)), null);
//                }
//#endif
//            }
//            il.MarkLabel(allDone);
//            il.BeginCatchBlock(typeof(Exception)); // stack is Exception
//            il.Emit(OpCodes.Ldloc_0); // stack is Exception, index
//            il.Emit(OpCodes.Ldarg_0); // stack is Exception, index, reader
//            LoadLocal(il, valueCopyLocal); // stack is Exception, index, reader, value
//            il.EmitCall(OpCodes.Call, typeof(SqlMapper).GetMethod(nameof(SqlMapper.ThrowDataException)), null);
//            il.EndExceptionBlock();

//            il.Emit(OpCodes.Ldloc_1); // stack is [rval]
//            if (type.IsValueType())
//            {
//                il.Emit(OpCodes.Box, type);
//            }
//            il.Emit(OpCodes.Ret);

//            var funcType = System.Linq.Expressions.Expression.GetFuncType(typeof(IDataReader), returnType);
//            return (Func<IDataReader, object>)dm.CreateDelegate(funcType);
//        }


//    }
//}
