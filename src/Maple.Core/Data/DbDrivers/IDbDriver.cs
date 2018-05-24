using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.DbDrivers
{
    /// <summary>
    /// 数据库驱动
    /// </summary>
    public interface IDbDriver
    {
        void BeginTransaction();
        void Commit();
        void Rollback();
        void Rollback(Exception RollBackException);
    }
}
