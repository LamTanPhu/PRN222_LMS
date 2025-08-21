using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Models;
using Service.Interface;
using System.Security.Claims;

namespace RazorPages_PRN222.Pages.Learning
{
    [Authorize]
    public class QuizResultModel : PageModel
    {
        private readonly IStudentQuizAttemptService _studentQuizAttemptService;
        private readonly IQuizService _quizService;
        private readonly ILessonService _lessonService;
        private readonly ICourseService _courseService;

        public QuizResultModel(
            IStudentQuizAttemptService studentQuizAttemptService,
            IQuizService quizService,
            ILessonService lessonService,
            ICourseService courseService)
        {
            _studentQuizAttemptService = studentQuizAttemptService;
            _quizService = quizService;
            _lessonService = lessonService;
            _courseService = courseService;
        }

        public StudentQuizAttempt QuizAttempt { get; set; }
        public Repository.Models.Quiz Quiz { get; set; }
        public Lesson Lesson { get; set; }
        public Course Course { get; set; }
        public List<QuestionResult> QuestionResults { get; set; } = new List<QuestionResult>();
        public int CorrectAnswers { get; set; }
        public int IncorrectAnswers { get; set; }

        public class QuestionResult
        {
            public string QuestionText { get; set; }
            public string StudentAnswer { get; set; }
            public string CorrectAnswer { get; set; }
            public bool IsCorrect { get; set; }
        }

        public async Task OnGetAsync(int attemptId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return;
            }
            
            // Get quiz attempt
            var allAttempts = await _studentQuizAttemptService.GetAllAsync();
            QuizAttempt = allAttempts.FirstOrDefault(a => a.AttemptId == attemptId && a.UserId == userId);
            if (QuizAttempt == null)
            {
                return;
            }

            // Get quiz
            Quiz = await _quizService.GetByIdAsync(QuizAttempt.QuizId);
            if (Quiz == null)
            {
                return;
            }

            // Get lesson
            Lesson = await _lessonService.GetByIdAsync(Quiz.LessonId);
            if (Lesson == null)
            {
                return;
            }

            // Get course
            Course = await _courseService.GetByIdAsync(Lesson.CourseId);
            if (Course == null)
            {
                return;
            }

            // For now, we'll create sample question results since we don't store individual answers
            // In a real implementation, you would store and retrieve the actual answers
            await LoadSampleQuestionResults();
        }

        private async Task LoadSampleQuestionResults()
        {
            // This is a simplified implementation
            // In a real system, you would store the actual student answers and retrieve them here
            CorrectAnswers = (int)((QuizAttempt.Score ?? 0) / 100 * 10); // Assuming 10 questions
            IncorrectAnswers = 10 - CorrectAnswers;

            // Create sample question results
            for (int i = 1; i <= 10; i++)
            {
                bool isCorrect = i <= CorrectAnswers;
                QuestionResults.Add(new QuestionResult
                {
                    QuestionText = $"Sample Question {i}",
                    StudentAnswer = isCorrect ? "Correct Answer" : "Wrong Answer",
                    CorrectAnswer = "Correct Answer",
                    IsCorrect = isCorrect
                });
            }
        }
    }
}
