using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Models;
using Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPages_PRN222.Pages.Admin
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
        public List<Course> Courses { get; set; } = new List<Course>();

        public async Task OnGetAsync()
        {
            // Get all lessons and courses for admin (without circular references)
            Lessons = await _lessonService.GetLessonsForAdminAsync();
            Courses = await _courseService.GetCoursesForAdminAsync();
        }

        public async Task<IActionResult> OnPostAddLessonAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Get form data
            var courseId = int.Parse(Request.Form["CourseId"]);
            var title = Request.Form["Title"];
            var description = Request.Form["Description"];
            var lessonType = Request.Form["LessonType"];
            var duration = Request.Form["Duration"];
            var sortOrder = int.Parse(Request.Form["SortOrder"]);
            var contentUrl = Request.Form["ContentUrl"];
            var isPreview = Request.Form.ContainsKey("IsPreview");

            // Create new lesson
            var lesson = new Lesson
            {
                CourseId = courseId,
                Title = title,
                Description = description,
                LessonType = lessonType,
                Duration = !string.IsNullOrEmpty(duration) ? int.Parse(duration) : null,
                SortOrder = sortOrder,
                ContentUrl = contentUrl,
                IsPreview = isPreview,
                CreatedAt = System.DateTime.Now
            };

            await _lessonService.CreateAsync(lesson);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteLessonAsync(int lessonId)
        {
            await _lessonService.DeleteAsync(lessonId);
            return RedirectToPage();
        }
    }
}
