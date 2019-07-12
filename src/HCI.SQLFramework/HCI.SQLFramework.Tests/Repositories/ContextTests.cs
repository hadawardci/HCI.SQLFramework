using HCI.SQLFramework.Model;
using HCI.SQLFramework.Tests.Entities;
using MySql.Data.MySqlClient;
using System.Linq;
using Xunit;

namespace HCI.SQLFramework.Tests.Repositories
{
    public class ContextTests
    {
        [Fact]
        public void ListKeyPair()
        {
            //var model = new KeyPair
            //{
            //     Text = "HCI.SQLFramework.Tests"
            //};

            // var ss = new MySqlConnection("");
            var con = "Server=class4u.cwkk80t8ge6w.sa-east-1.rds.amazonaws.com;Database=class4u_dev;Uid=root;Pwd=RDS_123*2019;";
            var context = new DataContext<MySqlConnection>(con);
                //"server=class4u.cwkk80t8ge6w.sa-east-1.rds.amazonaws.com;Uid=root;Pwd=RDS_123*2019;database=class4u_dev;persistsecurityinfo=True;connectiontimeout=60");
            //
            var result = context.Query<KeyPair>("SELECT * FROM class4u_dev.KeyPair").ToList();
            Assert.NotEmpty(result);
        }
    }
}
