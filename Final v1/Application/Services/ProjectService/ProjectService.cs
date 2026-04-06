using Final_v1.Application.DTO.ProjectDTO;
using Final_v1.Application.Interface;
using Final_v1.Domain.Entites;
using Final_v1.Domain.Interface;

namespace Final_v1.Application.Services.ProjectService
{
    public class ProjectService : IProject
    {
        private readonly IProjectRepository _repo;
        public ProjectService(IProjectRepository repo)
        {
            _repo = repo;
        }
        public async Task<int> CreateProjectAsync(CreateProjectRequestDto dto, CancellationToken ct)
        {
            try
            {
               return await _repo.CreateProjectAsync(dto, ct);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<(bool ok, string message)> UpdateProjectAsync(UpdateProjectRequestDto dto, CancellationToken ct)
        {
            try
            {
                return await _repo.UpdateProjectAsync(dto, ct);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Project?> GetProjectByIdAsync(int id, CancellationToken ct)
        {
            try { 
                return await _repo.GetProjectByIdAsync(id, ct); 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IReadOnlyList<Project>> SearchProjectsAsync(ProjectSearchRequestDto filters, CancellationToken ct)
        {
            try
            {
                return await _repo.SearchProjectsAsync(filters, ct);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<(bool ok, string message)> SetStatusAsync(int id, string status, CancellationToken ct)
        {
            try 
            { 
                return await _repo.SetStatusAsync(id, status, ct); 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<(bool ok, string message)> SetCapacityAsync(int id, int capacity, CancellationToken ct)
        {
            try
            {
                return await _repo.SetCapacityAsync(id, capacity, ct);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<(bool ok, string message)> SetDatesAsync(int id, DateTime start, DateTime end, CancellationToken ct)
        {
            try { 
                return await _repo.SetDatesAsync(id, start, end, ct); 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<(bool ok, string message)> DeactivateAsync(int id, CancellationToken ct)
        {
            try 
            {
               return await _repo.DeactivateAsync(id, ct); 
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
                return await _repo.GetAllProjectsAsync(ct);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
