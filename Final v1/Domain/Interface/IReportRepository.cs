using Final_v1.Application.DTO.EmployeeDTO;
using Final_v1.Application.DTO.ProjectDTO;
using Final_v1.Application.DTO.RrportDTO;
using SkillAvailabilityDTO = Final_v1.Application.DTO.RrportDTO.SkillAvailabilityDTO;

namespace Final_v1.Domain.Interface
{
    public interface IReportRepository
    {

        Task<IEnumerable<ProjectUtilizationDTO>> GetProjectUtilizationAsync();
        Task<IEnumerable<SkillAvailabilityDTO>> GetSkillAvailabilityAsync();
        Task<IEnumerable<EmployeeAllocationHistoryDTO>> GetEmployeeHistoryAsync(int employeeId);
        Task<IEnumerable<ProjectOverlapDto>> CheckProjectOverlapAsync();


    }
}
