using Maple.Core.Domain.Entities;
using Maple.Core.Domain.Entities.Auditing;
using Maple.Core.Domain.Values;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Data.PerformanceTests.Entities
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

        //public Address Address { get; set; }
    }

    public class Address : ValueObject<Address>
    {
        public Guid CityId { get; set; }
        public string Street { get; set; }
        public int Number { get; set; }
    }

    public enum Six
    {
        Man,
        Woman
    }
}
