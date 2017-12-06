using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Infrastructure.Mapper
{
    /// <summary>
    /// 接口 : Mapper 注册信息
    /// </summary>
    public interface IMapperProfile
    {
        /// <summary>
        /// 获取Mapper注册信息顺序
        /// </summary>
        int Order { get; }
    }
}
