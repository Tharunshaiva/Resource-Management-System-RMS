using Final_v1.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Final_v1.Controllers
{
    public class CountController : Controller
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public CountController(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        /// <summary>
        /// Get counts of active employees and active projects
        /// </summary>
        [HttpGet("count")]
        public async Task<IActionResult> GetActiveCounts()
        {
            try
            {
                using var con = _sqlConnectionFactory.CreateConnection();
                await con.OpenAsync();

                using var cmd = new SqlCommand("dbo.CountActiveEmployeesAndProjects", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Execute the SP
                using var reader = await cmd.ExecuteReaderAsync();

                int activeEmployeeCount = 0;
                int activeProjectCount = 0;

                // First result set: Active Employee Count
                if (await reader.ReadAsync())
                {
                    activeEmployeeCount = Convert.ToInt32(reader["ActiveEmployeeCount"]);
                }

                // Move to second result set: Active Project Count
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    activeProjectCount = Convert.ToInt32(reader["ActiveProjectCount"]);
                }

                return Ok(new
                {
                    ActiveEmployeeCount = activeEmployeeCount,
                    ActiveProjectCount = activeProjectCount
                });
            }
            catch (Exception ex)
            {
                // Log exception as needed
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
