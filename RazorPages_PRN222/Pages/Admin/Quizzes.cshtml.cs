using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Models;
using Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPages_PRN222.Pages.Admin
{
    public class QuizzesModel : PageModel
    {
        private readonly IQuizService _quizService;
        private readonly ICourseService _courseService;
        private readonly ILessonService _lessonService;

        public QuizzesModel(IQuizService quizService, ICourseService courseService, ILessonService lessonService)
        {
            _quizService = quizService;
            _courseService = courseService;
            _lessonService = lessonService;
        }

        public List<Repository.Models.Quiz> Quizzes { get; set; } = new List<Repository.Models.Quiz>();
        public List<Course> Courses { get; set; } = new List<Course>();
        public List<Lesson> Lessons { get; set; } = new List<Lesson>();

        public async Task OnGetAsync()
        {
            // Get all quizzes, courses, and lessons for admin (without circular references)
            Quizzes = await _quizService.GetAllAsync();
            Courses = await _courseService.GetCoursesForAdminAsync();
            Lessons = await _lessonService.GetLessonsForAdminAsync();
        }

        public async Task<IActionResult> OnPostAddQuizAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Get form data
            var lessonId = int.Parse(Request.Form["LessonId"]);
            var title = Request.Form["Title"];
            var description = Request.Form["Description"];
            var timeLimit = Request.Form["TimeLimit"];
            var passingScore = decimal.Parse(Request.Form["PassingScore"]);
            var maxAttempts = int.Parse(Request.Form["MaxAttempts"]);
            var isRandomized = Request.Form.ContainsKey("IsRandomized");

            // Create new quiz
            var quiz = new Repository.Models.Quiz
            {
                LessonId = lessonId,
                Title = title,
                Description = description,
                TimeLimit = !string.IsNullOrEmpty(timeLimit) ? int.Parse(timeLimit) : null,
                PassingScore = passingScore,
                MaxAttempts = maxAttempts,
                IsRandomized = isRandomized
            };

            await _quizService.CreateAsync(quiz);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteQuizAsync(int quizId)
        {
            await _quizService.DeleteAsync(quizId);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddSampleQuestionsAsync(int quizId)
        {
            // Get the quiz
            var quiz = await _quizService.GetByIdAsync(quizId);
            if (quiz == null)
            {
                return RedirectToPage();
            }

            // Get the lesson to determine course context
            var lesson = await _lessonService.GetByIdAsync(quiz.LessonId);
            if (lesson == null)
            {
                return RedirectToPage();
            }

            // Add sample questions based on the course
            var questions = new List<QuizQuestion>();

            if (lesson.CourseId == 1) // Web Development Bootcamp
            {
                questions.Add(new QuizQuestion
                {
                    QuizId = quizId,
                    QuestionText = "What does HTML stand for?",
                    QuestionType = "Multiple Choice",
                    SortOrder = 1,
                    QuizAnswers = new List<QuizAnswer>
                    {
                        new QuizAnswer { AnswerText = "HyperText Markup Language", IsCorrect = true, SortOrder = 1 },
                        new QuizAnswer { AnswerText = "High Tech Modern Language", IsCorrect = false, SortOrder = 2 },
                        new QuizAnswer { AnswerText = "Home Tool Markup Language", IsCorrect = false, SortOrder = 3 },
                        new QuizAnswer { AnswerText = "Hyperlink and Text Markup Language", IsCorrect = false, SortOrder = 4 }
                    }
                });

                questions.Add(new QuizQuestion
                {
                    QuizId = quizId,
                    QuestionText = "Which CSS property is used to change the text color?",
                    QuestionType = "Multiple Choice",
                    SortOrder = 2,
                    QuizAnswers = new List<QuizAnswer>
                    {
                        new QuizAnswer { AnswerText = "text-color", IsCorrect = false, SortOrder = 1 },
                        new QuizAnswer { AnswerText = "color", IsCorrect = true, SortOrder = 2 },
                        new QuizAnswer { AnswerText = "font-color", IsCorrect = false, SortOrder = 3 },
                        new QuizAnswer { AnswerText = "text-style", IsCorrect = false, SortOrder = 4 }
                    }
                });

                questions.Add(new QuizQuestion
                {
                    QuizId = quizId,
                    QuestionText = "JavaScript is a programming language.",
                    QuestionType = "True/False",
                    SortOrder = 3,
                    QuizAnswers = new List<QuizAnswer>
                    {
                        new QuizAnswer { AnswerText = "True", IsCorrect = true, SortOrder = 1 },
                        new QuizAnswer { AnswerText = "False", IsCorrect = false, SortOrder = 2 }
                    }
                });
            }
            else if (lesson.CourseId == 2) // Digital Marketing Masterclass
            {
                questions.Add(new QuizQuestion
                {
                    QuizId = quizId,
                    QuestionText = "What does SEO stand for?",
                    QuestionType = "Multiple Choice",
                    SortOrder = 1,
                    QuizAnswers = new List<QuizAnswer>
                    {
                        new QuizAnswer { AnswerText = "Search Engine Optimization", IsCorrect = true, SortOrder = 1 },
                        new QuizAnswer { AnswerText = "Social Engine Optimization", IsCorrect = false, SortOrder = 2 },
                        new QuizAnswer { AnswerText = "Search Engine Organization", IsCorrect = false, SortOrder = 3 },
                        new QuizAnswer { AnswerText = "Social Engine Organization", IsCorrect = false, SortOrder = 4 }
                    }
                });

                questions.Add(new QuizQuestion
                {
                    QuizId = quizId,
                    QuestionText = "Email marketing is still effective in digital marketing.",
                    QuestionType = "True/False",
                    SortOrder = 2,
                    QuizAnswers = new List<QuizAnswer>
                    {
                        new QuizAnswer { AnswerText = "True", IsCorrect = true, SortOrder = 1 },
                        new QuizAnswer { AnswerText = "False", IsCorrect = false, SortOrder = 2 }
                    }
                });
            }
            else // React.js Advanced Patterns
            {
                questions.Add(new QuizQuestion
                {
                    QuizId = quizId,
                    QuestionText = "What is a React Hook?",
                    QuestionType = "Multiple Choice",
                    SortOrder = 1,
                    QuizAnswers = new List<QuizAnswer>
                    {
                        new QuizAnswer { AnswerText = "A function that lets you use state and other React features", IsCorrect = true, SortOrder = 1 },
                        new QuizAnswer { AnswerText = "A component that hooks into the DOM", IsCorrect = false, SortOrder = 2 },
                        new QuizAnswer { AnswerText = "A way to connect to external APIs", IsCorrect = false, SortOrder = 3 },
                        new QuizAnswer { AnswerText = "A debugging tool", IsCorrect = false, SortOrder = 4 }
                    }
                });

                questions.Add(new QuizQuestion
                {
                    QuizId = quizId,
                    QuestionText = "useState is a React Hook.",
                    QuestionType = "True/False",
                    SortOrder = 2,
                    QuizAnswers = new List<QuizAnswer>
                    {
                        new QuizAnswer { AnswerText = "True", IsCorrect = true, SortOrder = 1 },
                        new QuizAnswer { AnswerText = "False", IsCorrect = false, SortOrder = 2 }
                    }
                });
            }

            // Add the questions to the database
            foreach (var question in questions)
            {
                await _quizService.AddQuestionToQuizAsync(quizId, question);
            }

            TempData["Message"] = $"Added {questions.Count} sample questions to the quiz!";
            return RedirectToPage();
        }
    }
}
