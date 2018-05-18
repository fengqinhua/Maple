using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maple.Core.Data.DbElementInfos.Adapters;

namespace Maple.Core.Data.DbElementInfos.Handlers
{
    public class BooleanMemberHandler : MemberHandler
    {
        public BooleanMemberHandler(Type entityType, Adapter adapter, int index)
            : base(entityType, adapter, index)
        {
        }

        protected override void InnerSetValue(object obj, object value)
        {
            bool v = false;
            if (value != null && value.ToString() == "1")
                v = true;
            base.adapter.SetValue(obj, v);
        }

    }
}
