using Final_v1.Application.DTO.EmployeeDTO;
using Final_v1.Application.Interface;
using Final_v1.Domain.Interface;
using Final_v1.Infrastructure.Database;

namespace Final_v1.Application.Services.EmployeeService
{
    public class EmployeeService : IEmployee
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ISqlConnectionFactory _sqlConnectionFactory;


        public EmployeeService(IEmployeeRepository employeeRepository, ISqlConnectionFactory sqlConnectionFactory)
        {
            _employeeRepository = employeeRepository;
            _sqlConnectionFactory = sqlConnectionFactory ?? throw new ArgumentNullException(nameof(sqlConnectionFactory));

        }
        public async Task<(int ResultCode, string Message, int NewEmployeeID)> AddEmployeeAsync(CreateEmployeeRequestDTO dto)
        {
            try
            {
                // (Optional spot for additional business rules)
                return await _employeeRepository.AddEmployeeAsync(dto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<EmployeeResponseDTO>> GetAllEmployeesAsync()
        {
            try
            {
                return await _employeeRepository.GetAllEmployeesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<(int ResultCode, string Message)> SetEmployeeStatusAsync(SetEmployeeStatusDTO dto)
        {
            try
            {
                return await _employeeRepository.SetEmployeeStatusAsync(dto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<(int ResultCode, string Message)> UpdateEmployeeAsync(UpdateEmployeeRequestDTO dto)
        {
            try
            {
                return await _employeeRepository.UpdateEmployeeAsync(dto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<PageResultDto<EmployeeResponseDTO>> GetAllEmployeesAsyncPaged(int pageNumber, int pageSize)
        {
            // Validation (optional)
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize < 10) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            // Call repository method
            return await _employeeRepository.GetAllEmployeesAsyncPaged(pageNumber, pageSize);
        }
    }
}
