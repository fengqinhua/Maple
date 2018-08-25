using Maple.Core.Data.ModelConfiguration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Tests.Domain
{
    public class RoleEntityConfiguration : EntityConfiguration<Role>
    {
        public override void Configuration()
        {
            this.ToTable("TEST_ROLE");
        }
    }
}
