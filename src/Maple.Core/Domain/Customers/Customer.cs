using Maple.Core.Domain.Entities;
using Maple.Core.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Domain.Customers
{
    public partial class Customer : FullAuditedEntity, IAggregateRoot
    {
    }
}
