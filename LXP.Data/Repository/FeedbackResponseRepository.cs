using LXP.Common.Entities;
using LXP.Data.IRepository;

namespace LXP.Data.Repository
{
    public class FeedbackResponseRepository : IFeedbackResponseRepository
    {
        private readonly LXPDbContext _context;

        public FeedbackResponseRepository(LXPDbContext context)
        {
            _context = context;
        }

        public QuizFeedbackQuestion GetQuizFeedbackQuestion(Guid quizFeedbackQuestionId)
        {
            return _context.QuizFeedbackQuestions.FirstOrDefault(q =>
                q.QuizFeedbackQuestionId == quizFeedbackQuestionId
            );
        }

        public TopicFeedbackQuestion GetTopicFeedbackQuestion(Guid topicFeedbackQuestionId)
        {
            return _context.TopicFeedbackQuestions.FirstOrDefault(q =>
                q.TopicFeedbackQuestionId == topicFeedbackQuestionId
            );
        }

        public Learner GetLearner(Guid learnerId)
        {
            return _context.Learners.FirstOrDefault(l => l.LearnerId == learnerId);
        }

        public FeedbackResponse GetExistingQuizFeedbackResponse(
            Guid quizFeedbackQuestionId,
            Guid learnerId
        )
        {
            return _context.FeedbackResponses.FirstOrDefault(r =>
                r.QuizFeedbackQuestionId == quizFeedbackQuestionId && r.LearnerId == learnerId
            );
        }

        public FeedbackResponse GetExistingTopicFeedbackResponse(
            Guid topicFeedbackQuestionId,
            Guid learnerId
        )
        {
            return _context.FeedbackResponses.FirstOrDefault(r =>
                r.TopicFeedbackQuestionId == topicFeedbackQuestionId && r.LearnerId == learnerId
            );
        }

        public void AddFeedbackResponse(FeedbackResponse feedbackResponse)
        {
            _context.FeedbackResponses.Add(feedbackResponse);
            _context.SaveChanges();
        }

        public void AddFeedbackResponses(IEnumerable<FeedbackResponse> feedbackResponses)
        {
            _context.FeedbackResponses.AddRange(feedbackResponses);
            _context.SaveChanges();
        }

        // public Guid? GetOptionIdByText(Guid questionId, string optionText)
        // {
        //     var option =
        //         _context.FeedbackQuestionsOptions.FirstOrDefault(o =>
        //             o.QuizFeedbackQuestionId == questionId
        //             && o.OptionText.ToLower() == optionText.ToLower()
        //         )
        //         ?? _context.FeedbackQuestionsOptions.FirstOrDefault(o =>
        //             o.TopicFeedbackQuestionId == questionId
        //             && o.OptionText.ToLower() == optionText.ToLower()
        //         );

        //     return option?.FeedbackQuestionOptionId;
        // }

         public Guid? GetOptionIdByText(Guid questionId, string optionText)
        {
            var option =
                _context.FeedbackQuestionsOptions.FirstOrDefault(o =>
                    o.QuizFeedbackQuestionId == questionId
                    && o.OptionText.ToLower() == optionText.ToLower()
                )
                ?? _context.FeedbackQuestionsOptions.FirstOrDefault(o =>
                    o.TopicFeedbackQuestionId == questionId
                    && o.OptionText.ToLower() == optionText.ToLower()
                )
                ?? _context.FeedbackQuestionsOptions.FirstOrDefault(o =>
                    o.CourseFeedbackQuestionId == questionId
                    && o.OptionText.ToLower() == optionText.ToLower()
                );
 
            return option?.FeedbackQuestionOptionId;
        } // new code

        //new bug fix
        public void DeleteFeedbackResponsesByQuizQuestionId(Guid quizFeedbackQuestionId)
        {
            var responses = _context
                .FeedbackResponses.Where(r => r.QuizFeedbackQuestionId == quizFeedbackQuestionId)
                .ToList();
            _context.FeedbackResponses.RemoveRange(responses);
            _context.SaveChanges();
        }

        public void DeleteFeedbackResponsesByTopicQuestionId(Guid topicFeedbackQuestionId)
        {
            var responses = _context
                .FeedbackResponses.Where(r => r.TopicFeedbackQuestionId == topicFeedbackQuestionId)
                .ToList();
            _context.FeedbackResponses.RemoveRange(responses);
            _context.SaveChanges();
        }

        // new

        public IEnumerable<QuizFeedbackQuestion> GetQuizFeedbackQuestions(Guid quizId)
        {
            return _context.QuizFeedbackQuestions.Where(q => q.QuizId == quizId).ToList();
        }

        public IEnumerable<FeedbackResponse> GetQuizFeedbackResponsesByLearner(
            Guid learnerId,
            Guid quizId
        )
        {
            return _context
                .FeedbackResponses.Where(r =>
                    r.LearnerId == learnerId && r.QuizFeedbackQuestion.QuizId == quizId
                )
                .ToList();
        }

        public IEnumerable<TopicFeedbackQuestion> GetTopicFeedbackQuestions(Guid topicId)
        {
            return _context.TopicFeedbackQuestions.Where(q => q.TopicId == topicId).ToList();
        }

        public IEnumerable<FeedbackResponse> GetTopicFeedbackResponsesByLearner(
            Guid learnerId,
            Guid topicId
        )
        {
            return _context
                .FeedbackResponses.Where(r =>
                    r.LearnerId == learnerId && r.TopicFeedbackQuestion.TopicId == topicId
                )
                .ToList();
        }

        public CourseFeedbackQuestion GetCourseFeedbackQuestion(Guid courseFeedbackQuestionId)
        {
            return _context.CourseFeedbackQuestions.FirstOrDefault(q =>
                q.CourseFeedbackQuestionId == courseFeedbackQuestionId
            );
        }

        public FeedbackResponse GetExistingCourseFeedbackResponse(
            Guid courseFeedbackQuestionId,
            Guid learnerId
        )
        {
            return _context.FeedbackResponses.FirstOrDefault(r =>
                r.CourseFeedbackQuestionId == courseFeedbackQuestionId && r.LearnerId == learnerId
            );
        }

        public IEnumerable<CourseFeedbackQuestion> GetCourseFeedbackQuestions(Guid courseId)
        {
            return _context.CourseFeedbackQuestions.Where(q => q.CourseId == courseId).ToList();
        }

        public IEnumerable<FeedbackResponse> GetCourseFeedbackResponsesByLearner(
            Guid learnerId,
            Guid courseId
        )
        {
            return _context
                .FeedbackResponses.Where(r =>
                    r.LearnerId == learnerId && r.CourseFeedbackQuestion.CourseId == courseId
                )
                .ToList();
        }
    }
}
