using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.Conditions
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
    public class ShowStringAttribute : Attribute
    {
        private readonly string _showString;

        public string ShowString
        {
            get { return _showString; }
        }

        public ShowStringAttribute(string showString)
        {
            _showString = showString;
        }
    }
}
