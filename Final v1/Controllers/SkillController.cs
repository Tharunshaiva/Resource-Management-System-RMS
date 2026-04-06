using Final_v1.Application.DTO.SkillDTO;
using Final_v1.Application.Services.SkillService;
using Microsoft.AspNetCore.Mvc;

namespace Final_v1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillController : ControllerBase
    {
        private readonly SkillService _skillService;

        public SkillController(SkillService skillService)
        {
            _skillService = skillService;
        }

        // CREATE SKILL
        [HttpPost("create")]
        public async Task<IActionResult> AddSkill([FromBody] CreateSkillDTO dto)
        {
            try
            {
                var result = await _skillService.AddSkillAsync(dto);

                return result switch
                {
                    -1 => StatusCode(500, new { message = "Database error while adding skill" }),
                    0 => BadRequest(new { message = "Failed to add skill" }),
                    _ => Ok(new { SkillID = result, message = "Skill added successfully" })
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // UPDATE SKILL
        [HttpPost("update")]
        public async Task<IActionResult> UpdateSkill(int id,UpdateSkillDTO dto)
        {
            try
            {
                dto.SkillID = id;
                var result = await _skillService.UpdateSkillAsync(dto);

                return result switch
                {
                    0 => Ok(new { message = "Skill updated" }),
                    -99 => StatusCode(500, new { message = "Database error while updating skill" })
                };
            }catch(Exception ex) { throw ex; }
        }

        // DELETE SKILL
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteSkill(string skillName)
        {
            try
            {
                var result = await _skillService.DeleteSkillAsync(skillName);

                return result switch
                {
                    0 => Ok(new { message = "Deleted succesfully" }),
                    -99 => StatusCode(500, new { message = "Database error while deleting skill" }),
                    _ => Ok(new { message = "Skill deleted successfully" })
                };
            }catch (Exception ex) { throw ex; }
        }

        // LIST ALL SKILLS
        [HttpPost("list")]
        public async Task<IActionResult> GetAllSkills()
        {
            try
            {
                var result = await _skillService.GetAllSkillsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error fetching skills: {ex.Message}" });
            }
        }

        // ASSIGN SKILL TO EMPLOYEE
        [HttpPost("assign")]
        public async Task<IActionResult> AssignSkillToEmployee([FromBody] EmployeeSkillMappingDTO dto)
        {
            try
            {
                var result = await _skillService.AssignSkillToEmployeeAsync(dto);

                return result switch
                {
                    0 => Ok(new { message = "Skill assigned to employee successfully" }),
                    1 => NotFound(new { message = "Employee or Skill not found" }),
                    2 => Conflict(new { message = "Skill already assigned to this employee" }),
                    4 => StatusCode(500, new { message = "Database error while assigning skill" }),
                    _ => StatusCode(500, new { message = "Unknown error while assigning skill" })
                };
            }catch(Exception ex) { throw ex; }
        }

        // REMOVE SKILL FROM EMPLOYEE
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveSkillFromEmployee([FromBody] EmployeeSkillMappingDTO dto)
        {
            try
            {
                var result = await _skillService.RemoveSkillFromEmployeeAsync(dto);

                return result switch
                {
                    0 => Ok(new { message = "Skill removed from employee successfully" }),
                    1 => NotFound(new { message = "Employee or Skill not found" }),
                    4 => StatusCode(500, new { message = "Database error while removing skill" }),
                    _ => StatusCode(500, new { message = "Unknown error while removing skill" })
                };
            }catch (Exception ex) { throw ex; }
        }

        // GET SKILLS BY EMPLOYEE
        [HttpPost("employee/{employeename}")]
        public async Task<IActionResult> GetSkillsByEmployee(string employeename)
        {
            try
            {
                var result = await _skillService.GetSkillsByEmployeeAsync(employeename);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error fetching skills for employee: {ex.Message}" });
            }
        }
    }
}
