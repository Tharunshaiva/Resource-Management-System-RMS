using Final_v1.Application.DTO.SkillDTO;
using Final_v1.Domain.Interface;
using Final_v1.Infrastructure.Database;
using Final_v1.USP_StoredProcedure;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Final_v1.Infrastructure.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public SkillRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory ?? throw new ArgumentNullException(nameof(sqlConnectionFactory));
        }

        public async Task<int> AddSkillAsync(CreateSkillDTO dto)
        {
            using var conn = _sqlConnectionFactory.CreateConnection();
            using var cmd = new SqlCommand(StoredProcedure.AddSkill, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@SkillName", dto.SkillName);
            cmd.Parameters.AddWithValue("@Description",
                string.IsNullOrWhiteSpace(dto.Description) ? DBNull.Value : dto.Description);

            var resultCode = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var message = new SqlParameter("@Message", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output };
            var skillId = new SqlParameter("@SkillID", SqlDbType.Int) { Direction = ParameterDirection.Output };

            cmd.Parameters.Add(resultCode);
            cmd.Parameters.Add(message);
            cmd.Parameters.Add(skillId);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            int rc = (int)(resultCode.Value ?? -1);
            int? newSkillId = skillId.Value == DBNull.Value ? null : (int?)skillId.Value;

            Console.WriteLine($"[AddSkill] RC={rc}, MSG={message.Value}, SkillID={newSkillId}");

            return newSkillId ?? 0;    // 0 = failed
        }
        public async Task<int> AssignSkillToEmployeeAsync(EmployeeSkillMappingDTO dto)
        {
            try
            {
                using var conn = _sqlConnectionFactory.CreateConnection();
                using var cmd = new SqlCommand(StoredProcedure.AssignSkillToEmployee, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Employeename", dto.EmployeeName);
                cmd.Parameters.AddWithValue("@Skillname", dto.SkillName);

                var resultCode = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var message = new SqlParameter("@Message", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output };

                cmd.Parameters.Add(resultCode);
                cmd.Parameters.Add(message);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                Console.WriteLine(message.Value);
                return Convert.ToInt32(resultCode.Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> DeleteSkillAsync(string skillName)
        {
            try
            {
                using var conn = _sqlConnectionFactory.CreateConnection();
                using var cmd = new SqlCommand(StoredProcedure.DeleteSkill, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Pass SkillName instead of SkillID
                cmd.Parameters.AddWithValue("@SkillName", skillName);

                var resultCode = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var message = new SqlParameter("@Message", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output };

                cmd.Parameters.Add(resultCode);
                cmd.Parameters.Add(message);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                Console.WriteLine(message.Value);
                return Convert.ToInt32(resultCode.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        public async Task<int> RemoveSkillFromEmployeeAsync(EmployeeSkillMappingDTO dto)
        {
            try
            {
                using var conn = _sqlConnectionFactory.CreateConnection();
                using var cmd = new SqlCommand(StoredProcedure.RemoveSkillFromEmployee, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Pass EmployeeName and SkillName instead of IDs
                cmd.Parameters.AddWithValue("@EmployeeName", dto.EmployeeName);
                cmd.Parameters.AddWithValue("@SkillName", dto.SkillName);

                var resultCode = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var message = new SqlParameter("@Message", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output };

                cmd.Parameters.Add(resultCode);
                cmd.Parameters.Add(message);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                Console.WriteLine(message.Value); // Show message
                return Convert.ToInt32(resultCode.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        public async Task<int> UpdateSkillAsync(UpdateSkillDTO dto)
        {
            try
            {
                using var conn = _sqlConnectionFactory.CreateConnection();
                using var cmd = new SqlCommand(StoredProcedure.UpdateSkill, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@SkillID", dto.SkillID);
                cmd.Parameters.AddWithValue("@SkillName", dto.SkillName);
                cmd.Parameters.AddWithValue("@Description", dto.Description ?? (object)DBNull.Value);

                var resultCode = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var message = new SqlParameter("@Message", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output };

                cmd.Parameters.Add(resultCode);
                cmd.Parameters.Add(message);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                Console.WriteLine(message);
                return Convert.ToInt32(resultCode.Value);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        public async Task<IEnumerable<CreateSkillDTO>> GetAllSkillsAsync()
        {
            var skills = new List<CreateSkillDTO>();
            try
            {
                using var conn = _sqlConnectionFactory.CreateConnection();
                using var cmd = new SqlCommand(StoredProcedure.GetAllSkills, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    skills.Add(new CreateSkillDTO
                    {
                        SkillID = reader.GetInt32(reader.GetOrdinal("SkillID")),
                        SkillName = reader.GetString(reader.GetOrdinal("SkillName")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description"))
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }

            return skills;
        }


        public async Task<IEnumerable<CreateSkillDTO>> GetSkillsByEmployeeAsync(string employeeName)
        {
            var skills = new List<CreateSkillDTO>();
            try
            {
                using var conn = _sqlConnectionFactory.CreateConnection();
                using var cmd = new SqlCommand(StoredProcedure.GetSkillsByEmployee, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Pass EmployeeName instead of EmployeeID
                cmd.Parameters.AddWithValue("@EmployeeName", employeeName);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    skills.Add(new CreateSkillDTO
                    {
                        SkillName = reader.GetString(reader.GetOrdinal("SkillName")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description"))
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
            return skills;
        }
    }
}
