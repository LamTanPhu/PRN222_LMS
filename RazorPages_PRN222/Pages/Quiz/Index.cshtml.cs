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
        private readonly IQuizQuestionService _quizQuestionService;
        private readonly ILessonService _lessonService;

        public IndexModel(
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

        // Form binding properties
        [BindProperty]
        public int QuizId { get; set; }
        
        [BindProperty]
        public int CourseId { get; set; }
        
        // Display properties
        public string CourseTitle { get; set; } = string.Empty;
        public Repository.Models.Quiz? Quiz { get; set; }
        public List<Repository.Models.QuizQuestion> Questions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int courseId)
        {
            // Check authentication
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Login");
            }

            // Set course ID
            CourseId = courseId;

            // Get user ID from claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return RedirectToPage("/Login");
            }

            try
            {
                // Get course information
                var course = await _courseService.GetByIdAsync(courseId);
                if (course == null)
                {
                    return NotFound();
                }
                CourseTitle = course.Title;

                // Debug: Log course information
                System.Diagnostics.Debug.WriteLine($"=== QUIZ PAGE DEBUG ===");
                System.Diagnostics.Debug.WriteLine($"Course ID: {courseId}, Title: {course.Title}");

                // Get all lessons for this course
                var allLessons = await _lessonService.GetAllAsync();
                var courseLessons = allLessons.Where(l => l.CourseId == courseId).OrderBy(l => l.SortOrder).ToList();
                
                System.Diagnostics.Debug.WriteLine($"Lessons found: {courseLessons.Count}");
                foreach (var lesson in courseLessons)
                {
                    System.Diagnostics.Debug.WriteLine($"  Lesson {lesson.LessonId}: {lesson.Title}");
                }

                // Get all quizzes and find ones that belong to lessons in this course
                var allQuizzes = await _quizService.GetAllAsync();
                var lessonIds = courseLessons.Select(l => l.LessonId).ToList();
                var courseQuizzes = allQuizzes.Where(q => lessonIds.Contains(q.LessonId)).ToList();
                
                System.Diagnostics.Debug.WriteLine($"Quizzes found: {courseQuizzes.Count}");
                foreach (var quiz in courseQuizzes)
                {
                    System.Diagnostics.Debug.WriteLine($"  Quiz {quiz.QuizId}: {quiz.Title} (LessonId: {quiz.LessonId})");
                }

                // Select the first available quiz (or you could show a list to choose from)
                if (courseQuizzes.Any())
                {
                    Quiz = courseQuizzes.First();
                    QuizId = Quiz.QuizId;
                    
                    System.Diagnostics.Debug.WriteLine($"Selected Quiz: {Quiz.QuizId} - {Quiz.Title}");
                    
                    // Load questions for this quiz
                    Questions = await _quizQuestionService.GetByQuizIdAsync(Quiz.QuizId);
                    
                    if (Questions != null && Questions.Any())
                    {
                        System.Diagnostics.Debug.WriteLine($"✓ Successfully loaded {Questions.Count} questions for quiz {Quiz.QuizId}");
                        
                        // Load answers for each question if needed
                        foreach (var question in Questions)
                        {
                            System.Diagnostics.Debug.WriteLine($"  Question {question.QuestionId}: {question.QuestionText.Substring(0, Math.Min(50, question.QuestionText.Length))}...");
                            System.Diagnostics.Debug.WriteLine($"    Type: {question.QuestionType}, Points: {question.Points}");
                            System.Diagnostics.Debug.WriteLine($"    Answers: {question.QuizAnswers?.Count ?? 0}");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"✗ No questions found for quiz {Quiz.QuizId}");
                        Questions = new List<Repository.Models.QuizQuestion>();
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("✗ No quizzes found for this course");
                    Quiz = null;
                    Questions = new List<Repository.Models.QuizQuestion>();
                }

                return Page();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in Quiz OnGetAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                // Log error and return error page
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Check authentication
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Login");
            }

            // Get user ID from claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return RedirectToPage("/Login");
            }

            try
            {
                // Load questions with answers
                Questions = await _quizQuestionService.GetByQuizIdAsync(QuizId);
                
                if (Questions == null || !Questions.Any())
                {
                    return RedirectToPage("/Error");
                }

                // DEBUG: Log questions and answers
                System.Diagnostics.Debug.WriteLine("=== DEBUG QUIZ ===");
                System.Diagnostics.Debug.WriteLine($"QuizId: {QuizId}");
                System.Diagnostics.Debug.WriteLine($"Questions count: {Questions.Count}");
                
                foreach (var q in Questions)
                {
                    System.Diagnostics.Debug.WriteLine($"Q: {q.QuestionText}");
                    System.Diagnostics.Debug.WriteLine($"  QuizAnswers count: {q.QuizAnswers?.Count ?? 0}");
                    if (q.QuizAnswers != null)
                    {
                        foreach (var a in q.QuizAnswers)
                        {
                            System.Diagnostics.Debug.WriteLine($"    - {a.AnswerId}: {a.AnswerText} (IsCorrect={a.IsCorrect})");
                        }
                    }
                }

                // Get form data - answers array
                var answers = new List<string>();
                
                // Get answers from form array format: Answers[0], Answers[1], Answers[2]
                for (int i = 0; i < Questions.Count; i++)
                {
                    var answerKey = $"Answers[{i}]";
                    var answerValue = Request.Form[answerKey].FirstOrDefault();
                    if (!string.IsNullOrEmpty(answerValue))
                    {
                        answers.Add(answerValue);
                    }
                }
                
                System.Diagnostics.Debug.WriteLine($"Form Answers count: {answers.Count}");
                System.Diagnostics.Debug.WriteLine($"Form Answers: {string.Join(",", answers)}");
                
                // DEBUG: Check all form data
                System.Diagnostics.Debug.WriteLine("=== FORM DATA DEBUG ===");
                foreach (var key in Request.Form.Keys)
                {
                    var values = Request.Form[key].ToList();
                    System.Diagnostics.Debug.WriteLine($"Form[{key}] = {string.Join(",", values)}");
                }
                System.Diagnostics.Debug.WriteLine("=== END FORM DATA DEBUG ===");

                // Calculate score
                var score = CalculateScore(answers);
                var isPassed = score >= 50;
                
                System.Diagnostics.Debug.WriteLine($"Calculated score: {score}");
                System.Diagnostics.Debug.WriteLine($"IsPassed: {isPassed}");
                System.Diagnostics.Debug.WriteLine("=== END DEBUG ===");

                // Create quiz attempt record
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

                System.Diagnostics.Debug.WriteLine("=== ATTEMPT DATA ===");
                System.Diagnostics.Debug.WriteLine($"UserId: {userId}");
                System.Diagnostics.Debug.WriteLine($"QuizId: {QuizId}");
                System.Diagnostics.Debug.WriteLine($"Score: {score}");
                System.Diagnostics.Debug.WriteLine($"IsPassed: {isPassed}");
                System.Diagnostics.Debug.WriteLine("=== END ATTEMPT DATA ===");

                // Save to database
                System.Diagnostics.Debug.WriteLine("=== SAVING TO DATABASE ===");
                await _attemptService.CreateAsync(attempt);
                System.Diagnostics.Debug.WriteLine($"Save completed successfully");
                System.Diagnostics.Debug.WriteLine($"Attempt ID after save: {attempt.AttemptId}");
                System.Diagnostics.Debug.WriteLine("=== END SAVING ===");

                // Redirect to result page
                return RedirectToPage("/Quiz/Result", new { attemptId = attempt.AttemptId });
            }
            catch (Exception ex)
            {
                // Log error and return error page
                System.Diagnostics.Debug.WriteLine($"ERROR: {ex.Message}");
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

            // Process each answer
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

            // Calculate percentage
            return totalQuestions > 0 ? (decimal)correctAnswers / totalQuestions * 100 : 0;
        }
    }
}

