using Final_v1.Application.DTO.AllocationDTO;
using Final_v1.Application.DTO.EmployeeDTO;
using Final_v1.Application.DTO.ProjectDTO;
using Final_v1.Application.DTO.RrportDTO;
using ProjectEmployeeDTO = Final_v1.Application.DTO.RrportDTO.ProjectEmployeeDTO;

namespace Final_v1.Domain.Interface
{
    public interface IAllocationRepository
    {
        Task<(int ResultCode, string Message)> AssignEmployeeAsync(AllocationResponseDto dto, CancellationToken ct);

        Task<List<AllocationHistoryDto>> GetAllocationHistoryAsync(int employeeId);

        Task<IEnumerable<ProjectEmployeeDTO>> GetEmployeesByProjectAsync(string projectName);

        // Add employee to project
        Task<AssignmentResultDTO> AssignEmployeeToProjectAsync(AssignEmployeeToProjectDto dto);


    }
}
