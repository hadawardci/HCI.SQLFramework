using EasyDapper.Contracts;
using EasyDapper.Model;
using HCI.EasyDapper.Tests.Entities;
using HCI.SQLFramework.XUnitTest.Entities.Enterprise;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Security;
using System.Text;
using Xunit;

namespace HCI.SQLFramework.XUnitTest.Repositories
{
    public class SystemLogRepository
    {
        private readonly static string _connectionString = "data source=rdsllip.cw0jgbfjcj8i.sa-east-1.rds.amazonaws.com;initial catalog=EnterpriseLLIP_Dev;persist security info=True;user id=desenvllip;password=@llip2016;connectretrycount=10;MultipleActiveResultSets=True;";
        private readonly static string _globalConnectionString = "data source=rdsllip.cw0jgbfjcj8i.sa-east-1.rds.amazonaws.com;initial catalog=Global_Dev;persist security info=True;user id=desenvllip;password=@llip2016;connectretrycount=10;MultipleActiveResultSets=True;";

        [Theory]
        [InlineData("AAAA")]
        [InlineData("TEST")]
        [InlineData("AMG/CP")]
        public void Save_TrademarkRPI(string rpiNumber)
        {
            var result = false;
            var entity = new TrademarkRPI
            {
                Date = DateTime.Now,
                IsCompleted = false,
                IsSent = true,
                RPINumber = rpiNumber
            };
            using (IDataContext context = new DataContext(_globalConnectionString))
            {
                result = context.SaveOrUpdate(entity);
            }
            Assert.True(result);
        }

        [Theory]
        [InlineData("TEST", "TESTE", "HCI.SQLFramework.XUnitTest")]
        public void Save(string level, string message, string application)
        {
            SystemLog entity = new SystemLog
            {
                Level = level,
                Logged = DateTime.Now,
                Message = message,
                Application = application
            };
            using (IDataContext context = new DataContext(_connectionString))
            {
                context.SaveOrUpdate(entity);
            }

            Assert.NotEqual(0, entity.Id);

        }

    }
}
