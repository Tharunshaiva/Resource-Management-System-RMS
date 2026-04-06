using Final_v1.Application.DTO.AllocationDTO;
using Final_v1.Application.DTO.ProjectDTO;
using Final_v1.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Final_v1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AllocationController : ControllerBase
    {
        private readonly IAllocation _allocationService;

        public AllocationController(IAllocation allocationService)
        {
            _allocationService = allocationService;
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AddEmployeeSkill([FromBody] AllocationResponseDto dto, CancellationToken ct)
        {
            try
            {
                var response = await _allocationService.AddEmployeeSkillAsync(dto, ct);

                return Ok(new
                {
                    ResultCode = response.ResultCode,
                    Message = response.Message
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost("history/{employeeId:int}")]
        public async Task<IActionResult> GetAllocationHistory(int employeeId)
        {
            try
            {
                var history = await _allocationService.GetAllocationHistoryAsync(employeeId);

                // Map DTO to JSON-friendly object
                var result = history.Select(h => new
                {
                    allocationID = h.AllocationID,
                    projectName = h.ProjectName,
                    allocationStartDate = h.AllocationStartDate.ToString("yyyy-MM-dd"),
                    allocationEndDate = h.AllocationEndDate?.ToString("yyyy-MM-dd"),
                    allocationPercentage = h.AllocationPercentage
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Always return JSON on error
                throw ex;
            }

        }
        
        [HttpPost("ByProject")]
        public async Task<IActionResult> GetEmployeesByProject([FromQuery] string projectName)
        {
            try
            {
                var employees = await _allocationService.GetEmployeesByProjectAsync(projectName);

                    if (employees == null || !employees.Any())
                    {
                        return Ok(new
                        {
                            message = $"No employees found for project '{projectName}'"
                        });
                    }


                return Ok(employees);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        [HttpPost("AssignProject")]
        public async Task<IActionResult> AssignEmployee([FromBody] AssignEmployeeToProjectDto dto)
        {
            try
            {
                var result = await _allocationService.AssignEmployeeToProjectAsync(dto);

                if (result.ResultCode == 0)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}