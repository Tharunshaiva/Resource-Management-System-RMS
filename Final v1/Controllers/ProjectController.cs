using Final_v1.Application.DTO.ProjectDTO;
using Final_v1.Application.Interface;
using Final_v1.Application.Services.ProjectService;
using Microsoft.AspNetCore.Mvc;

namespace Final_v1.Controllers
{
        [ApiController]
        [Route("api/projects")]
        public class ProjectsController : ControllerBase
        {
            private readonly IProject _service;
            public ProjectsController(IProject service) => _service = service;

            [HttpPost("create")]
            public async Task<IActionResult> Create([FromBody] CreateProjectRequestDto dto, CancellationToken ct)
            {
            try
            {
                var id = await _service.CreateProjectAsync(dto, ct);
                return Ok(new { success = true, projectId = id });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

            [HttpPost("update")]
            public async Task<IActionResult> Update([FromBody] UpdateProjectRequestDto dto, CancellationToken ct)
            {
            try
            {
                var (ok, message) = await _service.UpdateProjectAsync(dto, ct);
                return Ok(new { success = ok, message });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
            [HttpPost("by-id")]
            public async Task<IActionResult> GetById([FromBody] GetProjectByIdRequestDto dto, CancellationToken ct)
            {
            try
            {
                var project = await _service.GetProjectByIdAsync(dto.ProjectID, ct);
                if (project is null) return NotFound(new { success = false, error = "NotFound" });

                var resp = ProjectResponseDto.FromEntity(project);
                return Ok(new { success = true, data = resp });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

            [HttpPost("search")]
            public async Task<IActionResult> Search([FromBody] ProjectSearchRequestDto filters, CancellationToken ct)
            {
            try
            {
                var projects = await _service.SearchProjectsAsync(filters, ct);
                var data = projects.Select(ProjectResponseDto.FromEntity);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

            [HttpPost("set-status")]
            public async Task<IActionResult> SetStatus([FromBody] SetProjectStatusRequestDto dto, CancellationToken ct)
            {
            try
            {
                var (ok, message) = await _service.SetStatusAsync(dto.ProjectID, dto.ProjectStatus, ct);
                return Ok(new { success = ok, message });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
            [HttpPost("set-capacity")]
            public async Task<IActionResult> SetCapacity([FromBody] SetProjectCapacityRequestDto dto, CancellationToken ct)
            {
            try
            {
                var (ok, message) = await _service.SetCapacityAsync(dto.ProjectID, dto.Capacity, ct);
                return Ok(new { success = ok, message });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

            [HttpPost("set-dates")]
            public async Task<IActionResult> SetDates([FromBody] SetProjectDatesRequestDto dto, CancellationToken ct)
            {
            try
            {
                var (ok, message) = await _service.SetDatesAsync(dto.ProjectID, dto.StartDate, dto.EndDate, ct);
                return Ok(new { success = ok, message });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

            [HttpPost("deactivate")]
            public async Task<IActionResult> Deactivate([FromBody] GetProjectByIdRequestDto dto, CancellationToken ct)
            {
            try
            {
                var (ok, message) = await _service.DeactivateAsync(dto.ProjectID, ct);
                return Ok(new { success = ok, message });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost("get-all")]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            try
            {
                var projects = await _service.GetAllProjectsAsync(ct);
                return Ok(new { success = true, working_days = projects }); // match frontend key if needed
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    }
