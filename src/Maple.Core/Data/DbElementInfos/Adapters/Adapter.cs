using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;

namespace Maple.Core.Data.DbElementInfos.Adapters
{
    /// <summary>
    /// 负责获取字段的相关信息，以及SetValue GetValue (支持通过实体类的Attribute扩展 + Xml配置文件扩展)
    /// </summary>
    public abstract class Adapter
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// 字段类型
        /// </summary>
        public abstract Type MemberType { get; }
        /// <summary>
        /// 获取字段信息(数据库结构)
        /// </summary>
        /// <returns></returns>
        public abstract IDbField FieldInfo { get; }
        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public abstract void SetValue(object obj, object value);
        /// <summary>
        /// 读取属性值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract object GetValue(object obj);

        //[System.ComponentModel.DataAnnotations.Schema.NotMapped]
        /// <summary>
        /// 通过属性信息创建PropertyAdapter
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static Adapter NewPropertyAdapter(Type entityType, PropertyInfo pi)
        {
            return null;
            //PropertyAdapter adapter = new PropertyAdapter(entityType, pi);
            //adapter.CheckDefaultValue();
            //return adapter;
        }


        /// <summary>
        /// 确保存在DefaultValue
        /// </summary>
        protected void CheckDefaultValue()
        {
            //if (this.FieldInfo.Default == null)
            //{
            //    if (this.MemberType.IsEnum)
            //        this.FieldInfo.Default = 0;
            //    if (this.MemberType == typeof(string))
            //        this.FieldInfo.Default = "";
            //    if (this.MemberType == typeof(bool))
            //        this.FieldInfo.Default = false;
            //    if (this.MemberType == typeof(byte))
            //        this.FieldInfo.Default = (byte)0;
            //    if (this.MemberType == typeof(int))
            //        this.FieldInfo.Default = (int)0;
            //    if (this.MemberType == typeof(double))
            //        this.FieldInfo.Default = (double)0;
            //    if (this.MemberType == typeof(decimal))
            //        this.FieldInfo.Default = (decimal)0m;
            //    if (this.MemberType == typeof(float))
            //        this.FieldInfo.Default = (float)0f;
            //    if (this.MemberType == typeof(Guid))
            //        this.FieldInfo.Default = Guid.NewGuid();
            //    if (this.MemberType == typeof(DateTime))
            //        this.FieldInfo.Default = Timing.Clock.INVALID_DATETIME;
            //}
        }
    }

    public interface IDbField
    {
    }
}
