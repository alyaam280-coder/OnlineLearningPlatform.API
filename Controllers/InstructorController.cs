using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.BLL.Dtos.Instructor;
using OnlineLearningPlatform.BLL.Services.Instructors;

namespace OnlineLearningPlatform.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class InstructorController : ControllerBase
{
    private readonly IInstructorService _instructorService;
    public InstructorController(IInstructorService instructorService)
    {
        _instructorService = instructorService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var instructors = await _instructorService.GetAllInstructorsAsync();
        return Ok(instructors);
    }
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> GetById(int id)
    {
        var instructor = await _instructorService.GetInstructorByIdAsync(id);

        if (instructor == null)
        {
            return NotFound(new { Message = $"the instructor whose id is{id} is not found" });
        }

        return Ok(instructor);
    }
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(InstructorCreateDto instructorCreateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _instructorService.AddInstructorAsync(instructorCreateDto);
        return Ok(new { Message = "the Addition process is done" });
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, InstructorUpdateDto instructorUpdateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingInstructor = await _instructorService.GetInstructorByIdAsync(id);
        if (existingInstructor == null)
        {
            return NotFound(new { Message = $"can't update because he is not found" }); 
        }
        await _instructorService.UpdateInstructorAsync( id , instructorUpdateDto);
        return Ok(new { Message = "updated successfully" });
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existingInstructor = await _instructorService.GetInstructorByIdAsync(id);
        if (existingInstructor == null)
        {
            return NotFound(new { Message = $"can't delete the instructor whose id is {id} " });
        }

        await _instructorService.DeleteInstructorAsync(id);
        return Ok(new { Message = "the delete process is done" });
    }

}