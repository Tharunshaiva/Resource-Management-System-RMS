using Final_v1.Application.DTO.EmployeeDTO;

namespace Final_v1.Application.Interface
{
    public interface IEmployee
    {
        Task<(int ResultCode, string Message, int NewEmployeeID)> AddEmployeeAsync(CreateEmployeeRequestDTO dto);
        Task<(int ResultCode, string Message)> UpdateEmployeeAsync(UpdateEmployeeRequestDTO dto);
        Task<(int ResultCode, string Message)> SetEmployeeStatusAsync(SetEmployeeStatusDTO dto);
        Task<IEnumerable<EmployeeResponseDTO>> GetAllEmployeesAsync();
        Task<PageResultDto<EmployeeResponseDTO>> GetAllEmployeesAsyncPaged(int pageNumber, int pageSize);

    }
}
