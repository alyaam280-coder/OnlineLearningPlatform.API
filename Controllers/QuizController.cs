using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.BLL.Dtos.Quiz;
using OnlineLearningPlatform.BLL.Services.Quizes;

namespace OnlineLearningPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Instructor")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

       
        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var quizzes = await _quizService.GetAllQuizzesAsync();
            return Ok(quizzes);
        }

       
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Instructor,Student")]
        public async Task<IActionResult> GetById(int id)
        {
            var quiz = await _quizService.GetQuizByIdAsync(id);
            if (quiz == null)
            {
                return NotFound(new { Message = "not foune" });
            }
            return Ok(quiz);
        }

       
        [HttpPost]
        public async Task<IActionResult> Create(QuizCreateDto quizCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _quizService.AddQuizAsync(quizCreateDto);
            return Ok(new { Message = "created successfully" });
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, QuizUpdateDto quizUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingQuiz = await _quizService.GetQuizByIdAsync(id);
            if (existingQuiz == null)
            {
                return NotFound(new { Message = "can't update" });
            }

          
            await _quizService.UpdateQuizAsync(id, quizUpdateDto);
            return Ok(new { Message ="updated successfully" });
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingQuiz = await _quizService.GetQuizByIdAsync(id);
            if (existingQuiz == null)
            {
                return NotFound(new { Message = "can't delete" });
            }

            await _quizService.DeleteQuizAsync(id);
            return Ok(new { Message = "deleted successfully" });
        }
    }
}

