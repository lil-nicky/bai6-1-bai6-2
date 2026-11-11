
using System.Configuration;
using System.Data.SqlClient;

namespace RetailSalesSystem.DAL
{
    public class Database
    {
        public static SqlConnection GetConnection()
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["SalesDB"].ConnectionString;

            return new SqlConnection(connectionString);
        }
    }
}
