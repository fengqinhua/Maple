using Maple.Core.Domain.Entities;
using Maple.Core.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Tests.Domain
{
    public class User : FullAuditedEntity, IAggregateRoot, IMustHaveOrg, IMustHaveTenant, IExtendableObject
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public double Height { get; set; }
        public Six Six { get; set; }

        public string ExtensionData { get; set; }
        public long OrgId { get; set; }
        public long TenantId { get; set; }

        public Address Address { get; set; }
    }

    public enum Six
    {
        Man,
        Woman
    }
}
