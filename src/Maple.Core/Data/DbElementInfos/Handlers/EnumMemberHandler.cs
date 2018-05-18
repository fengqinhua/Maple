using System;
using Maple.Core.Data.DbElementInfos.Adapters;

namespace Maple.Core.Data.DbElementInfos.Handlers
{
    public class EnumMemberHandler : MemberHandler
    {
        public EnumMemberHandler(Type entityType, Adapter adapter, int index)
            : base(entityType, adapter, index)
        {
        }

        protected override void InnerSetValue(object obj, object value)
        {
            int b = 0;
            if (value != null)
                b = Convert.ToInt32(value);
            base.adapter.SetValue(obj, b);
        }
    }
}
