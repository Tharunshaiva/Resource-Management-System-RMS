using Final_v1.Application.DTO.RrportDTO;

namespace Final_v1.Application.Interface
{
    public interface IReport
    {
        Task<IEnumerable<ProjectOverlapDto>> CheckProjectOverlapAsync();
        Task<IEnumerable<EmployeeAllocationHistoryDTO>> GetEmployeeHistoryAsync(int employeeId);
        Task<IEnumerable<ProjectUtilizationDTO>> GetProjectUtilizationAsync();
        Task<IEnumerable<SkillAvailabilityDTO>> GetSkillAvailabilityAsync();

    }
}
