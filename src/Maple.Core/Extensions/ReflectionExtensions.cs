//using Maple.Core.Reflection;
//using System;
//using System.Collections;
//using System.Reflection;

//namespace Maple.Core.Extensions
//{
//    public static class ReflectionExtensions
//    {
//        private static readonly Hashtable s_getterDict = Hashtable.Synchronized(new Hashtable(10240));
//        private static readonly Hashtable s_setterDict = Hashtable.Synchronized(new Hashtable(10240));
//        private static readonly Hashtable s_methodDict = Hashtable.Synchronized(new Hashtable(10240));


//        public static object FastNew(this Type instanceType)
//        {
//            if (instanceType == null)
//                throw new ArgumentNullException("instanceType");

//            CtorDelegate ctor = (CtorDelegate)s_methodDict[instanceType];
//            if (ctor == null)
//            {
//                ConstructorInfo ctorInfo = instanceType.GetConstructor(Type.EmptyTypes);
//                ctor = DynamicMethodFactory.CreateConstructor(ctorInfo);
//                s_methodDict[instanceType] = ctor;
//            }

//            return ctor();
//        }

//        public static object FastGetValue(this PropertyInfo propertyInfo, object obj)
//        {
//            if (propertyInfo == null)
//                throw new ArgumentNullException("propertyInfo");

//            GetValueDelegate getter = (GetValueDelegate)s_getterDict[propertyInfo];
//            if (getter == null)
//            {
//                getter = DynamicMethodFactory.CreatePropertyGetter(propertyInfo);
//                s_getterDict[propertyInfo] = getter;
//            }

//            return getter(obj);
//        }

//        public static void FastSetValue(this PropertyInfo propertyInfo, object obj, object value)
//        {
//            if (propertyInfo == null)
//                throw new ArgumentNullException("propertyInfo");

//            SetValueDelegate setter = (SetValueDelegate)s_setterDict[propertyInfo];
//            if (setter == null)
//            {
//                setter = DynamicMethodFactory.CreatePropertySetter(propertyInfo);
//                s_setterDict[propertyInfo] = setter;
//            }

//            setter(obj, value);
//        }
//    }
//}
