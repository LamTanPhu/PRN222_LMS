using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public LessonsModel(ILessonService lessonService, ICourseService courseService)
        {
            _lessonService = lessonService;
            _courseService = courseService;
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
    }
}
