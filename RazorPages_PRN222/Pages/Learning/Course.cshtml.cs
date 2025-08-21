using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Models;
using Service.Interface;
using System.Security.Claims;

namespace RazorPages_PRN222.Pages.Learning
{
    [Authorize]
    public class CourseModel : PageModel
    {
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly ILessonService _lessonService;
        private readonly IQuizService _quizService;
        private readonly IStudentProgressService _studentProgressService;
        private readonly IStudentQuizAttemptService _studentQuizAttemptService;

        public CourseModel(
            ICourseService courseService,
            IEnrollmentService enrollmentService,
            ILessonService lessonService,
            IQuizService quizService,
            IStudentProgressService studentProgressService,
            IStudentQuizAttemptService studentQuizAttemptService)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
            _lessonService = lessonService;
            _quizService = quizService;
            _studentProgressService = studentProgressService;
            _studentQuizAttemptService = studentQuizAttemptService;
        }

        public Course Course { get; set; }
        public Enrollment Enrollment { get; set; }
        public List<Lesson> Lessons { get; set; } = new List<Lesson>();
        public List<Repository.Models.Quiz> Quizzes { get; set; } = new List<Repository.Models.Quiz>();
        public List<StudentProgress> LessonProgress { get; set; } = new List<StudentProgress>();
        public List<StudentQuizAttempt> QuizAttempts { get; set; } = new List<StudentQuizAttempt>();
        public List<StudentProgress> RecentActivity { get; set; } = new List<StudentProgress>();

        public int CompletedLessons { get; set; }
        public int PassedQuizzes { get; set; }
        public int TotalLessons { get; set; }
        public int TotalQuizzes { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToPage("/Login");
            }
            
            // Get course
            Course = await _courseService.GetByIdAsync(id);
            if (Course == null)
            {
                return NotFound();
            }

            // Check if user is enrolled
            var allEnrollments = await _enrollmentService.GetAllAsync();
            Enrollment = allEnrollments.FirstOrDefault(e => e.UserId == userId && e.CourseId == id);
            if (Enrollment == null)
            {
                return RedirectToPage("/Courses/Enroll", new { id = id });
            }

            await LoadCourseData(userId, id);
            return Page();
        }

        private async Task LoadCourseData(int userId, int courseId)
        {
            try
            {
                // Get lessons for this course
                var allLessons = await _lessonService.GetAllAsync();
                Lessons = allLessons.Where(l => l.CourseId == courseId).OrderBy(l => l.SortOrder).ToList();
                TotalLessons = Lessons.Count;

                // Get quizzes for this course by finding lessons first, then quizzes for those lessons
                var allQuizzes = await _quizService.GetAllAsync();
                var lessonIds = Lessons.Select(l => l.LessonId).ToList();
                
                // Load quizzes that belong to lessons in this course
                Quizzes = allQuizzes.Where(q => lessonIds.Contains(q.LessonId)).ToList();
                TotalQuizzes = Quizzes.Count;
                
                // Debug: Log detailed information
                System.Diagnostics.Debug.WriteLine($"=== COURSE DATA DEBUG ===");
                System.Diagnostics.Debug.WriteLine($"Course ID: {courseId}, Title: {Course?.Title}");
                System.Diagnostics.Debug.WriteLine($"Lessons found: {TotalLessons}");
                foreach (var lesson in Lessons)
                {
                    System.Diagnostics.Debug.WriteLine($"  Lesson {lesson.LessonId}: {lesson.Title} (CourseId: {lesson.CourseId})");
                }
                
                System.Diagnostics.Debug.WriteLine($"Quizzes found: {TotalQuizzes}");
                foreach (var quiz in Quizzes)
                {
                    System.Diagnostics.Debug.WriteLine($"  Quiz {quiz.QuizId}: {quiz.Title} (LessonId: {quiz.LessonId})");
                }

                // Get lesson progress
                var allProgress = await _studentProgressService.GetAllAsync();
                LessonProgress = allProgress.Where(p => p.UserId == userId && p.CourseId == courseId).ToList();
                CompletedLessons = LessonProgress.Count(p => p.IsCompleted == true);

                // Get quiz attempts
                var allQuizAttempts = await _studentQuizAttemptService.GetAllAsync();
                QuizAttempts = allQuizAttempts.Where(a => a.UserId == userId && Quizzes.Any(q => q.QuizId == a.QuizId)).ToList();
                PassedQuizzes = QuizAttempts.Count(a => a.IsPassed == true);

                // Get recent activity
                RecentActivity = LessonProgress
                    .OrderByDescending(p => p.LastAccessedAt)
                    .Take(5)
                    .ToList();

                // Update enrollment progress
                if (TotalLessons > 0)
                {
                    Enrollment.ProgressPercentage = (decimal)CompletedLessons / TotalLessons * 100;
                    Enrollment.LastAccessedAt = DateTime.Now;
                    await _enrollmentService.UpdateAsync(Enrollment);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadCourseData: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}
