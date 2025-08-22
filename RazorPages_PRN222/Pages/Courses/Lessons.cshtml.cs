using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Repository.DBContext;
using Repository.Models;
using Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPages_PRN222.Pages.Courses
{
    public class LessonsModel : PageModel
    {
        private readonly ILessonService _lessonService;
        private readonly ICourseService _courseService;
        private readonly IQuizService _quizService;
        private readonly IStudentQuizAttemptService _quizAttemptService;
        private readonly CourseraStyleLMSContext _context;

        public LessonsModel(ILessonService lessonService, ICourseService courseService, IQuizService quizService, IStudentQuizAttemptService quizAttemptService, CourseraStyleLMSContext context)
        {
            _lessonService = lessonService;
            _courseService = courseService;
            _quizService = quizService;
            _quizAttemptService = quizAttemptService;
            _context = context;
        }

        public List<Lesson> Lessons { get; set; } = new List<Lesson>();
        //public List<Quiz> CourseQuizzes { get; set; } = new List<Quiz>();
        public List<Repository.Models.Quiz> CourseQuizzes { get; set; } = new List<Repository.Models.Quiz>();
        public List<StudentQuizAttempt> QuizAttempts { get; set; } = new List<StudentQuizAttempt>();
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
            Lessons = allLessons.Where(l => l.CourseId == id).ToList();
            
            // Get all quizzes for this course
            var allQuizzes = await _quizService.GetAllAsync();
            var lessonIds = Lessons.Select(l => l.LessonId).ToList();
            CourseQuizzes = allQuizzes.Where(q => lessonIds.Contains(q.LessonId)).ToList();
            
            // Get quiz attempts for the current user
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var userId))
                {
                    var allAttempts = await _quizAttemptService.GetAllAsync();
                    QuizAttempts = allAttempts.Where(a => a.UserId == userId && CourseQuizzes.Any(q => q.QuizId == a.QuizId)).ToList();
                }
            }
        }
        public async Task<IActionResult> OnPostMarkCompletedAsync(int courseId, int lessonId)
        {
            var lesson = await _context.Lessons
        .FirstOrDefaultAsync(l => l.CourseId == courseId && l.LessonId == lessonId);

            if (lesson == null)
                return NotFound();

            lesson.IsCompleted = true;

            _context.Lessons.Update(lesson); 
            await _context.SaveChangesAsync();

            Console.WriteLine($"DEBUG: Lesson {lessonId} marked completed in course {courseId}");

            return RedirectToPage(new { id = courseId });
        }
    }
}