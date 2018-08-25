using Maple.Core.Domain.Repositories;
using Maple.Core.Domain.Uow;
using Maple.Core.Infrastructure;
using Maple.Core.Tests.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Maple.Core.Tests.Uow
{
    [Collection("Database connection")]
    public class UnitOfWorkManagerTest
    {
        private Fixture.DatabaseFixture fixture;
        public UnitOfWorkManagerTest(Fixture.DatabaseFixture fixture)
        {
            this.fixture = fixture;
        }
        [Fact]
        public void UnitOfWorkManager_Rollback22()
        {

        }

        [Fact]
        public void UnitOfWorkManager_Rollback()
        {
            var repositoryUser = EngineContext.Current.Resolve<IRepository<User>>();
            var repositoryRole = EngineContext.Current.Resolve<IRepository<Role>>();
            var unitOfWorkManager = EngineContext.Current.Resolve<IUnitOfWorkManager>();

            using (var one = unitOfWorkManager.Begin())
            {
                repositoryUser.Delete(f => 1 == 1);
                repositoryUser.Insert(EntityBuilder.CreatNewUser());
                repositoryUser.Insert(EntityBuilder.CreatNewUser());

                using (var two = unitOfWorkManager.Begin())
                {
                    repositoryRole.Insert(EntityBuilder.CreatNewRole());
                    repositoryRole.Insert(EntityBuilder.CreatNewRole());
                    repositoryRole.Insert(null);

                    two.Complete();
                }
                one.Complete();
            }

            Assert.NotEqual(repositoryUser.Count(), 0);
            Assert.NotEqual(repositoryRole.Count(), 0);

        }
    }
}
