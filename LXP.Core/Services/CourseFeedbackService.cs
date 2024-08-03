using LXP.Common.Constants;
using LXP.Common.Entities;
using LXP.Common.ViewModels.CourseFeedbackQuestionViewModel;
using LXP.Core.IServices;
using LXP.Data.IRepository;

namespace LXP.Core.Services
{
    public class CourseFeedbackService : ICourseFeedbackService
    {
        private readonly ICourseFeedbackRepository _courseFeedbackRepository;

        public CourseFeedbackService(ICourseFeedbackRepository courseFeedbackRepository)
        {
            _courseFeedbackRepository = courseFeedbackRepository;
        }

        public Guid AddFeedbackQuestion(
            CourseFeedbackQuestionViewModel courseFeedbackQuestion,
            List<CourseFeedbackQuestionsOptionViewModel> options
        )
        {
            var normalizedQuestionType = courseFeedbackQuestion.QuestionType.ToUpper();

            if (normalizedQuestionType == FeedbackQuestionTypes.DescriptiveQuestion.ToUpper())
            {
                options = null;
            }

            if (
                !ValidateOptionsByFeedbackQuestionType(courseFeedbackQuestion.QuestionType, options)
            )
                throw new ArgumentException(
                    "Invalid options for the given question type.",
                    nameof(options)
                );

            var questionEntity = new CourseFeedbackQuestion
            {
                CourseId = courseFeedbackQuestion.CourseId,
                QuestionNo = _courseFeedbackRepository.GetNextFeedbackQuestionNo(
                    courseFeedbackQuestion.CourseId
                ),
                Question = courseFeedbackQuestion.Question,
                QuestionType = normalizedQuestionType,
                CreatedBy = "Admin",
                CreatedAt = DateTime.Now
            };

            _courseFeedbackRepository.AddFeedbackQuestion(questionEntity);

            if (normalizedQuestionType == FeedbackQuestionTypes.MultiChoiceQuestion.ToUpper())
            {
                if (options != null && options.Count > 0)
                {
                    var optionEntities = options
                        .Select(option => new FeedbackQuestionsOption
                        {
                            CourseFeedbackQuestionId = questionEntity.CourseFeedbackQuestionId,
                            OptionText = option.OptionText,
                            CreatedAt = DateTime.Now,
                            CreatedBy = "Admin"
                        })
                        .ToList();

                    _courseFeedbackRepository.AddFeedbackQuestionOptions(optionEntities);
                }
            }

            return questionEntity.CourseFeedbackQuestionId;
        }

        public List<CourseFeedbackQuestionNoViewModel> GetAllFeedbackQuestions()
        {
            return _courseFeedbackRepository.GetAllFeedbackQuestions();
        }

        public CourseFeedbackQuestionNoViewModel GetFeedbackQuestionById(
            Guid courseFeedbackQuestionId
        )
        {
            return _courseFeedbackRepository.GetFeedbackQuestionById(courseFeedbackQuestionId);
        }

        public bool UpdateFeedbackQuestion(
            Guid courseFeedbackQuestionId,
            CourseFeedbackQuestionViewModel courseFeedbackQuestion,
            List<CourseFeedbackQuestionsOptionViewModel> options
        )
        {
            var existingQuestion = _courseFeedbackRepository.GetCourseFeedbackQuestionEntityById(
                courseFeedbackQuestionId
            );
            if (existingQuestion != null)
            {
                if (
                    !existingQuestion.QuestionType.Equals(
                        courseFeedbackQuestion.QuestionType,
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    throw new InvalidOperationException("Question type cannot be modified.");
                }

                existingQuestion.Question = courseFeedbackQuestion.Question;
                existingQuestion.ModifiedAt = DateTime.Now;
                existingQuestion.ModifiedBy = "Admin";
                _courseFeedbackRepository.UpdateFeedbackQuestion(existingQuestion);

                if (
                    existingQuestion.QuestionType
                    == FeedbackQuestionTypes.MultiChoiceQuestion.ToUpper()
                )
                {
                    if (
                        !ValidateOptionsByFeedbackQuestionType(
                            existingQuestion.QuestionType,
                            options
                        )
                    )
                    {
                        throw new ArgumentException("Invalid options for the given question type.");
                    }

                    var existingOptions = _courseFeedbackRepository.GetFeedbackQuestionOptionsById(
                        courseFeedbackQuestionId
                    );
                    _courseFeedbackRepository.RemoveFeedbackQuestionOptions(existingOptions);

                    if (options != null && options.Count > 0)
                    {
                        var optionEntities = options
                            .Select(option => new FeedbackQuestionsOption
                            {
                                CourseFeedbackQuestionId = courseFeedbackQuestionId,
                                OptionText = option.OptionText,
                                CreatedAt = DateTime.Now,
                                CreatedBy = "Admin"
                            })
                            .ToList();

                        _courseFeedbackRepository.AddFeedbackQuestionOptions(optionEntities);
                    }
                }

                return true;
            }
            return false;
        }

        public bool DeleteFeedbackQuestion(Guid courseFeedbackQuestionId)
        {
            var existingQuestion = _courseFeedbackRepository.GetCourseFeedbackQuestionEntityById(
                courseFeedbackQuestionId
            );
            if (existingQuestion != null)
            {
                var options = _courseFeedbackRepository.GetFeedbackQuestionOptionsById(
                    courseFeedbackQuestionId
                );
                _courseFeedbackRepository.RemoveFeedbackQuestionOptions(options);
                _courseFeedbackRepository.DeleteFeedbackQuestion(existingQuestion);
                return true;
            }
            return false;
        }

        public List<CourseFeedbackQuestionNoViewModel> GetFeedbackQuestionsByCourseId(Guid courseId)
        {
            return _courseFeedbackRepository.GetFeedbackQuestionsByCourseId(courseId);
        }

        private bool ValidateOptionsByFeedbackQuestionType(
            string questionType,
            List<CourseFeedbackQuestionsOptionViewModel> options
        )
        {
            if (string.IsNullOrWhiteSpace(questionType))
            {
                return false;
            }

            var normalizedQuestionType = questionType.ToUpper();
            if (normalizedQuestionType == FeedbackQuestionTypes.MultiChoiceQuestion.ToUpper())
            {
                return options != null && options.Count > 0;
            }

            return true;
        }
    }
}
