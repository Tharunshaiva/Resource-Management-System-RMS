using Final_v1.Application.DTO.EmployeeDTO;

namespace Final_v1.Domain.Interface
{
    public interface IEmployeeRepository
    {
        Task<(int ResultCode, string Message, int NewEmployeeID)> AddEmployeeAsync(CreateEmployeeRequestDTO dto);
        Task<(int ResultCode, string Message)> UpdateEmployeeAsync(UpdateEmployeeRequestDTO dto);
        Task<(int ResultCode, string Message)> SetEmployeeStatusAsync(SetEmployeeStatusDTO dto);
        Task<IEnumerable<EmployeeResponseDTO>> GetAllEmployeesAsync();

        Task<PageResultDto<EmployeeResponseDTO>> GetAllEmployeesAsyncPaged(int pageNumber = 1, int pageSize = 10);



    }
}
