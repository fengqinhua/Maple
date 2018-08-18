using System;
using System.Reflection.Emit;
using System.Reflection;
using System.Data;
using Maple.Core.Data.DbMappers;
using System.Collections.Generic;

namespace Maple.Core.Reflection
{
    public delegate object CtorDelegate();

    public delegate object MethodDelegate(object target, object[] args);

    public delegate object GetValueDelegate(object target);

    public delegate void SetValueDelegate(object target, object arg);

    

    public static class DynamicMethodFactory
    {
        public static CtorDelegate CreateConstructor(ConstructorInfo constructor)
        {
            Check.NotNull(constructor, nameof(constructor));

            if (constructor.GetParameters().Length > 0)
                throw new NotSupportedException("不支持有参数的构造函数。");

            DynamicMethod dm = new DynamicMethod(
                "ctor" + Guid.NewGuid().ToString(),
                constructor.DeclaringType,
                Type.EmptyTypes,
                true);

            ILGenerator il = dm.GetILGenerator();
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Newobj, constructor);
            il.Emit(OpCodes.Ret);

            return (CtorDelegate)dm.CreateDelegate(typeof(CtorDelegate));
        }

        public static MethodDelegate CreateMethod(MethodInfo method)
        {
            ParameterInfo[] pi = method.GetParameters();

            DynamicMethod dm = new DynamicMethod("DynamicMethod" + Guid.NewGuid().ToString(), typeof(object),
                new Type[] { typeof(object), typeof(object[]) },
                typeof(DynamicMethodFactory), true);

            ILGenerator il = dm.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);

            for (int index = 0; index < pi.Length; index++)
            {
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldc_I4, index);

