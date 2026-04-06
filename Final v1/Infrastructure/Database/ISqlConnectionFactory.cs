using Microsoft.Data.SqlClient;

namespace Final_v1.Infrastructure.Database
{
    public interface ISqlConnectionFactory
    {
        SqlConnection CreateConnection();
        
    }
}
