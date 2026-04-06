using Final_v1.Application.DTO.EmployeeDTO;
using Final_v1.Application.Services.EmployeeService;
using Microsoft.AspNetCore.Mvc;

namespace Final_v1.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeService _employeeService;

        public EmployeeController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // ============================================================
        // 1. ADD EMPLOYEE
        // POST: /api/employee
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] CreateEmployeeRequestDTO dto)
        {
            try
            {
                var result = await _employeeService.AddEmployeeAsync(dto);

                return Ok(new
                {
                    result.ResultCode,
                    result.Message,
                    result.NewEmployeeID
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // ============================================================
        // 2. UPDATE EMPLOYEE
        // POST: /api/employee/{id}
        // ============================================================
        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeRequestDTO dto)
        {
            try
            {
                dto.EmployeeID = id;  // ensure path parameter is used

                var result = await _employeeService.UpdateEmployeeAsync(dto);

                return Ok(new
                {
                    result.ResultCode,
                    result.Message
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // ============================================================
        // 3. ACTIVATE / DEACTIVATE EMPLOYEE
        // POST: /api/employee/{id}/status
        // ============================================================

        [HttpPost("status/{id}")]
        public async Task<IActionResult> SetEmployeeStatus(int id, [FromBody] SetEmployeeStatusDTO dto)
        {
            try
            {
                dto.EmployeeID = id; // ✅ Fill EmployeeID from route
                var result = await _employeeService.SetEmployeeStatusAsync(dto);

                return Ok(new
                {
                    message = result.Message,
                    resultCode = result.ResultCode
                });
            }
            catch (Exception ex)
            {
throw ex;            }
        }

        // ============================================================
        // 4. GET ALL EMPLOYEES
        // GET: /api/employee
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                var employees = await _employeeService.GetAllEmployeesAsync();

                return Ok(employees);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]

        public async Task<IActionResult> GetEmployees(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var employees = await _employeeService.GetAllEmployeesAsyncPaged(pageNumber, pageSize);

                return Ok(employees);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
