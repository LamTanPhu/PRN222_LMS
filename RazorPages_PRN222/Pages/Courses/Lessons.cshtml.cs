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
        private readonly CourseraStyleLMSContext _context;

        public LessonsModel(ILessonService lessonService, ICourseService courseService, CourseraStyleLMSContext context)
        {
            _lessonService = lessonService;
            _courseService = courseService;
            _context = context;
        }

        public List<Lesson> Lessons { get; set; } = new List<Lesson>();
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