using Final_v1.Application.DTO.AllocationDTO;
using Final_v1.Application.DTO.EmployeeDTO;
using Final_v1.Application.DTO.ProjectDTO;
using Final_v1.Application.DTO.RrportDTO;
using ProjectEmployeeDTO = Final_v1.Application.DTO.RrportDTO.ProjectEmployeeDTO;

namespace Final_v1.Application.Interface
{
    public interface IAllocation
    {

        //Task<(int ResultCode, string Message)> AssignEmployeeAsync(AllocationResponseDto dto, CancellationToken ct = default);
        Task<(int ResultCode, string Message)> AddEmployeeSkillAsync(AllocationResponseDto dto, CancellationToken cancellationToken);

        Task<List<AllocationHistoryDto>> GetAllocationHistoryAsync(int employeeId);

        Task<IEnumerable<ProjectEmployeeDTO>> GetEmployeesByProjectAsync(string projectName);

        Task<AssignmentResultDTO> AssignEmployeeToProjectAsync(AssignEmployeeToProjectDto dto);



    }
}
