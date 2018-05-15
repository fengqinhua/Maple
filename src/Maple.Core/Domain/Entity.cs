using System;
using System.Collections.Generic;
using System.Reflection;

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

            //Must have a IS-A relation of types or must be same type
            var other = (Entity<TPrimaryKey>)obj;
            var typeOfThis = GetType();
            var typeOfOther = other.GetType();
            if (!typeOfThis.GetTypeInfo().IsAssignableFrom(typeOfOther) && !typeOfOther.GetTypeInfo().IsAssignableFrom(typeOfThis))
            {
                return false;
            }

            if (this is IMustHaveTenant && other is IMustHaveTenant &&
                this.As<IMustHaveTenant>().TenantId != other.As<IMustHaveTenant>().TenantId)
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
