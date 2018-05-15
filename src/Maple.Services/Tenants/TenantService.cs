using Maple.Core.Domain.Tenants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Services.Tenants
{
    public partial class TenantService : ITenantService
    {
        public IList<Tenant> GetAll()
        {
            return new List<Tenant>()
            {
                new Tenant()
                {
                     Id=1,
                     Name="Default"
                }
            };
        }


    }
}
