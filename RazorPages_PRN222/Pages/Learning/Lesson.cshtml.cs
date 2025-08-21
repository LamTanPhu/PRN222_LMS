using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Models;
using Service.Interface;
using System.Security.Claims;

namespace RazorPages_PRN222.Pages.Learning
{
    [Authorize]
    public class LessonModel : PageModel
    {
        private readonly ILessonService _lessonService;
        private readonly ICourseService _courseService;
        private readonly IQuizService _quizService;
        private readonly IQuizQuestionService _quizQuestionService;
        private readonly IStudentProgressService _studentProgressService;
        private readonly IStudentQuizAttemptService _studentQuizAttemptService;
        private readonly IEnrollmentService _enrollmentService;

        public LessonModel(
            ILessonService lessonService,
            ICourseService courseService,
            IQuizService quizService,
            IQuizQuestionService quizQuestionService,
            IStudentProgressService studentProgressService,
            IStudentQuizAttemptService studentQuizAttemptService,
            IEnrollmentService enrollmentService)
        {
            _lessonService = lessonService;
            _courseService = courseService;
            _quizService = quizService;
            _quizQuestionService = quizQuestionService;
            _studentProgressService = studentProgressService;
            _studentQuizAttemptService = studentQuizAttemptService;
            _enrollmentService = enrollmentService;
        }

        public Lesson Lesson { get; set; }
        public Course Course { get; set; }
        public StudentProgress Progress { get; set; }
        public List<Repository.Models.Quiz> Quizzes { get; set; } = new List<Repository.Models.Quiz>();
        public List<StudentQuizAttempt> QuizAttempts { get; set; } = new List<StudentQuizAttempt>();
        public Lesson PreviousLesson { get; set; }
        public Lesson NextLesson { get; set; }
        public List<Lesson> AllLessons { get; set; } = new List<Lesson>();

        public int CourseProgress { get; set; }
        public int CompletedLessons { get; set; }
        public int TotalLessons { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToPage("/Login");
            }
            
            // Get lesson
            Lesson = await _lessonService.GetByIdAsync(id);
            if (Lesson == null)
            {
                return NotFound();
            }

            // Get course
            Course = await _courseService.GetByIdAsync(Lesson.CourseId);
            if (Course == null)
            {
                return NotFound();
            }

            // Check if user is enrolled
            var allEnrollments = await _enrollmentService.GetAllAsync();
            var enrollment = allEnrollments.FirstOrDefault(e => e.UserId == userId && e.CourseId == Lesson.CourseId);
            if (enrollment == null)
            {
                return RedirectToPage("/Courses/Enroll", new { id = Lesson.CourseId });
            }

            await LoadLessonData(userId, id);
            return Page();
        }

        public async Task<IActionResult> OnPostTestLoadQuizQuestionsAsync(int quizId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToPage("/Login");
            }
            
