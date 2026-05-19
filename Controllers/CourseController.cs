using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.BLL.Dtos.Course;
using OnlineLearningPlatform.BLL.Services.Courses;

namespace OnlineLearningPlatform.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CourseController : ControllerBase
{
    private readonly ICourseService _courseService;


    public CourseController(ICourseService courseService)
    {
        _courseService = courseService;
    }


    [HttpGet]

    public async Task<IActionResult> GetAll()
    {
        var courses = await _courseService.GetAllCoursesAsync();
        return Ok(courses);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);
        if (course == null)
        {
            return NotFound(new { Message = $"not founed" });
        }
        return Ok(course);
    }


    [HttpPost]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> Create(CourseCreateDto courseCreateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _courseService.AddCourseAsync(courseCreateDto);
        return Ok(new { Message = "created successfully" });
    }


    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> Update(int id, CourseUpdateDto courseUpdateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingCourse = await _courseService.GetCourseByIdAsync(id);
        if (existingCourse == null)
        {
            return NotFound(new { Message = "can't update" });
        }

        await _courseService.UpdateCourseAsync( courseUpdateDto, id);
        return Ok(new { Message = "updated successfully" });
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> Delete(int id)
    {
        var existingCourse = await _courseService.GetCourseByIdAsync(id);
        if (existingCourse == null)
        {
            return NotFound(new { Message = "can't delete" });
        }

        await _courseService.DeleteCourseAsync(id);
        return Ok(new { Message = "deleted successfully" });
    }
}