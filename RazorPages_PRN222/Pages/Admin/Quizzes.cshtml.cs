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
    }
}
