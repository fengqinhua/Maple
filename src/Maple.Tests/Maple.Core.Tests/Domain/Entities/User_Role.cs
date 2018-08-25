using Maple.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Tests.Domain
{
    public class User_Role : Entity<long>, IMustHaveTenant
    {
        public long UserID { get; set; }
        public long RoleID { get; set; }

        public long OrgId { get; set; }
        public long TenantId { get; set; }
    }
}
