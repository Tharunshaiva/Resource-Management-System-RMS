using Final_v1.Application.DTO.SkillDTO;
using Final_v1.Application.Interface;
using Final_v1.Domain.Interface;

namespace Final_v1.Application.Services.SkillService
{
    public class SkillService : ISkillService
    {
        private readonly ISkillRepository _repository;

        public SkillService(ISkillRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> AddSkillAsync(CreateSkillDTO dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.SkillName))
                    throw new ArgumentException("Skill name cannot be empty.");
                if (dto.SkillName.Length > 100)
                    throw new ArgumentException("Skill name too long.");
                if (!string.IsNullOrEmpty(dto.Description) && dto.Description.Length > 250)
                    throw new ArgumentException("Description too long.");

                return await _repository.AddSkillAsync(dto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");

                throw ex;
            }
        }

        public async Task<int> UpdateSkillAsync(UpdateSkillDTO dto)
        {
            try
            {
                if (dto.SkillID <= 0)
                    throw new ArgumentException("Invalid SkillID");
                if (string.IsNullOrWhiteSpace(dto.SkillName))
                    throw new ArgumentException("Skill name cannot be empty.");

                return await _repository.UpdateSkillAsync(dto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> DeleteSkillAsync(string skillName)
        {
            try
            {
                
                return await _repository.DeleteSkillAsync(skillName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> AssignSkillToEmployeeAsync(EmployeeSkillMappingDTO dto)
        {
            try
            {

                return await _repository.AssignSkillToEmployeeAsync(dto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> RemoveSkillFromEmployeeAsync(EmployeeSkillMappingDTO dto)
        {
            try
            {
                return await _repository.RemoveSkillFromEmployeeAsync(dto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<CreateSkillDTO>> GetAllSkillsAsync()
        {
            try
            {
                return await _repository.GetAllSkillsAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<CreateSkillDTO>> GetSkillsByEmployeeAsync(string employeename)
        {
            try
            {
                return await _repository.GetSkillsByEmployeeAsync(employeename);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
