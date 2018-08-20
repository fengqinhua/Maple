using Maple.Data.PerformanceTests.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;

namespace Maple.Data.PerformanceTests
{
    public class EFDataTest
    {
        private MyDbContext dbContext = null;
        public EFDataTest()
        {
            dbContext = new MyDbContext();
        }

        public void Insert(User user)
        {
            dbContext.Persons.Add(user);
            dbContext.SaveChanges();
        }

        public void SelectAll()
        {
            dbContext.Persons.ToList();
        }

        public void Single(long key)
        {
            var data = dbContext.Persons.Where(f => f.Id == key).FirstOrDefault();
        }

        public void DeleteAll()
        {
            string query = "DELETE FROM TEST_USER;";
            dbContext.Database.ExecuteSqlCommand(query);
        }
    }

    public class MyDbContext : DbContext
    {
        public DbSet<User> Persons { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseMySQL("Server=127.0.0.1;port=3306;Database=mapleleaf;Uid=root;Pwd=root;charset=utf8;SslMode=none;");//配置连接字符串
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var etPerson = modelBuilder.Entity<User>().ToTable("TEST_USER");
            etPerson.Property(e => e.Name).IsRequired().HasColumnName("USERNAME").HasMaxLength(500);
        }
    }
}