            try
            {
                // Try to load questions manually for the specific quiz
                var manualQuestions = await _quizQuestionService.GetByQuizIdAsync(quizId);
                if (manualQuestions != null && manualQuestions.Count > 0)
                {
                    TempData["Success"] = $"Successfully loaded {manualQuestions.Count} questions for quiz {quizId}!";
                    
                    // Update the quiz object with manually loaded questions
                    var quiz = Quizzes.FirstOrDefault(q => q.QuizId == quizId);
                    if (quiz != null)
                    {
                        quiz.QuizQuestions = manualQuestions;
                    }
                }
                else
                {
                    TempData["Warning"] = $"No questions found for quiz {quizId} when trying to load manually.";
                }
                
                // Redirect back to the same page
                return RedirectToPage(new { id = Lesson.LessonId });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in TestLoadQuizQuestions: {ex.Message}");
                TempData["Error"] = $"Error testing question load: {ex.Message}";
                return RedirectToPage(new { id = Lesson.LessonId });
            }
        }

        public async Task<IActionResult> OnPostMarkCompleteAsync(int lessonId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToPage("/Login");
            }
            
            // Get or create progress record
            var allProgress = await _studentProgressService.GetAllAsync();
            var progress = allProgress.FirstOrDefault(p => p.UserId == userId && p.LessonId == lessonId);
            
            if (progress == null)
            {
                var lesson = await _lessonService.GetByIdAsync(lessonId);
                progress = new StudentProgress
                {
                    UserId = userId,
                    LessonId = lessonId,
                    CourseId = lesson.CourseId,
                    IsCompleted = true,
                    CompletedAt = DateTime.Now,
                    LastAccessedAt = DateTime.Now,
                    TimeSpent = 30 // Default time spent
                };
                await _studentProgressService.CreateAsync(progress);
            }
            else
            {
                progress.IsCompleted = true;
                progress.CompletedAt = DateTime.Now;
                progress.LastAccessedAt = DateTime.Now;
                await _studentProgressService.UpdateAsync(progress);
            }

            TempData["Message"] = "Lesson marked as complete!";
            return RedirectToPage(new { id = lessonId });
        }

        private async Task LoadLessonData(int userId, int lessonId)
        {
            try
            {
                // Get all lessons in the course
                var allLessons = await _lessonService.GetAllAsync();
                AllLessons = allLessons.Where(l => l.CourseId == Lesson.CourseId).OrderBy(l => l.SortOrder).ToList();
                TotalLessons = AllLessons.Count;

                // Debug: Log lesson information
                System.Diagnostics.Debug.WriteLine($"Lesson {lessonId}: Found {AllLessons.Count} lessons in course {Lesson.CourseId}");

                // Get current lesson index
                var currentIndex = AllLessons.FindIndex(l => l.LessonId == lessonId);
                
                // Get previous and next lessons
                if (currentIndex > 0)
                {
                    PreviousLesson = AllLessons[currentIndex - 1];
                }
                if (currentIndex < AllLessons.Count - 1)
                {
                    NextLesson = AllLessons[currentIndex + 1];
                }

                // Get progress for this lesson
                var allProgress = await _studentProgressService.GetAllAsync();
                Progress = allProgress.FirstOrDefault(p => p.UserId == userId && p.LessonId == lessonId);
                
                // Update or create progress record
                if (Progress == null)
                {
                    Progress = new StudentProgress
                    {
                        UserId = userId,
                        LessonId = lessonId,
                        CourseId = Lesson.CourseId,
                        IsCompleted = false,
                        LastAccessedAt = DateTime.Now,
                        TimeSpent = 0
                    };
                    await _studentProgressService.CreateAsync(Progress);
                }
                else
                {
                    Progress.LastAccessedAt = DateTime.Now;
                    await _studentProgressService.UpdateAsync(Progress);
                }

                // Get quizzes for this lesson - try to load with questions
                var allQuizzes = await _quizService.GetAllAsync();
                Quizzes = allQuizzes.Where(q => q.LessonId == lessonId).ToList();

                // Debug: Log quiz information
                System.Diagnostics.Debug.WriteLine($"Lesson {lessonId}: Found {Quizzes.Count} quizzes");
                foreach (var quiz in Quizzes)
                {
                    System.Diagnostics.Debug.WriteLine($"  Quiz {quiz.QuizId}: {quiz.Title}, Questions: {quiz.QuizQuestions?.Count ?? 0}");
                    
                    // If quiz doesn't have questions loaded, try to load them manually
                    if (quiz.QuizQuestions == null || quiz.QuizQuestions.Count == 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"    Quiz {quiz.QuizId} has no questions, trying to load manually...");
                        try
                        {
                            var manualQuestions = await _quizQuestionService.GetByQuizIdAsync(quiz.QuizId);
                            if (manualQuestions != null && manualQuestions.Count > 0)
                            {
                                System.Diagnostics.Debug.WriteLine($"    Successfully loaded {manualQuestions.Count} questions manually for quiz {quiz.QuizId}");
                                // Update the quiz object with manually loaded questions
                                quiz.QuizQuestions = manualQuestions;
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"    No questions found manually for quiz {quiz.QuizId}");
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"    Error loading questions for quiz {quiz.QuizId}: {ex.Message}");
                        }
                    }
                    else
                    {
                        foreach (var question in quiz.QuizQuestions)
                        {
                            System.Diagnostics.Debug.WriteLine($"    Question {question.QuestionId}: {question.QuestionText}, Answers: {question.QuizAnswers?.Count ?? 0}");
                        }
                    }
                }

                // Get quiz attempts
                var allQuizAttempts = await _studentQuizAttemptService.GetAllAsync();
                QuizAttempts = allQuizAttempts.Where(a => a.UserId == userId && Quizzes.Any(q => q.QuizId == a.QuizId)).ToList();

                // Calculate course progress
                var courseProgress = allProgress.Where(p => p.UserId == userId && p.CourseId == Lesson.CourseId).ToList();
                CompletedLessons = courseProgress.Count(p => p.IsCompleted == true);
                CourseProgress = TotalLessons > 0 ? (int)((decimal)CompletedLessons / TotalLessons * 100) : 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading lesson data: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}
