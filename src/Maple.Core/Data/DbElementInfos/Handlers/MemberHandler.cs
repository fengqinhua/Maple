using System;
using Maple.Core.Data.DbElementInfos.Adapters;


namespace Maple.Core.Data.DbElementInfos.Handlers
{
    /// <summary>
    /// 
    /// </summary>
    public class MemberHandler
    {
        protected readonly Adapter adapter = null;
   
        #region 共有属性

        /// <summary>
        /// 存储元素在集合中的位置
        /// </summary>
        public readonly int Index;
        /// <summary>
        /// 字段类型
        /// </summary>
        public Type MemberType
        {
            get { return this.adapter.MemberType; }
        }

        /// <summary>
        /// 列名称(实体类)
        /// </summary>
        public string ColumnName
        {
            get { return this.adapter.Name; }
        }
   

        #endregion
        
        protected MemberHandler(Type entityType, Adapter adapter,int index)
        {
            this.adapter = adapter;
            Index = index;
        }

        public static MemberHandler NewObject(Type entityType, Adapter fi, int index)
        {
            if (fi.MemberType.IsEnum)
                return new EnumMemberHandler(entityType, fi, index);
            if (fi.MemberType == typeof(bool))
                return new BooleanMemberHandler(entityType, fi, index);
            return new MemberHandler(entityType, fi, index);
        }

        #region 共有方法

        public void SetValue(object obj, object value)
        {
            if (DBNull.Value != value)
            {
                InnerSetValue(obj, value);
            }
            else
            {
                //adapter.SetValue(obj, this.Default);
            }
        }

        protected virtual void InnerSetValue(object obj, object value)
        {
            adapter.SetValue(obj, value);
        }

        public virtual object GetValue(object obj)
        {
            return adapter.GetValue(obj);
        }

        #endregion

    }
}
