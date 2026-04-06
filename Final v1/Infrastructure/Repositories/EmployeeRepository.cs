using Final_v1.Application.DTO.EmployeeDTO;
using Final_v1.Domain.Interface;
using Final_v1.Infrastructure.Database;
using Final_v1.USP_StoredProcedure;
using Microsoft.AspNetCore.Connections;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Final_v1.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public EmployeeRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory ?? throw new ArgumentNullException(nameof(sqlConnectionFactory));
        }


        // ============================================================
        // 1. ADD EMPLOYEE
        // ============================================================
        public async Task<(int ResultCode, string Message, int NewEmployeeID)> AddEmployeeAsync(CreateEmployeeRequestDTO dto)
        {
            try
            {
                using var conn = _sqlConnectionFactory.CreateConnection();
                using var cmd = new SqlCommand(StoredProcedure.AddEmployee, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@FullName", dto.FullName);
                cmd.Parameters.AddWithValue("@Email", dto.Email);
                cmd.Parameters.AddWithValue("@Phone", dto.Phone);
                cmd.Parameters.AddWithValue("@Gender", dto.Gender);
                cmd.Parameters.AddWithValue("@Designation", dto.Designation);
                cmd.Parameters.AddWithValue("@ExperienceInYears", dto.ExperienceInYears);

                var rc = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var msg = new SqlParameter("@Message", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output };
                var newId = new SqlParameter("@NewEmployeeID", SqlDbType.Int) { Direction = ParameterDirection.Output };

                cmd.Parameters.Add(rc);
                cmd.Parameters.Add(msg);
                cmd.Parameters.Add(newId);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return (Convert.ToInt32(rc.Value), msg.Value?.ToString() ?? "", Convert.ToInt32(newId.Value));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        // ============================================================
        // 2. GET ALL EMPLOYEES
        // ============================================================
        public async Task<IEnumerable<EmployeeResponseDTO>> GetAllEmployeesAsync()

        {

            var list = new List<EmployeeResponseDTO>();

            try

            {

                using var conn = _sqlConnectionFactory.CreateConnection();

                using var cmd = new SqlCommand(StoredProcedure.GetAllEmployees, conn)

                {

                    CommandType = CommandType.StoredProcedure

                };

                await conn.OpenAsync();

                using var reader = await cmd.ExecuteReaderAsync();

                // Cache ordinals once (faster than calling GetOrdinal repeatedly)

                int idxEmployeeID = reader.GetOrdinal("EmployeeID");

                int idxFullName = reader.GetOrdinal("FullName");

                int idxEmail = reader.GetOrdinal("Email");

                int idxPhone = reader.GetOrdinal("Phone");

                int idxGender = reader.GetOrdinal("Gender");

                int idxDateOfJoining = reader.GetOrdinal("DateOfJoining");

                int idxDesignation = reader.GetOrdinal("Designation");

                int idxExperienceInYears = reader.GetOrdinal("ExperienceInYears");

                int idxIsActive = reader.GetOrdinal("IsActive");

                // New ordinals for skills (ensure SP returns these exact column names)

                int idxSkills = reader.GetOrdinal("Skills");

                int idxSkillIDs = reader.GetOrdinal("SkillIDs");

                while (await reader.ReadAsync())

                {
                    // Read base fields with NULL-safety for strings

                    var dto = new EmployeeResponseDTO

                    {

                        EmployeeID = reader.GetInt32(idxEmployeeID),

                        FullName = reader.IsDBNull(idxFullName) ? "" : reader.GetString(idxFullName),

                        Email = reader.IsDBNull(idxEmail) ? "" : reader.GetString(idxEmail),

                        Phone = reader.IsDBNull(idxPhone) ? "" : reader.GetString(idxPhone),

                        Gender = reader.IsDBNull(idxGender) ? "" : reader.GetString(idxGender),

                        DateOfJoining = reader.GetDateTime(idxDateOfJoining),

                        Designation = reader.IsDBNull(idxDesignation) ? "" : reader.GetString(idxDesignation),

                        ExperienceInYears = reader.GetDecimal(idxExperienceInYears),

                        IsActive = reader.GetBoolean(idxIsActive),

                    };

                    // Parse Skills (comma-separated names)

                    string? skillsStr = reader.IsDBNull(idxSkills) ? null : reader.GetString(idxSkills);

                    if (!string.IsNullOrWhiteSpace(skillsStr))

                    {

                        dto.Skills = skillsStr

                            .Split(',', StringSplitOptions.RemoveEmptyEntries)

                            .Select(s => s.Trim())

                            .Where(s => s.Length > 0)

                            .ToList();

                    }

                    // Parse SkillIDs (comma-separated ints)

                    string? skillIdsStr = reader.IsDBNull(idxSkillIDs) ? null : reader.GetString(idxSkillIDs);

                    if (!string.IsNullOrWhiteSpace(skillIdsStr))

                    {

                        dto.SkillIDs = skillIdsStr

                            .Split(',', StringSplitOptions.RemoveEmptyEntries)

                            .Select(s => int.TryParse(s.Trim(), out var id) ? (int?)id : null)

                            .Where(id => id.HasValue)

                            .Select(id => id!.Value)

                            .ToList();

                    }

                    list.Add(dto);

                }

            }

            catch (Exception ex)

            {
                throw ex;
            }

            return list;

        }


        // ============================================================
        // 3. UPDATE EMPLOYEE
        // ============================================================
        public async Task<(int ResultCode, string Message)> UpdateEmployeeAsync(UpdateEmployeeRequestDTO dto)
        {
            using var conn = _sqlConnectionFactory.CreateConnection();
            using var cmd = new SqlCommand(StoredProcedure.UpdateEmployee, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@EmployeeID", dto.EmployeeID);
            cmd.Parameters.AddWithValue("@FullName", dto.FullName);
            cmd.Parameters.AddWithValue("@Phone", dto.Phone);
            cmd.Parameters.AddWithValue("@Gender", dto.Gender);
            cmd.Parameters.AddWithValue("@Designation", dto.Designation);
            cmd.Parameters.AddWithValue("@ExperienceInYears", dto.ExperienceInYears);
            cmd.Parameters.AddWithValue("@Email", dto.Email); // <-- Include email

            var rc = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var msg = new SqlParameter("@Message", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output };

            cmd.Parameters.Add(rc);
            cmd.Parameters.Add(msg);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return (Convert.ToInt32(rc.Value), msg.Value?.ToString() ?? "");
        }
        public async Task<PageResultDto<EmployeeResponseDTO>> GetAllEmployeesAsyncPaged(int pageNumber = 1, int pageSize = 10)

        {

            var list = new List<EmployeeResponseDTO>();

            int totalCount = 0;

            try

            {

                using var conn = _sqlConnectionFactory.CreateConnection();

                using var cmd = new SqlCommand("SP_GetAllEmployeesPaged", conn)

                {

                    CommandType = CommandType.StoredProcedure

                };

                cmd.Parameters.AddWithValue("@PageNumber", pageNumber);

                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                await conn.OpenAsync();

                using var reader = await cmd.ExecuteReaderAsync();

                // cache ordinals

                int idxEmployeeID = reader.GetOrdinal("EmployeeID");

                int idxFullName = reader.GetOrdinal("FullName");

                int idxEmail = reader.GetOrdinal("Email");

                int idxPhone = reader.GetOrdinal("Phone");

                int idxGender = reader.GetOrdinal("Gender");

                int idxDateOfJoining = reader.GetOrdinal("DateOfJoining");

                int idxDesignation = reader.GetOrdinal("Designation");

                int idxExperienceInYears = reader.GetOrdinal("ExperienceInYears");

                int idxIsActive = reader.GetOrdinal("IsActive");

                int idxSkills = reader.GetOrdinal("Skills");

                int idxSkillIDs = reader.GetOrdinal("SkillIDs");

                int idxTotalCount = reader.GetOrdinal("TotalCount");

                while (await reader.ReadAsync())

                {

                    // read only once—totalCount same for all rows

                    if (totalCount == 0 && !reader.IsDBNull(idxTotalCount))

                        totalCount = reader.GetInt32(idxTotalCount);

                    var dto = new EmployeeResponseDTO

                    {

                        EmployeeID = reader.GetInt32(idxEmployeeID),

                        FullName = reader.IsDBNull(idxFullName) ? "" : reader.GetString(idxFullName),

                        Email = reader.IsDBNull(idxEmail) ? "" : reader.GetString(idxEmail),

                        Phone = reader.IsDBNull(idxPhone) ? "" : reader.GetString(idxPhone),

                        Gender = reader.IsDBNull(idxGender) ? "" : reader.GetString(idxGender),

                        DateOfJoining = reader.GetDateTime(idxDateOfJoining),

                        Designation = reader.IsDBNull(idxDesignation) ? "" : reader.GetString(idxDesignation),

                        ExperienceInYears = reader.GetDecimal(idxExperienceInYears),

                        IsActive = reader.GetBoolean(idxIsActive)

                    };

                    // skills parsing

                    string? skillsStr = reader.IsDBNull(idxSkills) ? null : reader.GetString(idxSkills);

                    dto.Skills = string.IsNullOrWhiteSpace(skillsStr)

                        ? new List<string>()

                        : skillsStr.Split(',').Select(x => x.Trim()).ToList();

                    // skillIDs parsing

                    string? skillIdsStr = reader.IsDBNull(idxSkillIDs) ? null : reader.GetString(idxSkillIDs);

                    dto.SkillIDs = string.IsNullOrWhiteSpace(skillIdsStr)

                        ? new List<int>()

                        : skillIdsStr.Split(',')

                                     .Select(s => int.TryParse(s.Trim(), out var id) ? id : (int?)null)

                                     .Where(id => id.HasValue)

                                     .Select(id => id!.Value)

                                     .ToList();

                    list.Add(dto);

                }

                return new PageResultDto<EmployeeResponseDTO>

                {

                    Items = list,

                    PageNumber = pageNumber,

                    PageSize = pageSize,

                    TotalCount = totalCount,

                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)

                };

            }

            catch (Exception)

            {

                throw;

            }

        }


        public async Task<(int ResultCode, string Message)> SetEmployeeStatusAsync(SetEmployeeStatusDTO dto)
        {
            try
            {
                using var conn = _sqlConnectionFactory.CreateConnection();
                using var cmd = new SqlCommand(StoredProcedure.SetEmployeeStatus, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@EmployeeID", dto.EmployeeID);
                cmd.Parameters.AddWithValue("@IsActive", dto.IsActive);

                var rc = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var msg = new SqlParameter("@Message", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output };

                cmd.Parameters.Add(rc);
                cmd.Parameters.Add(msg);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return (Convert.ToInt32(rc.Value), msg.Value?.ToString() ?? "");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
