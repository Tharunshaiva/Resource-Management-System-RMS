using Final_v1.Application.DTO.ProjectDTO;
using Final_v1.Domain.Entites;

namespace Final_v1.Domain.Interface
{
    public interface IProjectRepository
    {
        Task<int> CreateProjectAsync(CreateProjectRequestDto dto, CancellationToken ct);
        Task<(bool ok, string message)> UpdateProjectAsync(UpdateProjectRequestDto dto, CancellationToken ct);
        Task<Project?> GetProjectByIdAsync(int projectId, CancellationToken ct);
        Task<IReadOnlyList<Project>> SearchProjectsAsync(ProjectSearchRequestDto filters, CancellationToken ct);
        Task<(bool ok, string message)> SetStatusAsync(int projectId, string status, CancellationToken ct);
        Task<(bool ok, string message)> SetCapacityAsync(int projectId, int capacity, CancellationToken ct);
        Task<(bool ok, string message)> SetDatesAsync(int projectId, DateTime start, DateTime end, CancellationToken ct);
        Task<(bool ok, string message)> DeactivateAsync(int projectId, CancellationToken ct);
        Task<List<GetAllProjectsDTO>> GetAllProjectsAsync(CancellationToken ct);

    }
}
