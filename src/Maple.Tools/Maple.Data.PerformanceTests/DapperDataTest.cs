using Dapper;
using Maple.Data.PerformanceTests.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Maple.Data.PerformanceTests
{
    public class DapperDataTest
    {
        private IDbConnection conn = null;

        public DapperDataTest()
        {
            conn = new MySql.Data.MySqlClient.MySqlConnection("Server=127.0.0.1;port=3306;Database=mapleleaf;Uid=root;Pwd=root;charset=utf8;SslMode=none;");
        }


        public void Insert(User user)
        {
            string query = "INSERT INTO TEST_USER (Id,USERNAME,Age,Height,Six,ExtensionData,OrgId,TenantId,IsDeleted,DeleterUserId,DeletionTime,LastModificationTime,LastModifierUserId,CreationTime,CreatorUserId) VALUES (@Id,@Name,@Age,@Height,@Six,@ExtensionData,@OrgId,@TenantId,@IsDeleted,@DeleterUserId,@DeletionTime,@LastModificationTime,@LastModifierUserId,@CreationTime,@CreatorUserId);";
            //对对象进行操作
            conn.Execute(query, user);
        }


        public void SelectAll()
        {
            //string query = "SELECT * FROM TEST_USER";
            string query = "SELECT Id,USERNAME,Age,Height,Six,ExtensionData,OrgId,TenantId,IsDeleted,DeleterUserId,DeletionTime,LastModificationTime,LastModifierUserId,CreationTime,CreatorUserId FROM TEST_USER  ORDER BY Id ASC";
            //无参数查询，返回列表，带参数查询和之前的参数赋值法相同。
            var data = conn.Query<User>(query).ToList();
        }

        public void Single(long key)
        {
            string query = "SELECT Id,USERNAME,Age,Height,Six,ExtensionData,OrgId,TenantId,IsDeleted,DeleterUserId,DeletionTime,LastModificationTime,LastModifierUserId,CreationTime,CreatorUserId FROM TEST_USER WHERE Id = @id ORDER BY Id ASC";

            var data = conn.Query<User>(query, new { Id = key }).SingleOrDefault();
        }

        public void DeleteAll()
        {
            string query = "DELETE FROM TEST_USER;";
            //对对象进行操作
            conn.Execute(query);
        }

    }
}
