using HCI.EasyDapper.Model;
using HCI.EasyDapper.Tests.Entities;
using HCI.EasyDapper.XUnitTest.Entities.Sakila;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
//using System.Linq;
using Xunit;

namespace HCI.EasyDapper.Tests.Repositories
{
    public class ContextTests
    {
        private readonly DataContext<MySqlConnection> _context;
        //private readonly ISearchable<Film> _query;

        public ContextTests()
        {
            //var con = "Server=class4u.cwkk80t8ge6w.sa-east-1.rds.amazonaws.com;Database=class4u_dev;Uid=root;Pwd=RDS_123*2019;";
            var con = "Server=localhost;Database=sakila;Uid=root;Pwd=root;";
            _context = new DataContext<MySqlConnection>(con);
            //_context = new Searchable<Film>();
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
        public void TestWhereKeyPair()
        {

            var query = _context.Where<KeyPair>(x => x.Text == "0");
            Assert.NotEmpty(query);
        }

        [Fact]
        public void TestFilm_Query_Load()
        {

            var query = _context.Where<Film>(x => x.rating == "R");
            
            query = ((ISearchable<Film>)query).And(x => x.release_year >= 2006);
            query = ((ISearchable<Film>)query).And(x => x.title.StartsWith("A"));
            var result = ((ISearchable<Film>)query).Load(x=>x.description);
            Assert.NotEmpty(query);
        }

        [Fact]
        public void TestFilmFirstOrDefault()
        {

            var result = _context.FirstOrDefault<Film>(x => x.release_year == 2016 && x.title == "ACADEMY DINOSAUR");
            Assert.NotNull(result);
        }


        [Fact]
        public void TestAleatory()
        {
            var label = Expression.Label("sd");
            Expression labelReturn = Expression.Return(label);
            Func<Film, bool> exp = f => f.film_id < 2;
            var block = Expression.Block(labelReturn);
        }
    }
}
