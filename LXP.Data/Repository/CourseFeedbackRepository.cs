using LXP.Common.Entities;
using LXP.Common.ViewModels.CourseFeedbackQuestionViewModel;
using LXP.Data.IRepository;
using Microsoft.EntityFrameworkCore;

namespace LXP.Data.Repository
{
    public class CourseFeedbackRepository : ICourseFeedbackRepository
    {
        private readonly LXPDbContext _context;

        public CourseFeedbackRepository(LXPDbContext context)
        {
            _context = context;
        }

        public void AddFeedbackQuestion(CourseFeedbackQuestion coursefeedbackquestion)
        {
            _context.CourseFeedbackQuestions.Add(coursefeedbackquestion);
            _context.SaveChanges();
        }

        public void AddFeedbackQuestionOptions(List<FeedbackQuestionsOption> options)
        {
            _context.FeedbackQuestionsOptions.AddRange(options);
            _context.SaveChanges();
        }

        public List<CourseFeedbackQuestionNoViewModel> GetAllFeedbackQuestions()
        {
            var feedbackQuestions = _context
                .CourseFeedbackQuestions.Include(q => q.FeedbackQuestionsOptions)
                .ToList();

            return feedbackQuestions
                .Select(q => new CourseFeedbackQuestionNoViewModel
                {
                    CourseFeedbackQuestionId = q.CourseFeedbackQuestionId,
                    CourseId = q.CourseId,
                    QuestionNo = q.QuestionNo,
                    Question = q.Question,
                    QuestionType = q.QuestionType,
                    Options = q
                        .FeedbackQuestionsOptions?.Select(
                            o => new CourseFeedbackQuestionsOptionViewModel
                            {
                                OptionText = o.OptionText
                            }
                        )
                        .ToList()
                })
                .ToList();
        }

        public CourseFeedbackQuestionNoViewModel GetFeedbackQuestionById(
            Guid courseFeedbackQuestionId
        )
        {
            var question = _context
                .CourseFeedbackQuestions.Include(q => q.FeedbackQuestionsOptions)
                .FirstOrDefault(q => q.CourseFeedbackQuestionId == courseFeedbackQuestionId);

            if (question == null)
            {
                return null;
            }

            return new CourseFeedbackQuestionNoViewModel
            {
                CourseFeedbackQuestionId = question.CourseFeedbackQuestionId,
                CourseId = question.CourseId,
                QuestionNo = question.QuestionNo,
                Question = question.Question,
                QuestionType = question.QuestionType,
                Options = question
                    .FeedbackQuestionsOptions?.Select(
                        o => new CourseFeedbackQuestionsOptionViewModel
                        {
                            OptionText = o.OptionText
                        }
                    )
                    .ToList()
            };
        }

        public CourseFeedbackQuestion GetCourseFeedbackQuestionEntityById(
            Guid courseFeedbackQuestionId
        )
        {
            return _context
                .CourseFeedbackQuestions.Include(q => q.FeedbackQuestionsOptions)
                .FirstOrDefault(q => q.CourseFeedbackQuestionId == courseFeedbackQuestionId);
        }

        public void UpdateFeedbackQuestion(CourseFeedbackQuestion coursefeedbackquestion)
        {
            _context.CourseFeedbackQuestions.Update(coursefeedbackquestion);
            _context.SaveChanges();
        }

        public List<FeedbackQuestionsOption> GetFeedbackQuestionOptionsById(
            Guid courseFeedbackQuestionId
        )
        {
            return _context
                .FeedbackQuestionsOptions.Where(o =>
                    o.CourseFeedbackQuestionId == courseFeedbackQuestionId
                )
                .ToList();
        }

        public void RemoveFeedbackQuestionOptions(List<FeedbackQuestionsOption> options)
        {
            _context.FeedbackQuestionsOptions.RemoveRange(options);
            _context.SaveChanges();
        }

        public void DeleteFeedbackQuestion(CourseFeedbackQuestion coursefeedbackquestion)
        {
            _context.CourseFeedbackQuestions.Remove(coursefeedbackquestion);
            _context.SaveChanges();
        }

        public List<CourseFeedbackQuestionNoViewModel> GetFeedbackQuestionsByCourseId(Guid courseId)
        {
            var feedbackQuestions = _context
                .CourseFeedbackQuestions.Where(q => q.CourseId == courseId)
                .Include(q => q.FeedbackQuestionsOptions)
                .ToList();

            return feedbackQuestions
                .Select(q => new CourseFeedbackQuestionNoViewModel
                {
                    CourseFeedbackQuestionId = q.CourseFeedbackQuestionId,
                    CourseId = q.CourseId,
                    QuestionNo = q.QuestionNo,
                    Question = q.Question,
                    QuestionType = q.QuestionType,
                    Options = q
                        .FeedbackQuestionsOptions?.Select(
                            o => new CourseFeedbackQuestionsOptionViewModel
                            {
                                OptionText = o.OptionText
                            }
                        )
                        .ToList()
                })
                .ToList();
        }

        public int GetNextFeedbackQuestionNo(Guid courseId)
        {
            return _context
                    .CourseFeedbackQuestions.Where(q => q.CourseId == courseId)
                    .Max(q => (int?)q.QuestionNo) + 1
                ?? 1;
        }
    }
}
