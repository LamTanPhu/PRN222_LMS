using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Models;
using Service.Interface;
using System.Security.Claims;

namespace RazorPages_PRN222.Pages.Learning
{
    [Authorize]
    public class QuizModel : PageModel
    {
        private readonly IQuizService _quizService;
        private readonly IQuizQuestionService _quizQuestionService;
        private readonly ILessonService _lessonService;
        private readonly ICourseService _courseService;
        private readonly IStudentQuizAttemptService _studentQuizAttemptService;
        private readonly IEnrollmentService _enrollmentService;

        public QuizModel(
            IQuizService quizService,
            IQuizQuestionService quizQuestionService,
            ILessonService lessonService,
            ICourseService courseService,
            IStudentQuizAttemptService studentQuizAttemptService,
            IEnrollmentService enrollmentService)
        {
            _quizService = quizService;
            _quizQuestionService = quizQuestionService;
            _lessonService = lessonService;
            _courseService = courseService;
            _studentQuizAttemptService = studentQuizAttemptService;
            _enrollmentService = enrollmentService;
        }

        public Repository.Models.Quiz Quiz { get; set; }
        public Lesson Lesson { get; set; }
        public Course Course { get; set; }
        public List<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
        public StudentQuizAttempt PreviousAttempt { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToPage("/Login");
            }
            
            try
            {
                // Get quiz with all related data
                Quiz = await _quizService.GetByIdAsync(id);
                if (Quiz == null)
                {
                    TempData["Error"] = $"Quiz with ID {id} not found.";
                    return RedirectToPage("/Learning/Index");
                }

                // Get lesson
                Lesson = await _lessonService.GetByIdAsync(Quiz.LessonId);
                if (Lesson == null)
                {
                    TempData["Error"] = "Lesson not found for this quiz.";
                    return RedirectToPage("/Learning/Index");
                }

                // Get course
                Course = await _courseService.GetByIdAsync(Lesson.CourseId);
                if (Course == null)
                {
                    TempData["Error"] = "Course not found for this lesson.";
                    return RedirectToPage("/Learning/Index");
                }

                // Check if user is enrolled
                var allEnrollments = await _enrollmentService.GetAllAsync();
                var enrollment = allEnrollments.FirstOrDefault(e => e.UserId == userId && e.CourseId == Lesson.CourseId);
                if (enrollment == null)
                {
                    return RedirectToPage("/Courses/Enroll", new { id = Lesson.CourseId });
                }

                // Load quiz questions
                await LoadQuizData(userId, id);
                
                return Page();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading the quiz. Please try again.";
                return RedirectToPage("/Learning/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(int quizId, Dictionary<int, string> answers)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToPage("/Login");
            }
            
            try
            {
                // Get quiz and questions
                Quiz = await _quizService.GetByIdAsync(quizId);
                if (Quiz == null)
                {
                    return NotFound();
                }

                var allQuestions = await _quizQuestionService.GetByQuizIdAsync(quizId);
                Questions = allQuestions.ToList();

                // Calculate score
                int correctAnswers = 0;
                int totalQuestions = Questions.Count;

                foreach (var question in Questions)
                {
                    if (answers.ContainsKey(question.QuestionId))
                    {
                        var studentAnswer = answers[question.QuestionId];
                        var correctAnswer = question.QuizAnswers.FirstOrDefault(a => a.IsCorrect == true);
                        
                        if (correctAnswer != null)
                        {
                            if (question.QuestionType == "Multiple Choice")
                            {
                                if (int.TryParse(studentAnswer, out int answerId) && answerId == correctAnswer.AnswerId)
                                {
                                    correctAnswers++;
                                }
                            }
                            else if (question.QuestionType == "True/False")
                            {
                                bool studentBool = bool.Parse(studentAnswer);
                                bool correctBool = bool.Parse(correctAnswer.AnswerText);
                                if (studentBool == correctBool)
                                {
                                    correctAnswers++;
                                }
                            }
                            // For Short Answer, we'll just count as correct for now
                            else if (question.QuestionType == "Short Answer")
                            {
                                if (!string.IsNullOrWhiteSpace(studentAnswer))
                                {
                                    correctAnswers++;
                                }
                            }
                        }
                    }
                }

                decimal score = totalQuestions > 0 ? (decimal)correctAnswers / totalQuestions * 100 : 0;
                bool isPassed = score >= (Quiz.PassingScore ?? 70);

                // Get previous attempt number
                var allAttempts = await _studentQuizAttemptService.GetAllAsync();
                var previousAttempts = allAttempts.Where(a => a.UserId == userId && a.QuizId == quizId).ToList();
                int attemptNumber = previousAttempts.Count + 1;

                // Create quiz attempt
                var quizAttempt = new StudentQuizAttempt
                {
                    UserId = userId,
                    QuizId = quizId,
                    AttemptNumber = attemptNumber,
                    Score = score,
                    StartedAt = DateTime.Now.AddMinutes(-30), // Assuming 30 minutes for the quiz
                    CompletedAt = DateTime.Now,
                    TimeSpent = 30, // Assuming 30 minutes
                    IsPassed = isPassed
                };

                var result = await _studentQuizAttemptService.CreateAsync(quizAttempt);

                // Redirect to results page
                return RedirectToPage("/Learning/QuizResult", new { attemptId = quizAttempt.AttemptId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while submitting the quiz. Please try again.";
                return RedirectToPage(new { id = quizId });
            }
        }

        private async Task LoadQuizData(int userId, int quizId)
        {
            try
            {
                // Load questions using QuizQuestionService
                var questions = await _quizQuestionService.GetByQuizIdAsync(quizId);
                Questions = questions ?? new List<QuizQuestion>();

                // Get previous attempt
                var allAttempts = await _studentQuizAttemptService.GetAllAsync();
                PreviousAttempt = allAttempts
                    .Where(a => a.UserId == userId && a.QuizId == quizId)
                    .OrderByDescending(a => a.AttemptNumber)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading quiz data. Please try again.";
            }
        }
    }
}
