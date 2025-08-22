using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Models;
using Service.Interface;
using System;
using System.Collections.Generic;
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
            Lessons = await _lessonService.GetLessonsForAdminAsync();
            Courses = await _courseService.GetCoursesForAdminAsync();
        }

        public async Task<IActionResult> OnGetLessonAsync(int id)
        {
            var lesson = await _lessonService.GetByIdAsync(id);
            if (lesson == null)
            {
                return NotFound();
            }

            return new JsonResult(new
            {
                lessonId = lesson.LessonId,
                courseId = lesson.CourseId,
                title = lesson.Title,
                description = lesson.Description,
                lessonType = lesson.LessonType,
                duration = lesson.Duration,
                sortOrder = lesson.SortOrder,
                contentUrl = lesson.ContentUrl,
                isPreview = lesson.IsPreview
            });
        }

        public async Task<IActionResult> OnPostAddLessonAsync()
        {
            try
            {
                var courseIdStr = Request.Form["CourseId"];
                var title = Request.Form["Title"];
                var description = Request.Form["Description"];
                var lessonType = Request.Form["LessonType"];
                var durationStr = Request.Form["Duration"];
                var sortOrderStr = Request.Form["SortOrder"];
                var contentUrl = Request.Form["ContentUrl"];
                var isPreview = Request.Form.ContainsKey("IsPreview");

                // Validate inputs
                if (string.IsNullOrWhiteSpace(courseIdStr) || !int.TryParse(courseIdStr, out int courseId))
                {
                    TempData["ErrorMessage"] = "Invalid course selection.";
                    await LoadDataAsync();
                    return Page();
                }
                if (string.IsNullOrWhiteSpace(title))
                {
                    TempData["ErrorMessage"] = "Title is required.";
                    await LoadDataAsync();
                    return Page();
                }
                if (string.IsNullOrWhiteSpace(lessonType))
                {
                    TempData["ErrorMessage"] = "Lesson type is required.";
                    await LoadDataAsync();
                    return Page();
                }
                if (string.IsNullOrWhiteSpace(sortOrderStr) || !int.TryParse(sortOrderStr, out int sortOrder))
                {
                    TempData["ErrorMessage"] = "Invalid sort order.";
                    await LoadDataAsync();
                    return Page();
                }

                var lesson = new Lesson
                {
                    CourseId = courseId,
                    Title = title,
                    Description = description,
                    LessonType = lessonType,
                    Duration = !string.IsNullOrEmpty(durationStr) && int.TryParse(durationStr, out int duration) ? duration : null,
                    SortOrder = sortOrder,
                    ContentUrl = contentUrl,
                    IsPreview = isPreview,
                    CreatedAt = DateTime.Now
                };

                await _lessonService.CreateAsync(lesson);
                TempData["SuccessMessage"] = "Lesson created successfully!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to create lesson: {ex.Message}";
                await LoadDataAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostUpdateLessonAsync()
        {
            try
            {
                var lessonIdStr = Request.Form["LessonId"];
                var courseIdStr = Request.Form["CourseId"];
                var title = Request.Form["Title"];
                var description = Request.Form["Description"];
                var lessonType = Request.Form["LessonType"];
                var durationStr = Request.Form["Duration"];
                var sortOrderStr = Request.Form["SortOrder"];
                var contentUrl = Request.Form["ContentUrl"];
                var isPreview = Request.Form.ContainsKey("IsPreview");

                // Validate inputs
                if (!int.TryParse(lessonIdStr, out int lessonId))
                {
                    TempData["ErrorMessage"] = "Invalid lesson ID.";
                    await LoadDataAsync();
                    return Page();
                }
                if (string.IsNullOrWhiteSpace(courseIdStr) || !int.TryParse(courseIdStr, out int courseId))
                {
                    TempData["ErrorMessage"] = "Invalid course selection.";
                    await LoadDataAsync();
                    return Page();
                }
                if (string.IsNullOrWhiteSpace(title))
                {
                    TempData["ErrorMessage"] = "Title is required.";
                    await LoadDataAsync();
                    return Page();
                }
                if (string.IsNullOrWhiteSpace(lessonType))
                {
                    TempData["ErrorMessage"] = "Lesson type is required.";
                    await LoadDataAsync();
                    return Page();
                }
                if (string.IsNullOrWhiteSpace(sortOrderStr) || !int.TryParse(sortOrderStr, out int sortOrder))
                {
                    TempData["ErrorMessage"] = "Invalid sort order.";
                    await LoadDataAsync();
                    return Page();
                }

                var lesson = await _lessonService.GetByIdAsync(lessonId);
                if (lesson == null)
                {
                    TempData["ErrorMessage"] = "Lesson not found.";
                    return RedirectToPage();
                }

                lesson.CourseId = courseId;
                lesson.Title = title;
                lesson.Description = description;
                lesson.LessonType = lessonType;
                lesson.Duration = !string.IsNullOrEmpty(durationStr) && int.TryParse(durationStr, out int duration) ? duration : null;
                lesson.SortOrder = sortOrder;
                lesson.ContentUrl = contentUrl;
                lesson.IsPreview = isPreview;

                await _lessonService.UpdateAsync(lesson);
                TempData["SuccessMessage"] = "Lesson updated successfully!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to update lesson: {ex.Message}";
                await LoadDataAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteLessonAsync(int lessonId)
        {
            try
            {
                var lesson = await _lessonService.GetByIdAsync(lessonId);
                if (lesson == null)
                {
                    TempData["ErrorMessage"] = "Lesson not found.";
                    return RedirectToPage();
                }

                await _lessonService.DeleteAsync(lessonId);
                TempData["SuccessMessage"] = "Lesson deleted successfully!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to delete lesson: {ex.Message}";
                return RedirectToPage();
            }
        }

        private async Task LoadDataAsync()
        {
            Lessons = await _lessonService.GetLessonsForAdminAsync();
            Courses = await _courseService.GetCoursesForAdminAsync();
        }
    }
}