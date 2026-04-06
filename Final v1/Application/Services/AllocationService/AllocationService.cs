using Final_v1.Application.DTO.AllocationDTO;
using Final_v1.Application.DTO.ProjectDTO;
using Final_v1.Application.DTO.RrportDTO;
using Final_v1.Application.Interface;
using Final_v1.Domain.Interface;
using ProjectEmployeeDTO = Final_v1.Application.DTO.RrportDTO.ProjectEmployeeDTO;

namespace Final_v1.Application.Services
{
    public class AllocationService : IAllocation
    {
        private readonly IAllocationRepository _repo;

        public AllocationService(IAllocationRepository repo)
        {
            _repo = repo;
        }

        public async Task<(int ResultCode, string Message)> AddEmployeeSkillAsync(
    AllocationResponseDto dto, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(dto.EmployeeName) || string.IsNullOrWhiteSpace(dto.SkillName))
                return (-99, "Invalid EmployeeName or SkillName");

            return await _repo.AssignEmployeeAsync(dto, ct);
        }

        public async Task<AssignmentResultDTO> AssignEmployeeToProjectAsync(AssignEmployeeToProjectDto dto)
        {
            try
            {
                if (dto.AllocationEndDate < dto.AllocationStartDate)
                {
                    return new AssignmentResultDTO
                    {
                        ResultCode = 3,
                        Message = "AllocationEndDate must be after start date."
                    };
                }

                return await _repo.AssignEmployeeToProjectAsync(dto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<List<AllocationHistoryDto>> GetAllocationHistoryAsync(int employeeId)
        {
            try
            {
                if (employeeId <= 0)
                    throw new ArgumentException("Invalid EmployeeID");

                return await _repo.GetAllocationHistoryAsync(employeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<DTO.RrportDTO.ProjectEmployeeDTO>> GetEmployeesByProjectAsync(string projectName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(projectName))
                    throw new ArgumentException("Project name cannot be empty");

                return await _repo.GetEmployeesByProjectAsync(projectName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
