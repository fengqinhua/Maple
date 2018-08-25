using Maple.Core.Reflection;
using Maple.Core.Tests.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace Maple.Core.Tests.Reflection
{
    public class DynamicMethodFactoryTests
    {
        [Fact]
        public void DynamicMethodFactory_CreatePropertyGetter()
        {
            object obj = null;
            int value = 5;
            PropertyInfo vo = typeof(User).GetProperties().FirstOrDefault(f => f.Name == "Age");
            GetValueDelegate getValueDelegate = DynamicMethodFactory.CreatePropertyGetter(vo);
            User user = new User();
            user.Age = value;

            if (getValueDelegate != null)
                obj = getValueDelegate(user);
            Assert.Equal(value, obj);
        }

    
        [Fact]
        public void DynamicMethodFactory_CreatePropertySetter()
        {
            int value = 5;
            PropertyInfo vo = typeof(User).GetProperties().FirstOrDefault(f => f.Name == "Age");
            SetValueDelegate setValueDelegate = DynamicMethodFactory.CreatePropertySetter(vo);
            User user = new User();
            if (setValueDelegate != null)
                setValueDelegate(user, value);
            Assert.Equal(user.Age, value);
        }

        [Fact]
        public void DynamicMethodFactory_CreatePropertySetter_VO_STRING()
        {
            string value = "ABC";
            PropertyInfo vo = typeof(User).GetProperties().FirstOrDefault(f => f.Name == "Address");
            PropertyInfo property = typeof(Address).GetProperties().FirstOrDefault(f => f.Name == "Street");

            SetValueDelegate setValueDelegate = DynamicMethodFactory.CreatePropertySetter(vo, property);
            User user = new User();
            if (setValueDelegate != null)
                setValueDelegate(user, value);
      
            Assert.True(user.Address != null);
            Assert.Equal(user.Address.Street, value);

        }

        [Fact]
        public void DynamicMethodFactory_CreatePropertySetter_VO_INT()
        {
            int value = 12;
            PropertyInfo vo = typeof(User).GetProperties().FirstOrDefault(f => f.Name == "Address");
            PropertyInfo property = typeof(Address).GetProperties().FirstOrDefault(f => f.Name == "Number");

            SetValueDelegate setValueDelegate = DynamicMethodFactory.CreatePropertySetter(vo, property);
            User user = new User();
            if (setValueDelegate != null)
                setValueDelegate(user, value);

            Assert.True(user.Address != null);
            Assert.Equal(user.Address.Number, value);

        }

        [Fact]
        public void DynamicMethodFactory_CreatePropertyGetter_VO_STRING()
        {
            object obj =null;
            string value = "ABC";
            PropertyInfo vo = typeof(User).GetProperties().FirstOrDefault(f => f.Name == "Address");
            PropertyInfo property = typeof(Address).GetProperties().FirstOrDefault(f => f.Name == "Street");

            GetValueDelegate getValueDelegate = DynamicMethodFactory.CreatePropertyGetter(vo, property);
            User user = new User();
            user.Address = new Address();
            user.Address.Street = value;

            if (getValueDelegate != null)
                obj= getValueDelegate(user);

            Assert.Equal(obj, value);

        }

        [Fact]
        public void DynamicMethodFactory_CreatePropertyGetter_VO_INT()
        {
            object obj = null;
            int value = 12;
            PropertyInfo vo = typeof(User).GetProperties().FirstOrDefault(f => f.Name == "Address");
            PropertyInfo property = typeof(Address).GetProperties().FirstOrDefault(f => f.Name == "Number");

            GetValueDelegate getValueDelegate = DynamicMethodFactory.CreatePropertyGetter(vo, property);
            User user = new User();
            user.Address = new Address();
            user.Address.Number = value;

            if (getValueDelegate != null)
                obj = getValueDelegate(user);

            Assert.Equal(obj, value);

        }
    }
}
