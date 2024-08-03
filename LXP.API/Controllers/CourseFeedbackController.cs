using LXP.Common.ViewModels.CourseFeedbackQuestionViewModel;
using LXP.Core.IServices;
using Microsoft.AspNetCore.Mvc;

namespace LXP.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseFeedbackController : BaseController
    {
        private readonly ICourseFeedbackService _service;

        public CourseFeedbackController(ICourseFeedbackService service)
        {
            _service = service;
        }

        [HttpPost("question")]
        public IActionResult AddFeedbackQuestion(CourseFeedbackQuestionViewModel question)
        {
            if (question == null)
            {
                return BadRequest(CreateFailureResponse("Question object is null", 400));
            }

            var options = question.Options ?? new List<CourseFeedbackQuestionsOptionViewModel>(); // Ensure options are not null
            var questionId = _service.AddFeedbackQuestion(question, options);

            if (questionId != Guid.Empty)
            {
                return Ok(CreateSuccessResponse("Question added successfully"));
            }

            return StatusCode(500, CreateFailureResponse("Failed to add question", 500));
        }

        [HttpGet]
        public IActionResult GetAllFeedbackQuestions()
        {
            var questions = _service.GetAllFeedbackQuestions();
            return Ok(CreateSuccessResponse(questions));
        }

        [HttpGet("{courseFeedbackQuestionId}")]
        public IActionResult GetFeedbackQuestionById(Guid courseFeedbackQuestionId)
        {
            var question = _service.GetFeedbackQuestionById(courseFeedbackQuestionId);
            if (question == null)
            {
                return NotFound(CreateFailureResponse("Feedback question not found", 404));
            }

            return Ok(CreateSuccessResponse(question));
        }

        [HttpPut("{courseFeedbackQuestionId}")]
        public IActionResult UpdateFeedbackQuestion(
            Guid courseFeedbackQuestionId,
            CourseFeedbackQuestionViewModel question
        )
        {
            var existingQuestion = _service.GetFeedbackQuestionById(courseFeedbackQuestionId);
            if (existingQuestion == null)
            {
                return NotFound(CreateFailureResponse("Feedback question not found", 404));
            }

            var options = question.Options ?? new List<CourseFeedbackQuestionsOptionViewModel>(); // Ensure options are not null
            var result = _service.UpdateFeedbackQuestion(
                courseFeedbackQuestionId,
                question,
                options
            );

            if (result)
            {
                return Ok(CreateSuccessResponse("Feedback question updated successfully"));
            }

            return StatusCode(
                500,
                CreateFailureResponse("Failed to update feedback question", 500)
            );
        }

        [HttpDelete("{courseFeedbackQuestionId}")]
        public IActionResult DeleteFeedbackQuestion(Guid courseFeedbackQuestionId)
        {
            var existingQuestion = _service.GetFeedbackQuestionById(courseFeedbackQuestionId);
            if (existingQuestion == null)
            {
                return NotFound(CreateFailureResponse("Feedback question not found", 404));
            }

            var result = _service.DeleteFeedbackQuestion(courseFeedbackQuestionId);

            if (result)
            {
                return Ok(CreateSuccessResponse("Feedback question deleted successfully"));
            }

            return StatusCode(
                500,
                CreateFailureResponse("Failed to delete feedback question", 500)
            );
        }

        [HttpGet("course/{courseId}")]
        public IActionResult GetFeedbackQuestionsByCourseId(Guid courseId)
        {
            var questions = _service.GetFeedbackQuestionsByCourseId(courseId);
            if (questions == null || !questions.Any())
            {
                return NotFound(
                    CreateFailureResponse("No questions found for the given course", 404)
                );
            }

            return Ok(CreateSuccessResponse(questions));
        }
    }
}
