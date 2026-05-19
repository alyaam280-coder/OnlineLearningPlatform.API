using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.BLL.Dtos.Enrollment;
using OnlineLearningPlatform.BLL.Services.Enrollments;

namespace OnlineLearningPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

       
        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var enrollments = await _enrollmentService.GetAllEnrollmentsAsync();
            return Ok(enrollments);
        }

       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id);
            if (enrollment == null)
            {
                return NotFound(new { Message = "not found" });
            }
            return Ok(enrollment);
        }


        [HttpPost]
        public async Task<IActionResult> Create(EnrollmentCreateDto enrollmentCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _enrollmentService.EnrollStudentAsync(enrollmentCreateDto);
            return Ok(new { Message = "regestered successfully" });
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingEnrollment = await _enrollmentService.GetEnrollmentByIdAsync(id);
            if (existingEnrollment == null)
            {
                return NotFound(new { Message ="can't delete" });
            }

            await _enrollmentService.CancelEnrollmentAsync(id);
            return Ok(new { Message = "deleted successfully" });
        }
    }
}

