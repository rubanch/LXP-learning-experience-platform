using LXP.Common.ViewModels;
using LXP.Core.IServices;
using Microsoft.AspNetCore.Mvc;

namespace LXP.Api.Controllers
{
    ///<summary>
    ///Learner Video progress
    ///</summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LearnerProgressController : BaseController
    {
        //private readonly ILearnerProgressService _learnerProgressService;
        private readonly ILearnerProgressService _Progress;
      
        public LearnerProgressController(ILearnerProgressService Progress)
        {
            _Progress = Progress;
        }

        [HttpPost("/lxp/course/learner/learnerprogress")]
        public async Task<IActionResult> MaterialProgress(ProgressViewModel learnerProgress)
        {
            var result = await _Progress.LearnerProgress(learnerProgress);
            return Ok(result);
        }

        [HttpGet("/lxp/course/learner/learnerprogress/watchtime")]
        public async Task<IActionResult> GetLearnerProgressByLearnerIdAndMaterialId(
            string LearnerId,
            string MaterialId
        )
        {
            return Ok(
                CreateSuccessResponse(
                    await _Progress.GetLearnerProgressByLearnerIdAndMaterialId(
                        LearnerId,
                        MaterialId
                    )
                )
            );
        }

        //[HttpPost("/lxp/learner/learnerprogressStatus")]
        //public async Task MaterialCompleted(Guid learnerId, Guid courseId)
        //{
        //    await _Progress.CalculateMaterialCompletionPercentageAsync(learnerId, courseId);
        //}
        //[HttpPost("/lxp/learner/learnerprogressWatchTime")]
        //public async Task<IActionResult> MaterialWatchTime(Guid learnerId, Guid materialId, TimeOnly watchtime)
        //{
        //    var percentage = await _Progress.materialWatchTime(learnerId, materialId, watchtime);
        //    return Ok(percentage);
        //}

        //[HttpPost("calculate-course-completion/{learnerId}")]
        //public async Task<IActionResult> CalculateCourseCompletion(Guid learnerId)
        //{
        //    try
        //    {
        //        await _Progress.CalculateAndUpdateCourseCompletionAsync(learnerId);
        //        return Ok(
        //            new
        //            {
        //                Message = "Course completion percentage calculated and updated successfully."
        //            }
        //        );
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(
        //            500,
        //            new
        //            {
        //                Message = "An error occurred while calculating the course completion.",
        //                Details = ex.Message
        //            }
        //        );
        //    }
        //}


        [HttpGet("course-completion-percentage/{learnerId}/{enrollmentId}")]
        public async Task<IActionResult> GetCourseCompletionPercentage(
           Guid learnerId,
             Guid enrollmentId
)
        {
            try
            {
                var (percentage, courseId) = await _Progress.GetCourseCompletionAndCourseIdAsync(
                    learnerId,
                    enrollmentId
                );
                if (percentage.HasValue && courseId.HasValue)
                {
                    return Ok(new { CourseCompletionPercentage = percentage.Value, CourseId = courseId.Value });
                }
                else
                {
                    return NotFound(new { Message = "Enrollment not found." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        Message = "An error occurred while fetching the course completion percentage and course ID.",
                        Details = ex.Message
                    }
                );
            }
        }




        [HttpGet("{materialId}/progress/{learnerId}")]
        public async Task<ActionResult<double>> GetMaterialProgress(Guid materialId, Guid learnerId)
        {
            var progress = await _Progress.CalculateMaterialProgressAsync(materialId, learnerId);
            return Ok(progress);
        }








        //[HttpGet("combined-progress")]
        //public async Task<IActionResult> GetCombinedProgress(Guid learnerId, Guid enrollmentId, Guid materialId)
        //{
        //    try
        //    {
        //        // Call your learner progress service to calculate combined progress
        //        var (combinedProgress, courseId) = await _Progress.CalculateCombinedProgressAsync(learnerId, enrollmentId, materialId);

        //        return Ok(new
        //        {
        //            CombinedProgress = combinedProgress,
        //            CourseId = courseId
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions (e.g., invalid input, database errors)
        //        return BadRequest(ex.Message);
        //    }
        //}

    }
}
