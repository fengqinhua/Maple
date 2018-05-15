using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Domain
{
    /// <summary>
    /// 定义实体类型的接口。系统中的所有实体都必须实现此接口。
    /// </summary>
    /// <typeparam name="TPrimaryKey">主键ID类型</typeparam>
    public interface IEntity<TPrimaryKey>
    {
        /// <summary>
        /// ID ，实体的唯一标识符
        /// </summary>
        TPrimaryKey Id { get; set; }
    }


    /// <summary>
    /// 以类型long为ID唯一标识符的实体类型接口
    /// </summary>
    public interface IEntity : IEntity<long>
    {

    }
}
