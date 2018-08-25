using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Tests.Domain
{
    public static class EntityBuilder
    {
        public static IdWorker worker = new IdWorker(0);

        public static Role CreatNewRole(long id = -1, bool withValue = true)
        {
            Role role = new Role();
            if (id == -1)
                role.Id = worker.NextId();
            else
                role.Id = id;

            role.CreatorUserId = 9527;
            role.DeleterUserId = 9527;
            role.LastModifierUserId = 9527;
            role.DeletionTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            role.LastModificationTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            role.CreationTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            if (withValue)
            {
                role.IsDeleted = true;
                role.Name = "Maple";
                role.OrgId = 307;
                role.TenantId = 3306;
            }

            return role;
        }

        public static User CreatNewUser(long id = -1, bool withValue = true)
        {
            User user = new User();
            user.Address = new Address();
            if (id == -1)
                user.Id = worker.NextId();
            else
                user.Id = id;

            user.CreatorUserId = 9527;
            user.DeleterUserId = 9527;
            user.LastModifierUserId = 9527;
            user.DeletionTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            user.LastModificationTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            user.CreationTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            if (withValue)
            {
                user.Address.CityId = Guid.NewGuid();
                user.Address.Number = 109;
                user.Address.Street = "珞狮南路";
                user.Age = 10;
                user.Height = 175;
                user.IsDeleted = true;
                user.Name = "Maple";
                user.OrgId = 307;
                user.Six = Six.Woman;
                user.TenantId = 3306;
            }

            return user;
        }
    }



}
