using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Models;
using Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPages_PRN222.Pages.Courses
{
    public class QuizzesModel : PageModel
    {
        private readonly IQuizService _quizService;
        private readonly ICourseService _courseService;
        private readonly ILessonService _lessonService;
        private readonly IStudentQuizAttemptService _studentQuizAttemptService;

        public QuizzesModel(
            IQuizService quizService, 
            ICourseService courseService, 
            ILessonService lessonService,
            IStudentQuizAttemptService studentQuizAttemptService)
        {
            _quizService = quizService;
            _courseService = courseService;
            _lessonService = lessonService;
            _studentQuizAttemptService = studentQuizAttemptService;
        }

        public List<Repository.Models.Quiz> Quizzes { get; set; } = new List<Repository.Models.Quiz>();
        public List<StudentQuizAttempt> StudentAttempts { get; set; } = new List<StudentQuizAttempt>();
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }

        public async Task OnGetAsync(int id)
        {
            CourseId = id;
            
            // Get course title
            var course = await _courseService.GetByIdAsync(id);
            CourseTitle = course?.Title ?? "Unknown Course";
            
            // Get all lessons for this course
            var allLessons = await _lessonService.GetAllAsync();
            var courseLessons = allLessons.Where(l => l.CourseId == id).ToList();
            
            // Get all quizzes for lessons in this course
            var allQuizzes = await _quizService.GetAllAsync();
            Quizzes = allQuizzes.Where(q => courseLessons.Any(l => l.LessonId == q.LessonId)).ToList();
            
            // Get student quiz attempts (for current user - TODO: implement user authentication)
            var allAttempts = await _studentQuizAttemptService.GetAllAsync();
            StudentAttempts = allAttempts.Where(a => Quizzes.Any(q => q.QuizId == a.QuizId)).ToList();
        }
    }
}
