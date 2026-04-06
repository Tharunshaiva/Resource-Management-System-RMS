using Final_v1.Application.DTO.SkillDTO;

namespace Final_v1.Application.Interface
{
    public interface ISkillService
    {
        Task<int> AddSkillAsync(CreateSkillDTO dto);
        Task<int> UpdateSkillAsync(UpdateSkillDTO dto);
        Task<int> DeleteSkillAsync(string skillName);
        Task<IEnumerable<CreateSkillDTO>> GetAllSkillsAsync();
        Task<int> AssignSkillToEmployeeAsync(EmployeeSkillMappingDTO dto);
        Task<int> RemoveSkillFromEmployeeAsync(EmployeeSkillMappingDTO dto);
        Task<IEnumerable<CreateSkillDTO>> GetSkillsByEmployeeAsync(string employeename);

    }
}
