using Final_v1.Application.DTO.AllocationDTO;
using Final_v1.Application.DTO.ProjectDTO;
using Final_v1.Application.DTO.RrportDTO;
using Final_v1.Domain.Interface;
using Final_v1.Infrastructure.Database;
using Final_v1.USP_StoredProcedure;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using ProjectEmployeeDTO = Final_v1.Application.DTO.RrportDTO.ProjectEmployeeDTO;

namespace Final_v1.Infrastructure.Repositories
{
    public class AllocationRepository : IAllocationRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public AllocationRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<(int ResultCode, string Message)> AssignEmployeeAsync(AllocationResponseDto dto, CancellationToken ct)
        {
            try
            {
                await using var con = _sqlConnectionFactory.CreateConnection();
                await con.OpenAsync(ct);

                await using var cmd = new SqlCommand(StoredProcedure.AddEmployeeSkill, con);
                cmd.CommandType = CommandType.StoredProcedure;

                // Input parameters
                cmd.Parameters.Add(new SqlParameter("@EmployeeName", dto.EmployeeName));
                cmd.Parameters.Add(new SqlParameter("@SkillName", dto.SkillName));

                // Output parameters
                var resultCode = new SqlParameter("@ResultCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                var message = new SqlParameter("@Message", SqlDbType.NVarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };

                cmd.Parameters.Add(resultCode);
                cmd.Parameters.Add(message);

                // Execute
                await cmd.ExecuteNonQueryAsync(ct);

                return ((int)resultCode.Value, message.Value?.ToString() ?? "");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<AllocationHistoryDto>> GetAllocationHistoryAsync(int employeeId)
        {
            try
            {
                var list = new List<AllocationHistoryDto>();
                await using var con = _sqlConnectionFactory.CreateConnection();
                await con.OpenAsync();

                await using var cmd = new SqlCommand(StoredProcedure.GetAllocationHistoryByEmployee, con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new AllocationHistoryDto
                    {
                        AllocationID = Convert.ToInt32(reader["AllocationID"]),
                        ProjectName = reader["ProjectName"].ToString(),  // <- ensure this column exists in SP
                        AllocationStartDate = Convert.ToDateTime(reader["AllocationStartDate"]),
                        AllocationEndDate = reader["AllocationEndDate"] == DBNull.Value ? null : (DateTime?)reader["AllocationEndDate"],
                        AllocationPercentage = Convert.ToInt32(reader["AllocationPercentage"])
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ProjectEmployeeDTO>> GetEmployeesByProjectAsync(string projectName)
        {
            try
            {
                var result = new List<ProjectEmployeeDTO>();
                await using var con = _sqlConnectionFactory.CreateConnection();

                using (var cmd = new SqlCommand(StoredProcedure.GetEmployeesByProject, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProjectName", projectName);

                    await con.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new ProjectEmployeeDTO
                            {
                                EmployeeID = reader.GetInt32("EmployeeID"),
                                FullName = reader.GetString("FullName"),
                                Email = reader.GetString("Email"),
                                Designation = reader.GetString("Designation"),
                            });
                        }
                    }
                }

                return result;
            }
            catch (Exception ex) { throw ex; }
        }

        public async Task<AssignmentResultDTO> AssignEmployeeToProjectAsync(AssignEmployeeToProjectDto dto)
        {
            var result = new AssignmentResultDTO();

            await using var con = _sqlConnectionFactory.CreateConnection();

            using var cmd = new SqlCommand(StoredProcedure.AssignEmployeeToProject, con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@EmployeeName", dto.EmployeeName);
            cmd.Parameters.AddWithValue("@ProjectName", dto.ProjectName);
            cmd.Parameters.AddWithValue("@AllocationStartDate", dto.AllocationStartDate);
            cmd.Parameters.AddWithValue("@AllocationEndDate", dto.AllocationEndDate);

            var outputCode = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var outputMsg = new SqlParameter("@Message", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output };

            cmd.Parameters.Add(outputCode);
            cmd.Parameters.Add(outputMsg);

            try
            {
                await con.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                result.ResultCode = (int)outputCode.Value;
                result.Message = (string)outputMsg.Value;
            }
            catch (Exception ex)
            {
                result.ResultCode = 4;
                result.Message = ex.Message;
            }

            return result;
        }
    }
}