                Type parameterType = pi[index].ParameterType;
                if (parameterType.IsByRef)
                {
                    parameterType = parameterType.GetElementType();
                    if (parameterType.IsValueType)
                    {
                        il.Emit(OpCodes.Ldelem_Ref);
                        il.Emit(OpCodes.Unbox, parameterType);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldelema, parameterType);
                    }
                }
                else
                {
                    il.Emit(OpCodes.Ldelem_Ref);

                    if (parameterType.IsValueType)
                    {
                        il.Emit(OpCodes.Unbox, parameterType);
                        il.Emit(OpCodes.Ldobj, parameterType);
                    }
                }
            }

            if ((method.IsAbstract || method.IsVirtual)
                && !method.IsFinal && !method.DeclaringType.IsSealed)
            {
                il.Emit(OpCodes.Callvirt, method);
            }
            else
            {
                il.Emit(OpCodes.Call, method);
            }

            if (method.ReturnType == typeof(void))
            {
                il.Emit(OpCodes.Ldnull);
            }
            else if (method.ReturnType.IsValueType)
            {
                il.Emit(OpCodes.Box, method.ReturnType);
            }
            il.Emit(OpCodes.Ret);

            return (MethodDelegate)dm.CreateDelegate(typeof(MethodDelegate));
        }

        public static GetValueDelegate CreatePropertyGetter(PropertyInfo property)
        {
            Check.NotNull(property, nameof(property));

            if (!property.CanRead)
                return null;

            MethodInfo getMethod = property.GetGetMethod(true);

            DynamicMethod dm = new DynamicMethod("PropertyGetter" + Guid.NewGuid().ToString(), typeof(object),
                new Type[] { typeof(object) },
                property.DeclaringType, true);

            ILGenerator il = dm.GetILGenerator();

            if (!getMethod.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.EmitCall(OpCodes.Callvirt, getMethod, null);
            }
            else
                il.EmitCall(OpCodes.Call, getMethod, null);

            if (property.PropertyType.IsValueType)
                il.Emit(OpCodes.Box, property.PropertyType);

            il.Emit(OpCodes.Ret);

            return (GetValueDelegate)dm.CreateDelegate(typeof(GetValueDelegate));
        }

        public static GetValueDelegate CreatePropertyGetter(PropertyInfo vo,PropertyInfo property)
        {
            Check.NotNull(vo, nameof(vo));
            Check.NotNull(property, nameof(property));

            if (!vo.CanRead || !property.CanRead)
                return null; 

            //定义一个动态函数
            DynamicMethod dm = new DynamicMethod("vo_PropertyGetter" + Guid.NewGuid().ToString(), typeof(object),
                new Type[] { typeof(object) });

            ConstructorInfo constructor = vo.PropertyType.GetConstructor(Type.EmptyTypes);
            MethodInfo getVOMethod = vo.GetGetMethod(true);
            MethodInfo setVOMethod = vo.GetSetMethod(true);
            MethodInfo getMethod = property.GetGetMethod(true);

            ILGenerator il = dm.GetILGenerator();

            Label IL_001c = il.DefineLabel();
            Label IL_002f = il.DefineLabel();

            LocalBuilder local0 = il.DeclareLocal(vo.ReflectedType);
            LocalBuilder local1 = il.DeclareLocal(typeof(object));
            LocalBuilder local2 = il.DeclareLocal(typeof(bool));


            il.Emit(OpCodes.Nop);
            //将索引为 0 的参数加载到计算堆栈上
            il.Emit(OpCodes.Ldarg_0);
            //测试对象引用（O 类型）是否为特定类的实例。
            il.Emit(OpCodes.Isinst, vo.ReflectedType);
            //从计算堆栈的顶部弹出当前值并将其存储到索引 1 处的局部变量列表中。
            il.Emit(OpCodes.Stloc_0);
            //将索引 1 处的局部变量加载到计算堆栈上
            il.Emit(OpCodes.Ldloc_0);
            //对对象调用后期绑定方法，并且将返回值推送到计算堆栈上。
            il.EmitCall(OpCodes.Callvirt, getVOMethod, null);
            //将空引用（O 类型）推送到计算堆栈上
            il.Emit(OpCodes.Ldnull);
            //比较两个值。如果这两个值相等，则将整数值 1 (int32) 推送到计算堆栈上；否则，将 0 (int32) 推送到计算堆栈上。
            il.Emit(OpCodes.Ceq);
            //将整数值 0 作为 int32 推送到计算堆栈上
            il.Emit(OpCodes.Ldc_I4_0);
            //比较两个值。如果这两个值相等，则将整数值 1 (int32) 推送到计算堆栈上；否则，将 0 (int32) 推送到计算堆栈上。
            il.Emit(OpCodes.Ceq);
            //从计算堆栈的顶部弹出当前值并将其存储到索引 2 处的局部变量列表中。
            il.Emit(OpCodes.Stloc_2);
            //将索引 2 处的局部变量加载到计算堆栈上
            il.Emit(OpCodes.Ldloc_2);
            //如果 value 为 true、非空或非零，则将控制转移到目标指令（短格式）
            il.Emit(OpCodes.Brtrue_S, IL_001c);

            //将空引用（O 类型）推送到计算堆栈上
            il.Emit(OpCodes.Ldnull);
            //从计算堆栈的顶部弹出当前值并将其存储到索引 1 处的局部变量列表中。
            il.Emit(OpCodes.Stloc_1);
            //无条件地将控制转移到目标指令（短格式）
            il.Emit(OpCodes.Br_S, IL_002f);

            il.MarkLabel(IL_001c);
            //将索引 1 处的局部变量加载到计算堆栈上
            il.Emit(OpCodes.Ldloc_0);
            //对对象调用后期绑定方法，并且将返回值推送到计算堆栈上。
            il.EmitCall(OpCodes.Callvirt, getVOMethod, null);
            //对对象调用后期绑定方法，并且将返回值推送到计算堆栈上
            il.EmitCall(OpCodes.Callvirt, getMethod, null);

            if (property.PropertyType.IsValueType)
                il.Emit(OpCodes.Box, property.PropertyType);
            //从计算堆栈的顶部弹出当前值并将其存储到索引 1 处的局部变量列表中。
            il.Emit(OpCodes.Stloc_1);
            il.Emit(OpCodes.Br_S, IL_002f);

            il.MarkLabel(IL_002f);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ret);

            return (GetValueDelegate)dm.CreateDelegate(typeof(GetValueDelegate));
        }
         
        public static SetValueDelegate CreatePropertySetter(PropertyInfo property)
        {
            Check.NotNull(property, nameof(property));

            if (!property.CanWrite)
                return null;

            MethodInfo setMethod = property.GetSetMethod(true);

            DynamicMethod dm = new DynamicMethod("PropertySetter" + Guid.NewGuid().ToString(), null,
                new Type[] { typeof(object), typeof(object) },
                property.DeclaringType, true);

            ILGenerator il = dm.GetILGenerator();

            if (!setMethod.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
            }
            il.Emit(OpCodes.Ldarg_1);

            EmitCastToReference(il, property.PropertyType);
            if (!setMethod.IsStatic && !property.DeclaringType.IsValueType)
            {
                il.EmitCall(OpCodes.Callvirt, setMethod, null);
            }
            else
                il.EmitCall(OpCodes.Call, setMethod, null);

            il.Emit(OpCodes.Ret);

            return (SetValueDelegate)dm.CreateDelegate(typeof(SetValueDelegate));
        }

        public static SetValueDelegate CreatePropertySetter(PropertyInfo vo, PropertyInfo property)
        {
            Check.NotNull(vo, nameof(vo));
            Check.NotNull(property, nameof(property));

            if (!vo.CanWrite || !property.CanWrite)
                return null;

            ConstructorInfo constructor = vo.PropertyType.GetConstructor(Type.EmptyTypes);
            MethodInfo getVOMethod = vo.GetGetMethod(true);
            MethodInfo setVOMethod = vo.GetSetMethod(true);
            MethodInfo setMethod = property.GetSetMethod(true);
            //定义一个动态函数
            DynamicMethod dm = new DynamicMethod("vo_PropertySetter" + Guid.NewGuid().ToString(), 
                typeof(void),
                new Type[] { typeof(object), typeof(object) });

            ILGenerator il = dm.GetILGenerator();

            Label IL_001d = il.DefineLabel();

            LocalBuilder local0 = il.DeclareLocal(vo.ReflectedType);
            LocalBuilder local1 = il.DeclareLocal(typeof(bool));

            il.Emit(OpCodes.Nop);
            //将索引为 0 的参数加载到计算堆栈上
            il.Emit(OpCodes.Ldarg_0);
            //测试对象引用（O 类型）是否为特定类的实例。
            il.Emit(OpCodes.Isinst, vo.ReflectedType);
            //从计算堆栈的顶部弹出当前值并将其存储到索引 1 处的局部变量列表中。
            il.Emit(OpCodes.Stloc_0);
            //将索引 1 处的局部变量加载到计算堆栈上
            il.Emit(OpCodes.Ldloc_0);
            //对对象调用后期绑定方法，并且将返回值推送到计算堆栈上。
            il.EmitCall(OpCodes.Callvirt, getVOMethod, null);
            //将空引用（O 类型）推送到计算堆栈上
            il.Emit(OpCodes.Ldnull);
            //比较两个值。如果这两个值相等，则将整数值 1 (int32) 推送到计算堆栈上；否则，将 0 (int32) 推送到计算堆栈上。
            il.Emit(OpCodes.Ceq);
            //将整数值 0 作为 int32 推送到计算堆栈上
            il.Emit(OpCodes.Ldc_I4_0);
            //比较两个值。如果这两个值相等，则将整数值 1 (int32) 推送到计算堆栈上；否则，将 0 (int32) 推送到计算堆栈上。
            il.Emit(OpCodes.Ceq);
            //从计算堆栈的顶部弹出当前值并将其存储到索引 1 处的局部变量列表中。
            il.Emit(OpCodes.Stloc_1);
            //将索引 1 处的局部变量加载到计算堆栈上
            il.Emit(OpCodes.Ldloc_1);
            //如果 value 为 true、非空或非零，则将控制转移到目标指令（短格式）
            il.Emit(OpCodes.Brtrue_S, IL_001d);

            //将索引 1 处的局部变量加载到计算堆栈上
            il.Emit(OpCodes.Ldloc_0);
            //创建一个值类型的新对象或新实例，并将对象引用（O 类型）推送到计算堆栈上。
            il.Emit(OpCodes.Newobj, constructor);
            //对对象调用后期绑定方法，并且将返回值推送到计算堆栈上
            il.EmitCall(OpCodes.Callvirt, setVOMethod, null);

            il.MarkLabel(IL_001d);
            //将索引 1 处的局部变量加载到计算堆栈上
            il.Emit(OpCodes.Ldloc_0);
            //对对象调用后期绑定方法，并且将返回值推送到计算堆栈上。
            il.EmitCall(OpCodes.Callvirt, getVOMethod, null);
            //将索引为 1 的参数加载到计算堆栈上
            il.Emit(OpCodes.Ldarg_1);

            EmitCastToReference(il, property.PropertyType);
            //对对象调用后期绑定方法，并且将返回值推送到计算堆栈上
            il.EmitCall(OpCodes.Callvirt, setMethod, null);


            il.Emit(OpCodes.Ret);


            return (SetValueDelegate)dm.CreateDelegate(typeof(SetValueDelegate));
        }

        public static GetValueDelegate CreateFieldGetter(FieldInfo field)
        {
            Check.NotNull(field, nameof(field));

            DynamicMethod dm = new DynamicMethod("FieldGetter" + Guid.NewGuid().ToString(), typeof(object),
                new Type[] { typeof(object) },
                field.DeclaringType, true);

            ILGenerator il = dm.GetILGenerator();

            if (!field.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);

                EmitCastToReference(il, field.DeclaringType);  //to handle struct object

                il.Emit(OpCodes.Ldfld, field);
            }
            else
                il.Emit(OpCodes.Ldsfld, field);

            if (field.FieldType.IsValueType)
                il.Emit(OpCodes.Box, field.FieldType);

            il.Emit(OpCodes.Ret);

            return (GetValueDelegate)dm.CreateDelegate(typeof(GetValueDelegate));
        }

        public static SetValueDelegate CreateFieldSetter(FieldInfo field)
        {
            Check.NotNull(field, nameof(field));

            DynamicMethod dm = new DynamicMethod("FieldSetter" + Guid.NewGuid().ToString(), null,
                new Type[] { typeof(object), typeof(object) },
                field.DeclaringType, true);

            ILGenerator il = dm.GetILGenerator();

            if (!field.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
            }
            il.Emit(OpCodes.Ldarg_1);

            EmitCastToReference(il, field.FieldType);

            if (!field.IsStatic)
                il.Emit(OpCodes.Stfld, field);
            else
                il.Emit(OpCodes.Stsfld, field);
            il.Emit(OpCodes.Ret);

            return (SetValueDelegate)dm.CreateDelegate(typeof(SetValueDelegate));
        }

 
        private static void EmitCastToReference(ILGenerator il, Type type)
        {
            if (type.IsValueType)
                il.Emit(OpCodes.Unbox_Any, type);
            else
                il.Emit(OpCodes.Castclass, type);
        }
    }
}
