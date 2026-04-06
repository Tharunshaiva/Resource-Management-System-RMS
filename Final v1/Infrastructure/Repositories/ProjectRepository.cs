using Final_v1.Application.DTO.ProjectDTO;
using Final_v1.Domain.Entites;
using Final_v1.Domain.Interface;
using Final_v1.Infrastructure.Database;
using Final_v1.USP_StoredProcedure;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Final_v1.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public ProjectRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory ?? throw new ArgumentNullException(nameof(sqlConnectionFactory));
        }
        public async Task<int> CreateProjectAsync(CreateProjectRequestDto dto, CancellationToken ct)
        {
            await using var con = _sqlConnectionFactory.CreateConnection();
            await con.OpenAsync(ct);

            await using var cmd = new SqlCommand(StoredProcedure.CreateProject, con)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@ProjectName", dto.ProjectName);
            cmd.Parameters.AddWithValue("@ClientName", dto.ClientName);
            cmd.Parameters.AddWithValue("@Capacity", dto.Capacity);
            cmd.Parameters.AddWithValue("@StartDate", dto.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", dto.EndDate);
            cmd.Parameters.AddWithValue("@ProjectStatus", dto.ProjectStatus);


            var pId = new SqlParameter("@NewProjectID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var pCode = new SqlParameter("@ReturnCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var pMsg = new SqlParameter("@Message", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output };

            cmd.Parameters.AddRange(new[] { pId, pCode, pMsg });

            await cmd.ExecuteNonQueryAsync(ct);

            var code = (int)(pCode.Value ?? 4);
            var msg = (string)(pMsg.Value ?? "Unknown");

            return code switch
            {
                0 => (int)(pId.Value ?? 0),
                2 => throw new InvalidOperationException("ProjectName must be unique."),
                1 => throw new ArgumentException(msg),
                _ => throw new Exception(msg)
            };
        }

        public async Task<(bool ok, string message)> DeactivateAsync(int projectId, CancellationToken ct)
        {
            try
            {
                await using var con = _sqlConnectionFactory.CreateConnection();
                await con.OpenAsync(ct);

                await using var cmd = new SqlCommand(StoredProcedure.DeactivateProject, con)
                { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.AddWithValue("@ProjectID", projectId);

                var pCode = new SqlParameter("@ReturnCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var pMsg = new SqlParameter("@Message", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output };
                cmd.Parameters.AddRange(new[] { pCode, pMsg });

                await cmd.ExecuteNonQueryAsync(ct);
                var code = (int)(pCode.Value ?? 4);
                var msg = (string)(pMsg.Value ?? "Unknown");
                return (code == 0, msg);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<Project?> GetProjectByIdAsync(int projectId, CancellationToken ct)
        {
            try
            {
                await using var con = _sqlConnectionFactory.CreateConnection();
                await con.OpenAsync(ct);

                await using var cmd = new SqlCommand(StoredProcedure.GetProjectById, con)
                { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.AddWithValue("@ProjectID", projectId);

                var pCode = new SqlParameter("@ReturnCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var pMsg = new SqlParameter("@Message", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output };
                cmd.Parameters.AddRange(new[] { pCode, pMsg });

                await using var reader = await cmd.ExecuteReaderAsync(ct);

                if (!reader.HasRows) return null;

                if (await reader.ReadAsync(ct))
                {
                    return new Project
                    {
                        ProjectID = reader.GetInt32(reader.GetOrdinal("ProjectID")),
                        ProjectName = reader.GetString(reader.GetOrdinal("ProjectName")),
                        ClientName = reader.GetString(reader.GetOrdinal("ClientName")),
                        Capacity = reader.GetInt32(reader.GetOrdinal("Capacity")),
                        StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                        EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                        ProjectStatus = reader.GetString(reader.GetOrdinal("ProjectStatus"))
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }


        public async Task<IReadOnlyList<Project>> SearchProjectsAsync(ProjectSearchRequestDto filters, CancellationToken ct)
        {
            try
            {
                await using var con = _sqlConnectionFactory.CreateConnection();
                await con.OpenAsync(ct);

                await using var cmd = new SqlCommand(StoredProcedure.SearchProjects, con)
                { CommandType = CommandType.StoredProcedure };

                // Use explicit types to avoid AddWithValue inference issues
                cmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.VarChar, 20)
                { Value = (object?)filters.Status ?? DBNull.Value });

                cmd.Parameters.Add(new SqlParameter("@NameContains", SqlDbType.VarChar, 150)
                { Value = (object?)filters.NameContains ?? DBNull.Value });

                cmd.Parameters.Add(new SqlParameter("@StartsOnOrAfter", SqlDbType.Date)
                { Value = (object?)filters.StartsOnOrAfter ?? DBNull.Value });

                cmd.Parameters.Add(new SqlParameter("@EndsOnOrBefore", SqlDbType.Date)
                { Value = (object?)filters.EndsOnOrBefore ?? DBNull.Value });

                cmd.Parameters.Add(new SqlParameter("@Page", SqlDbType.Int) { Value = filters.Page });
                cmd.Parameters.Add(new SqlParameter("@PageSize", SqlDbType.Int) { Value = filters.PageSize });

                // Do not send @Assigned — SP doesn’t accept it
                // cmd.Parameters.AddWithValue("@Assigned", filters.Assigned);

                var pCode = new SqlParameter("@ReturnCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var pMsg = new SqlParameter("@Message", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output };
                cmd.Parameters.AddRange(new[] { pCode, pMsg });

                var list = new List<Project>();
                await using var reader = await cmd.ExecuteReaderAsync(ct);

                // Cache column ordinals (safer and faster)
                int ordProjectID = reader.GetOrdinal("ProjectID");
                int ordProjectName = reader.GetOrdinal("ProjectName");
                int ordClientName = reader.GetOrdinal("ClientName");
                int ordCapacity = reader.GetOrdinal("Capacity");
                int ordStartDate = reader.GetOrdinal("StartDate");
                int ordEndDate = reader.GetOrdinal("EndDate");
                int ordProjectStatus = reader.GetOrdinal("ProjectStatus");
                int ordAssigned = reader.GetOrdinal("Assigned");
                // Try to get Assigned ordinal defensively (in case SP hasn’t been updated yet)
                //int ordAssigned = -1;
                //try
                //{
                //    ordAssigned = reader.GetOrdinal("Assigned");
                //}
                //catch (IndexOutOfRangeException)
                //{
                //    // Column does not exist; will default to 0 below
                //}

                while (await reader.ReadAsync(ct))
                {
                    list.Add(new Project
                    {
                        ProjectID = reader.GetInt32(ordProjectID),
                        ProjectName = reader.IsDBNull(ordProjectName) ? string.Empty : reader.GetString(ordProjectName),
                        ClientName = reader.IsDBNull(ordClientName) ? string.Empty : reader.GetString(ordClientName),
                        Capacity = reader.IsDBNull(ordCapacity) ? 0 : reader.GetInt32(ordCapacity),
                        StartDate = reader.IsDBNull(ordStartDate) ? default : reader.GetDateTime(ordStartDate),
                        EndDate = reader.IsDBNull(ordEndDate) ? default : reader.GetDateTime(ordEndDate),
                        ProjectStatus = reader.IsDBNull(ordProjectStatus) ? string.Empty : reader.GetString(ordProjectStatus),
                        Assigned = (ordAssigned >= 0 && !reader.IsDBNull(ordAssigned))
                                        ? reader.GetInt32(ordAssigned)
                                        : 0
                    });
                }

                return list;
            }
            catch
            {
                // Preserve stack trace
                throw;
            }
        }
        public async Task<(bool ok, string message)> SetCapacityAsync(int projectId, int capacity, CancellationToken ct)
        {
            try
            {
                await using var con = _sqlConnectionFactory.CreateConnection();
                await con.OpenAsync(ct);

                await using var cmd = new SqlCommand(StoredProcedure.SetCapacity, con)
                { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.AddWithValue("@ProjectID", projectId);
                cmd.Parameters.AddWithValue("@Capacity", capacity);

                var pCode = new SqlParameter("@ReturnCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var pMsg = new SqlParameter("@Message", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output };
                cmd.Parameters.AddRange(new[] { pCode, pMsg });

                await cmd.ExecuteNonQueryAsync(ct);
                var code = (int)(pCode.Value ?? 4);
                var msg = (string)(pMsg.Value ?? "Unknown");
                return (code == 0, msg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<(bool ok, string message)> SetDatesAsync(int projectId, DateTime start, DateTime end, CancellationToken ct)
        {
            try
            {
                await using var con = _sqlConnectionFactory.CreateConnection();
                await con.OpenAsync(ct);

                await using var cmd = new SqlCommand(StoredProcedure.SetDates, con)
                { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.AddWithValue("@ProjectID", projectId);
                cmd.Parameters.AddWithValue("@StartDate", start);
                cmd.Parameters.AddWithValue("@EndDate", end);

                var pCode = new SqlParameter("@ReturnCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var pMsg = new SqlParameter("@Message", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output };
                cmd.Parameters.AddRange(new[] { pCode, pMsg });

                await cmd.ExecuteNonQueryAsync(ct);
                var code = (int)(pCode.Value ?? 4);
                var msg = (string)(pMsg.Value ?? "Unknown");
                return (code == 0, msg);
            }
            catch (Exception ex) { throw ex; }
        }

        public async Task<(bool ok, string message)> SetStatusAsync(int projectId, string status, CancellationToken ct)
        {
            try
            {
                await using var con = _sqlConnectionFactory.CreateConnection();
                await con.OpenAsync(ct);

                await using var cmd = new SqlCommand(StoredProcedure.SetStatus, con)
                { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.AddWithValue("@ProjectID", projectId);
                cmd.Parameters.AddWithValue("@ProjectStatus", status);

                var pCode = new SqlParameter("@ReturnCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var pMsg = new SqlParameter("@Message", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output };
                cmd.Parameters.AddRange(new[] { pCode, pMsg });

                await cmd.ExecuteNonQueryAsync(ct);
                var code = (int)(pCode.Value ?? 4);
                var msg = (string)(pMsg.Value ?? "Unknown");
                return (code == 0, msg);
            }
            catch (Exception ex) { throw ex; }
        }

        public async Task<(bool ok, string message)> UpdateProjectAsync(UpdateProjectRequestDto dto, CancellationToken ct)
        {
            try
            {
                await using var con = _sqlConnectionFactory.CreateConnection();
                await con.OpenAsync(ct);

                await using var cmd = new SqlCommand(StoredProcedure.UpdateProject, con)
                { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.AddWithValue("@ProjectID", dto.ProjectID);
                cmd.Parameters.AddWithValue("@ProjectName", (object?)dto.ProjectName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ClientName", (object?)dto.ClientName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Capacity", (object?)dto.Capacity ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@StartDate", (object?)dto.StartDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EndDate", (object?)dto.EndDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ProjectStatus", (object?)dto.ProjectStatus ?? DBNull.Value);

                var pCode = new SqlParameter("@ReturnCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var pMsg = new SqlParameter("@Message", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output };
                cmd.Parameters.AddRange(new[] { pCode, pMsg });

                await cmd.ExecuteNonQueryAsync(ct);
                var code = (int)(pCode.Value ?? 4);
                var msg = (string)(pMsg.Value ?? "Unknown");
                return (code == 0, msg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<GetAllProjectsDTO>> GetAllProjectsAsync(CancellationToken ct)
        {
            try
            {
                var projects = new List<GetAllProjectsDTO>();

                await using var con = _sqlConnectionFactory.CreateConnection();
                await con.OpenAsync(ct);

                await using var cmd = new SqlCommand(StoredProcedure.GetAllProjects, con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                await using var reader = await cmd.ExecuteReaderAsync(ct);

                while (await reader.ReadAsync(ct))
                {
                    var project = new GetAllProjectsDTO
                    {
                        ProjectID = reader.GetInt32(reader.GetOrdinal("ProjectID")),
                        ProjectName = reader.GetString(reader.GetOrdinal("ProjectName")),
                        ClientName = reader.GetString(reader.GetOrdinal("ClientName")),
                        Capacity = reader.GetInt32(reader.GetOrdinal("Capacity")),
                        StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                        EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                        ProjectStatus = reader.GetString(reader.GetOrdinal("ProjectStatus"))
                    };

                    projects.Add(project);
                }

                return projects;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

}
