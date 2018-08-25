using Maple.Core.Domain.Values;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Tests.Domain
{
    public class Address : ValueObject<Address>
    {
        public Guid CityId { get; set; }
        public string Street { get; set; }
        public int Number { get; set; }
    }
}
