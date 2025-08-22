/*
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interface;
using Repository.Models;

namespace RazorPages_PRN222.Pages.Learning
{
    public class QuizModel : PageModel
    {
        private readonly IQuizService _quizService;
        private readonly IStudentQuizAttemptService _attemptService;
        private readonly ICourseService _courseService;
        private readonly IQuizQuestionService _quizQuestionService;
        private readonly ILessonService _lessonService;

        public QuizModel(
            IQuizService quizService, 
            IStudentQuizAttemptService attemptService, 
            ICourseService courseService, 
            IQuizQuestionService quizQuestionService,
            ILessonService lessonService)
        {
            _quizService = quizService;
            _attemptService = attemptService;
            _courseService = courseService;
            _quizQuestionService = quizQuestionService;
            _lessonService = lessonService;
        }

        [BindProperty]
        public int QuizId { get; set; }
        
        public Repository.Models.Quiz? Quiz { get; set; }
        public List<Repository.Models.QuizQuestion> Questions { get; set; } = new();
        public Repository.Models.Lesson? Lesson { get; set; }
        public Repository.Models.Course? Course { get; set; }

        public async Task<IActionResult> OnGetAsync(int quizId)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Login");
            }

            QuizId = quizId;

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return RedirectToPage("/Login");
            }

            try
            {
                Quiz = await _quizService.GetByIdAsync(quizId);
                if (Quiz == null)
                {
                    return NotFound();
                }

                Lesson = await _lessonService.GetByIdAsync(Quiz.LessonId);
                if (Lesson != null)
                {
                    Course = await _courseService.GetByIdAsync(Lesson.CourseId);
                }

                Questions = await _quizQuestionService.GetByQuizIdAsync(Quiz.QuizId);
                
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Login");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return RedirectToPage("/Login");
            }

            try
            {
                Questions = await _quizQuestionService.GetByQuizIdAsync(QuizId);
                
                if (Questions == null || !Questions.Any())
                {
                    return RedirectToPage("/Error");
                }

                var answers = new List<string>();
                
                for (int i = 0; i < Questions.Count; i++)
                {
                    var answerKey = $"Answers[{i}]";
                    var answerValue = Request.Form[answerKey].FirstOrDefault();
                    if (!string.IsNullOrEmpty(answerValue))
                    {
                        answers.Add(answerValue);
                    }
                }

                var score = CalculateScore(answers);
                var isPassed = score >= 50;

                var attempt = new Repository.Models.StudentQuizAttempt
                {
                    UserId = userId,
                    QuizId = QuizId,
                    AttemptNumber = 1,
                    StartedAt = DateTime.Now,
                    CompletedAt = DateTime.Now,
                    Score = score,
                    IsPassed = isPassed
                };

                await _attemptService.CreateAsync(attempt);

                return RedirectToPage("/Learning/QuizResult", new { attemptId = attempt.AttemptId });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error");
            }
        }

        private decimal CalculateScore(List<string> answers)
        {
            if (Questions == null || !Questions.Any() || !answers.Any())
            {
                return 0;
            }

            int correctAnswers = 0;
            int totalQuestions = Questions.Count;

            for (int i = 0; i < answers.Count && i < totalQuestions; i++)
            {
                if (int.TryParse(answers[i], out int selectedAnswerId))
                {
                    var question = Questions[i];
                    var correctAnswer = question.QuizAnswers?.FirstOrDefault(a => a.IsCorrect == true);
                    
                    if (correctAnswer != null && correctAnswer.AnswerId == selectedAnswerId)
                    {
                        correctAnswers++;
                    }
                }
            }

            return totalQuestions > 0 ? (decimal)correctAnswers / totalQuestions * 100 : 0;
        }
    }
}
*/

/*
OLD LEARNING QUIZ PAGE MODEL - NO LONGER USED
This page model has been replaced by /Quiz/Index.cshtml.cs for lesson-based quizzes.
The content above is preserved for reference but commented out.
*/
