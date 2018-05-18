//using Maple.Core.Data.DataSettings;
//using System.Collections.Generic;
//using System.Data;

//namespace Maple.Core.Data.DbTranslators
//{
//    /// <summary>
//    /// 数据库信息翻译器
//    /// </summary>
//    public abstract class DbTranslator
//    {
//        protected readonly Dictionary<DbType, string> TypeNames = new Dictionary<DbType, string>();
//        /// <summary>
//        /// 数据库信息翻译器
//        /// </summary>
//        public DbTranslator()
//        {
//            TypeNames[DbType.String] = "VARCHAR";
//            TypeNames[DbType.DateTime] = "DATETIME";
//            TypeNames[DbType.Date] = "DATE";
//            TypeNames[DbType.Time] = "DATETIME";
//            TypeNames[DbType.Boolean] = "CHAR";//bool: char(1) [0 = False] [1=True] 
//            TypeNames[DbType.Byte] = "TINYINT";
//            TypeNames[DbType.SByte] = "";
//            TypeNames[DbType.Decimal] = "DECIMAL";
//            TypeNames[DbType.Double] = "FLOAT";
//            TypeNames[DbType.Single] = "REAL";
//            TypeNames[DbType.Int32] = "INT";
//            TypeNames[DbType.UInt32] = "INT";
//            TypeNames[DbType.Int64] = "BIGINT";
//            TypeNames[DbType.UInt64] = "BIGINT";
//            TypeNames[DbType.Int16] = "SMALLINT";
//            TypeNames[DbType.UInt16] = "SMALLINT";
//            TypeNames[DbType.Guid] = "UNIQUEIDENTIFIER";
//            TypeNames[DbType.Binary] = "BINARY";
//        }
//        /// <summary>
//        /// 0、数据库类型
//        /// </summary>
//        public virtual DataSouceType DataSouceType { get { return DataSouceType.Unknown; } }
//        /// <summary>
//        /// 1、获取数据库对象工厂名称(具体配置方法参见Coding Log 2014.03.20-001)
//        /// </summary>
//        /// <returns></returns>
//        public abstract string GetProviderFactoryName();
//        /// <summary>
//        /// 2、创建数据库驱动
//        /// </summary>
//        /// <param name="dbc">数据库信息</param>
//        /// <param name="dbProviderFactoryName">数据库对象工厂名称</param>
//        /// <returns></returns>
//        public abstract DbDriver CreateDbDriver(DataBaseConnection dbc, string dbProviderFactoryName);
//        /// <summary>
//        /// 3、获得测试数据库连接的sql
//        /// </summary>
//        /// <returns></returns>
//        public abstract SqlStatement GetTestConnectionSql();
//        /// <summary>
//        /// 4、获取参数连接符
//        /// </summary>
//        /// <returns></returns>
//        protected abstract char Connector { get; }
//        /// <summary>
//        /// 5、引用开始
//        /// </summary>
//        protected virtual string OpenQuote
//        {
//            get { return string.Empty; }
//        }
//        /// <summary>
//        /// 6、引用结束
//        /// </summary>
//        protected virtual string CloseQuote
//        {
//            get { return string.Empty; }
//        }

//        /// <summary>
//        /// 包装表名称或字段
//        /// </summary>
//        /// <param name="name"></param>
//        /// <returns></returns>
//        public string Quote(string name)
//        {
//            return string.Format("{0}{2}{1}", OpenQuote, CloseQuote, name);
//        }
//        /// <summary>
//        /// 包装参数
//        /// </summary>
//        /// <param name="name"></param>
//        /// <returns></returns>
//        public string QuoteParameter(string name)
//        {
//            return string.Format("{0}{1}", Connector, name);
//        }
//        public string QuoteParameter(object name)
//        {
//            return string.Format("{0}{1}", Connector, name);
//        }
//    }
//}
