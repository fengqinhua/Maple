using System;
using System.Collections.Generic;
using System.Reflection;
using Maple.Core.Extensions;

namespace Maple.Core.Domain
{
    /// <summary>
    /// 以类型long为ID唯一标识符的实体类型基类
    /// </summary>
    [Serializable]
    public abstract class Entity : Entity<long>, IEntity
    {

    }

    /// <summary>
    /// 实体类型基类
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    [Serializable]
    public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey>
    {
        /// <summary>
        /// ID ，实体的唯一标识符
        /// </summary>
        public virtual TPrimaryKey Id { get; set; }

        /// <summary>
        /// 判断该实体是否为临时的（未保存至数据库）
        /// </summary>
        /// <returns></returns>
        public virtual bool IsTransient()
        {
            if (EqualityComparer<TPrimaryKey>.Default.Equals(Id, default(TPrimaryKey)))
            {
                return true;
            }

            //Workaround for EF Core since it sets int/long to min value when attaching to dbcontext
            if (typeof(TPrimaryKey) == typeof(int))
            {
                return Convert.ToInt32(Id) <= 0;
            }

            if (typeof(TPrimaryKey) == typeof(long))
            {
                return Convert.ToInt64(Id) <= 0;
            }

            return false;
        }


        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity<TPrimaryKey>))
            {
                return false;
            }

            //Same instances must be considered as equal
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            //Transient objects are not considered as equal
            var other = (Entity<TPrimaryKey>)obj;
            if (IsTransient() && other.IsTransient())
            {
                return false;
            }

            //Must have a IS-A relation of types or must be same type
            var typeOfThis = GetType();
            var typeOfOther = other.GetType();
            if (!typeOfThis.GetTypeInfo().IsAssignableFrom(typeOfOther) && !typeOfOther.GetTypeInfo().IsAssignableFrom(typeOfThis))
            {
                return false;
            }

            if (this is IMayHaveOrg && other is IMayHaveOrg &&
                this.As<IMayHaveOrg>().OrgId != other.As<IMayHaveOrg>().OrgId)
            {
                return false;
            }

            if (this is IMustHaveOrg && other is IMustHaveOrg &&
                this.As<IMustHaveOrg>().OrgId != other.As<IMustHaveOrg>().OrgId)
            {
                return false;
            }

            return Id.Equals(other.Id);
        }


        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }


        public static bool operator ==(Entity<TPrimaryKey> left, Entity<TPrimaryKey> right)
        {
            if (Equals(left, null))
            {
                return Equals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(Entity<TPrimaryKey> left, Entity<TPrimaryKey> right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"[{GetType().Name} {Id}]";
        }
    }
}
