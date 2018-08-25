using Maple.Core.Data;
using Maple.Core.Data.DataProviders;
using Maple.Core.Data.DataProviders.Internal;
using Maple.Core.Data.DataSettings;
using Maple.Core.Data.DbTranslators;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Maple.Core.Domain.Uow
{
    /// <summary>
    /// 缺省的工作单元
    /// </summary>
    public class MapleUnitOfWork : UnitOfWorkBase
    {
        protected readonly IDataSettingContainer dataSettingContainer = null;

        protected IDictionary<string, IDataProvider> ActiveDataProviders { get; }

        public IReadOnlyList<IDataProvider> GetAllActiveDataProvider()
        {
            return this.ActiveDataProviders.Values.ToImmutableList();
        }

        public MapleUnitOfWork(IDataSettingContainer dataSettingContainer)
        {
            this.dataSettingContainer = dataSettingContainer;
            this.ActiveDataProviders = new Dictionary<string, IDataProvider>();
        }

        protected override void CompleteUow()
        {
            foreach (var dataProvider in GetAllActiveDataProvider())
            {
                CompleteInDataProvider(dataProvider);
            }
        }

        protected override void DisposeUow()
        {
            foreach (var dataProvider in GetAllActiveDataProvider())
            {
                Release(dataProvider);
            }

            this.ActiveDataProviders.Clear();
        }

        protected virtual void CompleteInDataProvider(IDataProvider dataProvider)
        {
            dataProvider.DatabaseContext.Commit();
        }

        protected virtual void Release(IDataProvider dataProvider)
        {
            dataProvider.Dispose();
            //IocResolver.Release(dbContext);
        }

        public virtual IDataProvider GetOrCreateDataProvider(string dataSettingName)
        {
            var dataProviderKey = dataSettingName;
            if (string.IsNullOrEmpty(dataProviderKey))
                dataProviderKey = DataSetting.DefalutDataSettingName;

            IDataProvider dataProvider;
            if (this.Options.IsTransactional == true)
            {
                if (!ActiveDataProviders.TryGetValue(dataProviderKey, out dataProvider))
                {
                    //创建数据库连接驱动
                    dataProvider = getDataProvider(dataSettingName);
                    //启动数据库事务
                    dataProvider.DatabaseContext.BeginTransaction(this.Options.IsolationLevel);

                    ActiveDataProviders[dataProviderKey] = dataProvider;
                }
            }
            else
                dataProvider = getDataProvider(dataSettingName);    //创建数据库连接驱动

            return dataProvider;
        }

        private IDataProvider getDataProvider(string dataSettingName)
        {
            IDataSetting dataSetting = this.dataSettingContainer.GetDataSetting(dataSettingName);
            if (dataSetting == null)
                throw new MapleException($"未设置[{dataSettingName}]对应的数据库配置信息");

            IDbTranslator dbTranslator = getDbTranslator(dataSetting);
            var databaseContext = new InternalDatabaseContext(dataSetting, dbTranslator);
            return new InternalDataProvider(databaseContext);
        }

        private IDbTranslator getDbTranslator(IDataSetting dataSetting)
        {
            IDbTranslator dbTranslator;
            switch (dataSetting.DataSouceType)
            {
                case DataSouceType.MySQL:
                    dbTranslator = new MySQLTranslator();
                    break;
                case DataSouceType.Oracle:
                    dbTranslator = new OracleTranslator();
                    break;
                case DataSouceType.Sql2000:
                    dbTranslator = new Sql2000Translator();
                    break;
                case DataSouceType.Sql2005:
                    dbTranslator = new Sql2005Translator();
                    break;
                case DataSouceType.Sql2008:
                    dbTranslator = new Sql2008Translator();
                    break;
                case DataSouceType.Sqlite:
                    dbTranslator = new SqliteTranslator();
                    break;
                default:
                    throw new MapleException(string.Format("未知的数据源类型“{0}”。请核实数据源类型配置是否正确。", dataSetting.DataSouceType));
            }

            return dbTranslator;
        }
    }
}
