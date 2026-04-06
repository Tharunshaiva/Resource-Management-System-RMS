using Final_v1.Application.DTO.EmployeeDTO;
using Final_v1.Application.DTO.RrportDTO;
using Final_v1.Domain.Interface;
using Final_v1.Infrastructure.Database;
using Final_v1.USP_StoredProcedure;
using Microsoft.Data.SqlClient;
using System.Data;
using SkillAvailabilityDTO = Final_v1.Application.DTO.RrportDTO.SkillAvailabilityDTO;

namespace Final_v1.Infrastructure.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public ReportRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        // Generic ADO.NET Executor
        private async Task<List<T>> ExecuteAsync<T>(string spName, Func<SqlDataReader, T> map, object? parameters = null)
        {
            var list = new List<T>();

            using var con = _sqlConnectionFactory.CreateConnection();
            using var cmd = new SqlCommand(spName, (SqlConnection)con)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
            {
                foreach (var p in parameters.GetType().GetProperties())
                {
                    cmd.Parameters.AddWithValue(p.Name, p.GetValue(parameters) ?? DBNull.Value);
                }
            }

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                list.Add(map(reader));
            return list;

        }

        // ---------------------------------------------------------
        // Map DTOs according to the corrected SPs
        // ---------------------------------------------------------

        public async Task<IEnumerable<ProjectOverlapDto>> CheckProjectOverlapAsync()
        {
            return await ExecuteAsync(
                StoredProcedure.CheckProjectOverlap,
                reader => new ProjectOverlapDto
                {
                    EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                    ProjectName = reader.GetString(reader.GetOrdinal("ProjectName")),
                    OverlapDays = reader.GetInt32(reader.GetOrdinal("OverlapDays"))
                }
            );
        }

        public async Task<IEnumerable<EmployeeAllocationHistoryDTO>> GetEmployeeHistoryAsync(int employeeId)
        {
            return await ExecuteAsync(
                StoredProcedure.GetEmployeeAllocationHistory,
                reader => new EmployeeAllocationHistoryDTO
                {
                    EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                    ProjectName = reader.GetString(reader.GetOrdinal("ProjectName")),
                    StartDate = reader.GetDateTime(reader.GetOrdinal("AllocationStartDate")),
                    EndDate = reader.GetDateTime(reader.GetOrdinal("AllocationEndDate")),
                    AllocationPercentage = reader.GetInt32(reader.GetOrdinal("AllocationPercentage"))
                },
                new { EmployeeID = employeeId }
            );
        }

        public async Task<IEnumerable<ProjectUtilizationDTO>> GetProjectUtilizationAsync()
        {
            return await ExecuteAsync(StoredProcedure.GetProjectUtilization,
                reader => new ProjectUtilizationDTO
                {
                    ProjectId = reader.GetInt32(reader.GetOrdinal("ProjectID")),
                    ProjectName = reader.GetString(reader.GetOrdinal("ProjectName")),
                    Utilization = reader.GetDecimal(reader.GetOrdinal("UtilizationPercentage"))
                }
            );
        }

        public async Task<IEnumerable<SkillAvailabilityDTO>> GetSkillAvailabilityAsync()
        {
            return await ExecuteAsync(
                StoredProcedure.GetSkillAvailability,
                reader => new SkillAvailabilityDTO
                {
                    SkillName = reader.GetString(reader.GetOrdinal("SkillName")),
                    AvailableCount = reader.GetInt32(reader.GetOrdinal("AvailableCount"))
                }
            );
        }
    }
}
