using Final_v1.Application.Interface;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly IReport _reportService;

    public ReportController(IReport reportService)
    {
        _reportService = reportService;
    }

    [HttpPost("project-overlap")]
    public async Task<IActionResult> GetProjectOverlap()
    {
        try
        {
            var result = await _reportService.CheckProjectOverlapAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    [HttpPost("employee-history/{employeeId:int}")]
    public async Task<IActionResult> GetEmployeeHistory(int employeeId)
    {
        try
        {
            if (employeeId <= 0)
                return BadRequest("Employee ID must be greater than zero.");

            var result = await _reportService.GetEmployeeHistoryAsync(employeeId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    [HttpPost("project-utilization")]
    public async Task<IActionResult> GetProjectUtilization()
    {
        try
        {
            var result = await _reportService.GetProjectUtilizationAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    [HttpPost("skill-availability")]
    public async Task<IActionResult> GetSkillAvailability()
    {
        try
        {
            var result = await _reportService.GetSkillAvailabilityAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
