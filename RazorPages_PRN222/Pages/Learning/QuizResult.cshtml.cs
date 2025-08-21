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
        private readonly IQuizQuestionService _quizQuestionService;
        private readonly ILessonService _lessonService;
        private readonly ICourseService _courseService;

        public QuizResultModel(
            IStudentQuizAttemptService studentQuizAttemptService,
            IQuizService quizService,
            IQuizQuestionService quizQuestionService,
            ILessonService lessonService,
            ICourseService courseService)
        {
            _studentQuizAttemptService = studentQuizAttemptService;
            _quizService = quizService;
            _quizQuestionService = quizQuestionService;
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
            public string QuestionType { get; set; }
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

            // Load actual quiz questions and calculate results
            await LoadQuizResults();
        }

        private async Task LoadQuizResults()
        {
            try
            {
                // Get all questions for this quiz
                var questions = await _quizQuestionService.GetByQuizIdAsync(QuizAttempt.QuizId);
                
                if (questions != null && questions.Any())
                {
                    // Calculate correct and incorrect answers based on score
                    int totalQuestions = questions.Count;
                    CorrectAnswers = (int)((QuizAttempt.Score ?? 0) / 100 * totalQuestions);
                    IncorrectAnswers = totalQuestions - CorrectAnswers;

                    // Create question results for display
                    foreach (var question in questions)
                    {
                        var correctAnswer = question.QuizAnswers?.FirstOrDefault(a => a.IsCorrect == true);
                        string correctAnswerText = correctAnswer?.AnswerText ?? "N/A";
                        
                        // For display purposes, we'll show a simplified version
                        // In a real system, you'd store the actual student answers
                        QuestionResults.Add(new QuestionResult
                        {
                            QuestionText = question.QuestionText,
                            StudentAnswer = "Answer submitted", // Placeholder since we don't store individual answers
                            CorrectAnswer = correctAnswerText,
                            IsCorrect = true, // This would be calculated based on actual student answers
                            QuestionType = question.QuestionType
                        });
                    }
                }
                else
                {
                    // Fallback if no questions found
                    CorrectAnswers = 0;
                    IncorrectAnswers = 0;
                }
            }
            catch (Exception ex)
            {
                // Handle error gracefully
                CorrectAnswers = 0;
                IncorrectAnswers = 0;
            }
        }
    }
}
