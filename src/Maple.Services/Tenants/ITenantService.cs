using Maple.Core.Domain.Tenants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Services.Tenants
{
    public partial interface ITenantService
    {
        IList<Tenant> GetAll();
    }
}
