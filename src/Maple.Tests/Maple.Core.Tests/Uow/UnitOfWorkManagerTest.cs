using Maple.Core.Domain.Uow;
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
        public void UnitOfWorkManager_Begin()
        {
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider = new AsyncLocalCurrentUnitOfWorkProvider();
            IUnitOfWorkManager unitOfWorkManager = new UnitOfWorkManager(currentUnitOfWorkProvider);

            using (var one = unitOfWorkManager.Begin())
            {

                using (var two = unitOfWorkManager.Begin())
                {

                    two.Complete();
                }
                one.Complete();
            }



        }
    }
}
