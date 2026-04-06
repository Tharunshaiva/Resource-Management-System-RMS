using Microsoft.Data.SqlClient;

namespace Final_v1.Infrastructure.Database
{
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SqlConnection CreateConnection()
        {
            string cs = _configuration.GetConnectionString("DefaultConnection");
            return new SqlConnection(cs);
        }
    }
}
