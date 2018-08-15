using Maple.Core.Data.DbMappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.ModelConfiguration
{
    public interface IEntityConfiguration
    {
        /// <summary>
        /// 执行映射配置
        /// </summary>
        void ExceConfiguration(IEntityMapper entityMapper);
    }
}
