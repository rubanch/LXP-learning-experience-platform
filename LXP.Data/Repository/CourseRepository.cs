using LXP.Common.Entities;
using LXP.Common.ViewModels;
using LXP.Data.IRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LXP.Data.Repository
{
    public class CourseRepository : ICourseRepository
    {
        private readonly LXPDbContext _lXPDbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _contextAccessor;

        public CourseRepository(
            LXPDbContext lXPDbContext,
            IWebHostEnvironment environment,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _lXPDbContext = lXPDbContext;
            _environment = environment;
            _contextAccessor = httpContextAccessor;
        }

        public Course GetCourseDetailsByCourseName(string courseName)
        {
            return _lXPDbContext
                .Courses.Include(course => course.Level)
                .Include(course => course.Category)
                .FirstOrDefault(course => course.Title == courseName);
        }

        public void AddCourse(Course course)
        {
            _lXPDbContext.Courses.Add(course);
            _lXPDbContext.SaveChanges();
        }

        public bool AnyCourseByCourseTitle(string courseTitle)
        {
            return _lXPDbContext.Courses.Any(course => course.Title == courseTitle);
        }

        public Course GetCourseDetailsByCourseId(Guid CourseId)
        {
            return _lXPDbContext
                .Courses.Include(course => course.Level)
                .Include(course => course.Category)
                .FirstOrDefault(course => course.CourseId == CourseId);
        }

        public Course FindCourseid(Guid courseid)
        {
            return _lXPDbContext.Courses.Find(courseid);
        }

        public Enrollment FindEntrollmentcourse(Guid Courseid)
        {
            return _lXPDbContext.Enrollments.FirstOrDefault(Course => Course.CourseId == Courseid);
        }

        public async Task Deletecourse(Course course)
        {
            _lXPDbContext.Courses.Remove(course);
            await _lXPDbContext.SaveChangesAsync();
        }

        public async Task Changecoursestatus(Course course)
        {
            _lXPDbContext.Courses.Update(course);
            await _lXPDbContext.SaveChangesAsync();
        }

        public async Task Updatecourse(Course course)
        {
            _lXPDbContext.Courses.Update(course);
            await _lXPDbContext.SaveChangesAsync();
        }

        public IEnumerable<CourseDetailsViewModel> GetAllCourse()
        {
            // Get the course ratings
            var courseRatings = GetCourseRating().ToDictionary(cr => cr.Course_Id);
            return _lXPDbContext
                .Courses.Select(c => new CourseDetailsViewModel
                {
                    CourseId = c.CourseId,
                    Status = c.IsAvailable,
                    Title = c.Title,
                    Level = c.Level.Level,
                    Category = c.Category.Category,
                    Duration = c.Duration,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt,
                    CategoryId = c.CategoryId,
                    LevelId = c.LevelId,
                    Thumbnailimage = String.Format(
                        "{0}://{1}{2}/wwwroot/CourseThumbnailImages/{3}",
                        _contextAccessor.HttpContext.Request.Scheme,
                        _contextAccessor.HttpContext.Request.Host,
                        _contextAccessor.HttpContext.Request.PathBase,
                        c.Thumbnail
                    ),
                    ModifiedAt = c.ModifiedAt.ToString(),
                    AverageRating = courseRatings.ContainsKey(c.CourseId) ? courseRatings[c.CourseId].Rating : 0
                })
                .ToList();
        }

        public IEnumerable<CourseDetailsViewModel> GetLimitedCourse()
        {
            var courseRating = GetCourseRating().ToDictionary(cr=> cr.Course_Id);
            return _lXPDbContext
                .Courses.OrderByDescending(c => c.CreatedAt)
                .Select(c => new CourseDetailsViewModel
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    Level = c.Level.Level,
                    Category = c.Category.Category,
                    Duration = c.Duration,
                    Thumbnailimage = String.Format(
                        "{0}://{1}{2}/wwwroot/CourseThumbnailImages/{3}",
                        _contextAccessor.HttpContext.Request.Scheme,
                        _contextAccessor.HttpContext.Request.Host,
                        _contextAccessor.HttpContext.Request.PathBase,
                        c.Thumbnail
                    ),
                    CreatedAt = c.CreatedAt,
                  AverageRating = courseRating.ContainsKey(c.CourseId)?courseRating[c.CourseId].Rating : 0
                })
                .Take(9)
                .ToList();
        }

        public IEnumerable<CourseListViewModel> GetAllCourseDetails()
        {
            return _lXPDbContext
                .Courses.Select(c => new CourseListViewModel
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    Description = c.Description,
                    Level = c.Level.Level,
                    Category = c.Category.Category,
                    Duration = c.Duration,
                    CreatedAt = c.CreatedAt,
                    CreatedBy = c.CreatedBy,
                    Thumbnailimage = String.Format(
                        "{0}://{1}{2}/wwwroot/CourseThumbnailImages/{3}",
                        _contextAccessor.HttpContext.Request.Scheme,
                        _contextAccessor.HttpContext.Request.Host,
                        _contextAccessor.HttpContext.Request.PathBase,
                        c.Thumbnail
                    ),
                })
                .ToList();
        }

        public async Task<dynamic> GetAllCourseDetailsByLearnerId(Guid learnerId)
        {
            var query =
                from course in _lXPDbContext.Courses
                join enrollment in _lXPDbContext.Enrollments
                    on new { course.CourseId, LearnerId = learnerId } equals new
                    {
                        enrollment.CourseId,
                        enrollment.LearnerId
                    }
                    into enrollments
                from enrollment in enrollments.DefaultIfEmpty()
                where course.IsAvailable && course.IsActive
                orderby course.CreatedAt descending, course.CourseId, enrollment.EnrollmentId // Added 'descending' for CreatedAt
                select new
                {
                    CourseId = course.CourseId,
                    Category = course.Category.Category,
                    Level = course.Level.Level,
                    Title = course.Title,
                    Description = course.Description,
                    Duration = course.Duration,
                    Thumbnailimage = String.Format(
                        "{0}://{1}{2}/wwwroot/CourseThumbnailImages/{3}",
                        _contextAccessor.HttpContext.Request.Scheme,
                        _contextAccessor.HttpContext.Request.Host,
                        _contextAccessor.HttpContext.Request.PathBase,
                        course.Thumbnail
                    ),
                    CreatedBy = course.CreatedBy,
                    CreatedAt = course.CreatedAt,
                    IsActive = true,
                    IsAvailable = true,
                    ModifiedAt = course.ModifiedAt,
                    ModifiedBy = course.ModifiedBy,
                    EnrollStatus = enrollment.EnrollStatus == null ? false : enrollment.EnrollStatus
                };
            return query.ToList();
        }


        public IEnumerable<CourseRatingViewModel> GetCourseRating()
        {

            var queryrating = from c in _lXPDbContext.Courses
                              join cfq in _lXPDbContext.CourseFeedbackQuestions on c.CourseId equals cfq.CourseId into cfqGroup
                              from cfq in cfqGroup.DefaultIfEmpty()
                              join fr in _lXPDbContext.FeedbackResponses on cfq.CourseFeedbackQuestionId equals fr.CourseFeedbackQuestionId into frGroup
                              from fr in frGroup.DefaultIfEmpty()
                              join fqo in _lXPDbContext.FeedbackQuestionsOptions on fr.OptionId equals fqo.FeedbackQuestionOptionId into fqoGroup
                              from fqo in fqoGroup.DefaultIfEmpty()
                              group new { c, fqo } by new { c.CourseId, c.Title } into g
                              select new CourseRatingViewModel
                              {
                                  Course_Id = g.Key.CourseId,
                                  Title = g.Key.Title,
                                  Rating = g.Average(x => (decimal?)Convert.ToDecimal(x.fqo.OptionText)) ?? 0
                              };

            return queryrating.ToList();

        }


        public IEnumerable<TopicRatingViewModel> GetTopicRating()
        {
            var queryTopicRating = from t in _lXPDbContext.Topics
                                   join tfq in _lXPDbContext.TopicFeedbackQuestions on t.TopicId equals tfq.TopicId into tfqGroup
                                   from tfq in tfqGroup.DefaultIfEmpty()
                                   join fr in _lXPDbContext.FeedbackResponses on tfq.TopicFeedbackQuestionId equals fr.TopicFeedbackQuestionId into frGroup
                                   from fr in frGroup.DefaultIfEmpty()
                                   join fqo in _lXPDbContext.FeedbackQuestionsOptions on fr.OptionId equals fqo.FeedbackQuestionOptionId into fqoGroup
                                   from fqo in fqoGroup.DefaultIfEmpty()
                                   group new { t, fqo } by new { t.TopicId, t.Name } into g
                                   select new TopicRatingViewModel
                                   {
                                       Topic_Id = g.Key.TopicId,
                                       Name = g.Key.Name,
                                       Rating = g.Average(x => (decimal?)Convert.ToDecimal(x.fqo.OptionText)) ?? 0
                                   };

            return queryTopicRating.ToList();
        }


    }
}
