using HCI.SQLFramework.Model;
using HCI.SQLFramework.Tests.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Linq;
using Xunit;

namespace HCI.SQLFramework.Tests.Repositories
{
    public class ContextTests
    {
        private readonly DataContext<MySqlConnection> _context;

        public ContextTests()
        {
            //var con = "Server=class4u.cwkk80t8ge6w.sa-east-1.rds.amazonaws.com;Database=class4u_dev;Uid=root;Pwd=RDS_123*2019;";
            var con = "Server=localhost;Database=sakila;Uid=root;Pwd=root;";
            _context = new DataContext<MySqlConnection>(con);
        }


        [Fact]
        public void TestListKeyPair()
        {
            var result = _context.Query<KeyPair>("SELECT * FROM class4u_dev.KeyPair");
            Assert.NotEmpty(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void TestSaveOrUpdate(int value)
        {
            var model = new KeyPair
            {
                Value = value,
                Text = value.ToString()
            };
            var result = _context.SaveOrUpdate(model, true);
            Assert.True(result);
        }

        [Fact]
        public void TestDelete()
        {
            var collection = _context.Query<KeyPair>("SELECT * FROM class4u_dev.KeyPair where value > 2");
            foreach (var item in collection)
            {
                var result = _context.Remove(item);
                Assert.True(result);
            }
        }

        [Fact]
        public void TestWhere()
        {
            var query = _context.Where<KeyPair>(x => x.Text == "0");            
            Assert.NotEmpty(query);
        }
    }
}
