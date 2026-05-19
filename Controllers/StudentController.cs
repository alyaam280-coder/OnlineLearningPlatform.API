using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.BLL.Dtos.Student;
using OnlineLearningPlatform.BLL.Services.Students;

namespace OnlineLearningPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        
        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return Ok(students);
        }

       
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> GetById(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                return NotFound(new { Message = "not found" });
            }
            return Ok(student);
        }

       
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(StudentCreateDto studentCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _studentService.AddStudentAsync(studentCreateDto);
            return Ok(new { Message = "created successfully" });
        }

      
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> Update(int id, StudentUpdateDto studentUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingStudent = await _studentService.GetStudentByIdAsync(id);
            if (existingStudent == null)
            {
                return NotFound(new { Message = "can't update" });
            }

           
            await _studentService.UpdateStudentAsync( id,studentUpdateDto);
            return Ok(new { Message = "updated successfully" });
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingStudent = await _studentService.GetStudentByIdAsync(id);
            if (existingStudent == null)
            {
                return NotFound(new { Message = "can't delete" });
            }

            await _studentService.DeleteStudentAsync(id);
            return Ok(new { Message = "deleted successfully" });
        }
    }
}

