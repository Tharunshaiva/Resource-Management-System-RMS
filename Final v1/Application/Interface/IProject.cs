using Final_v1.Application.DTO.ProjectDTO;
using Final_v1.Domain.Entites;

namespace Final_v1.Application.Interface
{
    public interface IProject
    {
        Task<int> CreateProjectAsync(CreateProjectRequestDto dto, CancellationToken ct);
        Task<(bool ok, string message)> UpdateProjectAsync(UpdateProjectRequestDto dto, CancellationToken ct);
        Task<Project?> GetProjectByIdAsync(int id, CancellationToken ct);
        Task<IReadOnlyList<Project>> SearchProjectsAsync(ProjectSearchRequestDto filters, CancellationToken ct);
        Task<(bool ok, string message)> SetStatusAsync(int id, string status, CancellationToken ct);
        Task<(bool ok, string message)> SetCapacityAsync(int id, int capacity, CancellationToken ct);
        Task<(bool ok, string message)> SetDatesAsync(int id, DateTime start, DateTime end, CancellationToken ct);
        Task<(bool ok, string message)> DeactivateAsync(int id, CancellationToken ct);
        Task<List<GetAllProjectsDTO>> GetAllProjectsAsync(CancellationToken ct);

    }
}
