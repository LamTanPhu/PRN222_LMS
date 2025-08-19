using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interface;
using Repository.Models;

namespace RazorPages_PRN222.Pages.Quiz
{
    public class IndexModel : PageModel
    {
        private readonly IQuizService _quizService;
        private readonly IStudentQuizAttemptService _attemptService;
        private readonly ICourseService _courseService;

        public IndexModel(IQuizService quizService, IStudentQuizAttemptService attemptService, ICourseService courseService)
        {
            _quizService = quizService;
            _attemptService = attemptService;
            _courseService = courseService;
        }

        [BindProperty]
        public int QuizId { get; set; }
        
        [BindProperty]
        public int CourseId { get; set; }
        
        public string CourseTitle { get; set; }
        public Repository.Models.Quiz Quiz { get; set; }
        public List<Repository.Models.QuizQuestion> Questions { get; set; } = new List<Repository.Models.QuizQuestion>();

        public async Task<IActionResult> OnGetAsync(int courseId)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Login");
            }

            CourseId = courseId;
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            // Get course information
            var course = await _courseService.GetByIdAsync(courseId);
            if (course == null)
            {
                return NotFound();
            }
            CourseTitle = course.Title;

            // Get quiz for this course (assuming quiz is linked to lesson)
            var lessons = course.Lessons?.ToList() ?? new List<Repository.Models.Lesson>();
            if (lessons.Any())
            {
                var firstLesson = lessons.First();
                var allQuizzes = await _quizService.GetAllAsync();
                Quiz = allQuizzes.FirstOrDefault(q => q.LessonId == firstLesson.LessonId);
                if (Quiz != null)
                {
                    var allQuestions = await _quizService.GetAllAsync(); // This should be GetQuestionsAsync
                    // For now, we'll use a placeholder since the interface doesn't have the right method
                    Questions = new List<Repository.Models.QuizQuestion>();
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Login");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            // Get form data for answers
            var answers = Request.Form["Answers"].ToList();
            
            // Create quiz attempt
            var attempt = new Repository.Models.StudentQuizAttempt
            {
                UserId = userId,
                QuizId = QuizId,
                StartedAt = DateTime.Now,
                CompletedAt = DateTime.Now,
                Score = CalculateScore(answers), // You'll need to implement this method
                IsPassed = true // You'll need to implement passing logic
            };

            await _attemptService.CreateAsync(attempt);

            return RedirectToPage("/Quiz/Result", new { attemptId = attempt.AttemptId });
        }

        private decimal CalculateScore(List<string> answers)
        {
            // Implement score calculation logic
            // This is a placeholder - you'll need to compare answers with correct answers
            return 0;
        }
    }
}

