using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace EasyDapper.Model
{
    public class DataContext : DataContext<SqlConnection>
    {
        public DataContext(string connectionString, bool isTransaction = false) : base(connectionString, isTransaction)
        {
        }
    }
}
