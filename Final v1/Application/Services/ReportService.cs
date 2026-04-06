using Final_v1.Application.DTO.EmployeeDTO;
using Final_v1.Application.DTO.RrportDTO;
using Final_v1.Application.Interface;
using Final_v1.Domain.Interface;
using Final_v1.Infrastructure.Repositories;
using SkillAvailabilityDTO = Final_v1.Application.DTO.RrportDTO.SkillAvailabilityDTO;

namespace Final_v1.Application.Services
{
    public class ReportService : IReport
    {
        private readonly IReportRepository _repo;

        public ReportService(IReportRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ProjectOverlapDto>> CheckProjectOverlapAsync()
        {
            try
            {
                return await _repo.CheckProjectOverlapAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<EmployeeAllocationHistoryDTO>> GetEmployeeHistoryAsync(int employeeId)
        {
            try
            {
                if (employeeId <= 0)
                    throw new ArgumentException("Invalid employee id");

                return await _repo.GetEmployeeHistoryAsync(employeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ProjectUtilizationDTO>> GetProjectUtilizationAsync()
        {
            try
            {
                return await _repo.GetProjectUtilizationAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<SkillAvailabilityDTO>> GetSkillAvailabilityAsync()
        {
            try
            {
                return await _repo.GetSkillAvailabilityAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
