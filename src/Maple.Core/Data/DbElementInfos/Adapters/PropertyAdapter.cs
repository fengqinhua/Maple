//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Reflection;


//namespace Maple.Core.Data.DbElementInfos.Adapters
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    internal class PropertyAdapter : Adapter
//    {
//        private IDbField fieldInfo = null;
//        private Accessor.IPropertyAccessor propertyAccessor = null;
//        private string name = "";
//        private Type memberType = null;

//        public PropertyAdapter(Type entityType, PropertyInfo info)
//        {
//            //读取名称+类型
//            this.name = info.Name;
//            this.memberType = info.PropertyType;
//            //初始化字段访问接口
//            this.propertyAccessor = new Accessor.PropertyAccessor(entityType, info);
//            //初始化字段信息
//            var array = info.GetCustomAttributes(false);
//            foreach (Attribute o in array)
//            {
//                if (o is DbFieldAttribute)
//                {
//                    this.fieldInfo = o as IDbField;
//                    break;
//                }
//            }
            
//            if (this.fieldInfo == null)
//            {
//                //如果未定义DbFieldAttribute，则使用默认的
//                this.fieldInfo = new DbFieldAttribute()
//                {
//                    NameInStore = info.Name
//                };
//            }
//        }

//        public override string Name
//        {
//            get { return this.name; }
//        }

//        public override Type MemberType
//        {
//            get { return this.memberType; }
//        }

//        public override IDbField FieldInfo
//        {
//            get { return this.fieldInfo; }
//        }

//        public override object GetValue(object obj)
//        {
//            return this.propertyAccessor.GetValue(obj);
//        }

//        public override void SetValue(object obj, object value)
//        {
//            this.propertyAccessor.SetValue(obj, value);
//        }
//    }
//}
