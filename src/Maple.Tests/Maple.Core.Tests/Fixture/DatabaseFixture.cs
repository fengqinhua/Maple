using Autofac;
using Autofac.Core;
using Maple.Core.Data;
using Maple.Core.Data.DataProviders;
using Maple.Core.Data.DataSettings;
using Maple.Core.Data.DbTranslators;
using Maple.Core.Data.ModelConfiguration;
using Maple.Core.Domain.Repositories;
using Maple.Core.Domain.Uow;
using Maple.Core.Infrastructure;
using Maple.Core.Tests.Domain;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Xunit;

namespace Maple.Core.Tests.Fixture
{
    public class DatabaseFixture : IDisposable
    {
        public DatabaseFixture()
        {
            //设置SQL默认设置
            DatabaseCommon.DbSqlNeedLog = false;
            //设置数据库驱动
            DbDriveFactories.SetFactory<System.Data.SqlClient.SqlClientFactory>(new Sql2000Translator().ProviderInvariantName);
            DbDriveFactories.SetFactory<MySql.Data.MySqlClient.MySqlClientFactory>(new MySQLTranslator().ProviderInvariantName);
            DbDriveFactories.SetFactory<System.Data.SQLite.SQLiteFactory>(new SqliteTranslator().ProviderInvariantName);
            //设置实体映射
            EntityConfigurationFactory.SetConfiguration(typeof(User), typeof(UserEntityConfiguration));
            EntityConfigurationFactory.SetConfiguration(typeof(User_Role), typeof(User_RoleEntityConfiguration));
            EntityConfigurationFactory.SetConfiguration(typeof(Role), typeof(RoleEntityConfiguration));
            //初始化Engine
            var engine = new OnlyIocEngine((builder) =>
            {
                //InstancePerDependency         瞬态
                //SingleInstance                单例
                //InstancePerLifetimeScope      基于线程或者请求的单例
                //InstancePerRequest            每个HTTP请求单例

                //数据库连接配置容器  （单例）
                builder.RegisterType<DefaultDataSettingContainer>().As<IDataSettingContainer>().SingleInstance();
                //Data持久层相关      （瞬态）
                builder.RegisterType<SimpleDataProviderFactory>().Named<IDataProviderFactory>("SimpleDataProviderFactory").InstancePerDependency();
                builder.RegisterType<UnitOfWorkDataProviderFactory>().As<IDataProviderFactory>().InstancePerDependency();
                //UOW工作单元相关     （瞬态）
                builder.RegisterType<AsyncLocalCurrentUnitOfWorkProvider>().As<ICurrentUnitOfWorkProvider>().InstancePerDependency();
                builder.RegisterType<UnitOfWorkManager>().As<IUnitOfWorkManager>().InstancePerDependency();
                builder.RegisterType<MapleUnitOfWork>().As<IUnitOfWork>().InstancePerDependency();
                //Repository仓储      （基于线程或者请求的单例）
                //builder.RegisterGeneric(typeof(MapleRepositoryBase<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
                builder.RegisterType<UserRepository>().As<IRepository<User>>().InstancePerLifetimeScope();
                //builder.RegisterType<UserRepository>().As<IRepository<User>>()
                //                                      .WithParameter(ResolvedParameter.ForNamed<IDataProviderFactory>("SimpleDataProviderFactory"))
                //                                      .InstancePerLifetimeScope();
                builder.RegisterType<RoleRepository>().As<IRepository<Role>>().InstancePerLifetimeScope();



            });
            EngineContext.Replace(engine);

            EngineContext.Current.Resolve<IDataSettingContainer>().AddOrUpdate(new DataSetting(DataSetting.DefalutDataSettingName, DataSouceType.MySQL, "Server=127.0.0.1;port=3306;Database=mapleleaf;Uid=root;Pwd=root;charset=utf8;SslMode=none;"));
        }

        public void Dispose()
        {

        }
    }

    [CollectionDefinition("Database connection")]
    public class DatabaseConnection : ICollectionFixture<DatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
