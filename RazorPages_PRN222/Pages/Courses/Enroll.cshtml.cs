using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Models;
using Service.Interface;
using System.Security.Claims;

namespace RazorPages_PRN222.Pages.Courses
{
    [Authorize]
    public class EnrollModel : PageModel
    {
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly ILessonService _lessonService;
        private readonly IQuizService _quizService;
        private readonly ICourseReviewService _courseReviewService;

        public EnrollModel(
            ICourseService courseService,
            IEnrollmentService enrollmentService,
            ILessonService lessonService,
            IQuizService quizService,
            ICourseReviewService courseReviewService)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
            _lessonService = lessonService;
            _quizService = quizService;
            _courseReviewService = courseReviewService;
        }

        [BindProperty]
        public int CourseId { get; set; }

        public Course Course { get; set; }
        public bool IsEnrolled { get; set; }
        public int LessonCount { get; set; }
        public int QuizCount { get; set; }
        public int EnrollmentCount { get; set; }
        public decimal AverageRating { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            CourseId = id;
            await LoadCourseData();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadCourseData();
                return Page();
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToPage("/Login");
            }
            
            // Check if already enrolled
            var existingEnrollment = await _enrollmentService.GetByUserAndCourseAsync(userId, CourseId);
            if (existingEnrollment != null)
            {
                TempData["Message"] = "You are already enrolled in this course!";
                return RedirectToPage("/Courses/Details", new { id = CourseId });
            }

            // Create new enrollment
            var enrollment = new Enrollment
            {
                UserId = userId,
                CourseId = CourseId,
                EnrollmentDate = DateTime.Now,
                Status = "Active",
                PaymentStatus = "Paid", // Assuming free enrollment for now
                ProgressPercentage = 0,
                LastAccessedAt = DateTime.Now
            };

            var result = await _enrollmentService.CreateAsync(enrollment);
            if (result)
            {
                TempData["Message"] = "Successfully enrolled in the course!";
                return RedirectToPage("/Courses/Details", new { id = CourseId });
            }
            else
            {
                ModelState.AddModelError("", "Failed to enroll in the course. Please try again.");
                await LoadCourseData();
                return Page();
            }
        }

        private async Task LoadCourseData()
        {
            Course = await _courseService.GetByIdAsync(CourseId);
            if (Course != null)
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return;
                }
                
                // Check if user is enrolled
                var enrollment = await _enrollmentService.GetByUserAndCourseAsync(userId, CourseId);
                IsEnrolled = enrollment != null;

                // Get lesson count
                var allLessons = await _lessonService.GetAllAsync();
                LessonCount = allLessons.Count(l => l.CourseId == CourseId);

                // Get quiz count
                var allQuizzes = await _quizService.GetAllAsync();
                QuizCount = allQuizzes.Count(q => q.Lesson.CourseId == CourseId);

                // Get enrollment count
                var allEnrollments = await _enrollmentService.GetAllAsync();
                EnrollmentCount = allEnrollments.Count(e => e.CourseId == CourseId);

                // Get average rating
                var allReviews = await _courseReviewService.GetAllAsync();
                var courseReviews = allReviews.Where(r => r.CourseId == CourseId).ToList();
                if (courseReviews.Any())
                {
                    AverageRating = (decimal)courseReviews.Average(r => r.Rating);
                }
            }
        }
    }
}
