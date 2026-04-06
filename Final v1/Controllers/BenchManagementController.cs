using Final_v1.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Final_v1.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class BenchManagementController : Controller
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public BenchManagementController(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }
        /// <summary>
        /// 1. List of Bench Employees
        /// </summary>
        [HttpGet("bench-employees")]
        public async Task<IActionResult> GetBenchEmployees()
        {
            try
            {
                var result = new List<object>();
                using var con = _sqlConnectionFactory.CreateConnection();
                await con.OpenAsync();

                using var cmd = new SqlCommand("dbo.GetBenchEmployees", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Add(new
                    {
                        EmployeeID = reader["EmployeeID"],
                        EmployeeName = reader["FULLNAME"],
                        Skill = reader["SkillNAME"],
                        Experience = reader["ExperienceInYears"]
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 2. Percentage of Workforce on Bench
        /// </summary>
        [HttpGet("bench-percentage")]
        public async Task<IActionResult> GetBenchPercentage()
        {
            try
            {
                using var con = _sqlConnectionFactory.CreateConnection();
                await con.OpenAsync();

                using var cmd = new SqlCommand("dbo.GetBenchPercentage", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                var percentage = await cmd.ExecuteScalarAsync();
                return Ok(new { BenchPercentage = percentage });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 3. Skills of Bench Employees
        /// </summary>

        [HttpGet("bench-by-skill")]
        public async Task<IActionResult> GetBenchBySkill()
        {
            try
            {
                var result = new List<object>();
                using var con = _sqlConnectionFactory.CreateConnection();
                await con.OpenAsync();

                using var cmd = new SqlCommand("dbo.GetBenchBySkill", con)
                {

                    CommandType = CommandType.StoredProcedure
                };

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Add(new
                    {
                        SkillName = reader["SkillName"],
                        BenchCount = reader["BenchCount"]
                    });

                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        [HttpGet("bench-project-total")]
        public async Task<IActionResult> GetBenchProjectAndTotalCounts()
        {
            try
            {
                using var con = _sqlConnectionFactory.CreateConnection();
                await con.OpenAsync();

                using var cmd = new SqlCommand("dbo.GetBenchAndProjectCounts", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return Ok(new
                    {
                        BenchCount = reader["BenchCount"],
                        ProjectCount = reader["ProjectCount"],
                    });
                }
                return Ok(new { BenchCount = 0, ProjectCount = 0, TotalEmployees = 0 });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet("capacity-summary")]
        public async Task<IActionResult> GetProjectCapacitySummary()
        {
            try
            {
                var projects = new List<object>();
                int totalAssigned = 0;
                int totalCapacity = 0;

                using var con = _sqlConnectionFactory.CreateConnection();
                await con.OpenAsync();

                using var cmd = new SqlCommand("dbo.GetProjectCapacitySummary", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                using var reader = await cmd.ExecuteReaderAsync();

                // First result set: Project-wise details
                while (await reader.ReadAsync())
                {
                    projects.Add(new
                    {
                        ProjectName = reader["ProjectName"],
                        Assigned = reader["Assigned"],
                        Capacity = reader["Capacity"]
                    });
                }

                // Move to second result set: Totals
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    totalAssigned = Convert.ToInt32(reader["TotalAssigned"]);
                    totalCapacity = Convert.ToInt32(reader["TotalCapacity"]);
                }

                return Ok(new
                {
                    TotalAssigned = totalAssigned,
                    TotalCapacity = totalCapacity,
                    Projects = projects
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("active-project-dates")]
        public async Task<IActionResult> GetActiveProjectDates()
        {
            try
            {
                var projects = new List<object>();

                using var con = _sqlConnectionFactory.CreateConnection();
                await con.OpenAsync();

                using var cmd = new SqlCommand("dbo.GetActiveProjectDates", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    projects.Add(new
                    {
                        ProjectName = reader["ProjectName"],
                        StartDate = reader["StartDate"],
                        EndDate = reader["EndDate"]
                    });
                }

                return Ok(new { Projects = projects });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}