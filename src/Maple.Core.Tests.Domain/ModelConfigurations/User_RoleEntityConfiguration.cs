using Maple.Core.Data.ModelConfiguration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Tests.Domain
{
    public class User_RoleEntityConfiguration : EntityConfiguration<User_Role>
    {
        public override void Configuration()
        {
            this.ToTable("TEST_USER_ROLE");
        }
    }
}
