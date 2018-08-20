using Maple.Core.Data.ModelConfiguration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Tests.Domain
{
    public class UserEntityConfiguration : EntityConfiguration<User>
    {
        public override void Configuration()
        {
            this.ToTable("TEST_USER");

            this.Property(f => f.Name)
                .ToAllowsNulls(true)
                .ToColumnName("USERNAME")
                .ToDbSize(500);

            this.Property(f => f.Address.Number)
                .ToAllowsNulls(true)
                .ToColumnName("ADDRESS_NUM");
        }
    }
}
