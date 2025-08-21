using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interface;
using Repository.Models;

namespace RazorPages_PRN222.Pages.Progress
{
    public class CourseProgressViewModel
    {
        public Enrollment Enrollment { get; set; }
        public int CompletedLessons { get; set; }
        public int TotalLessons { get; set; }
        public decimal ProgressPercentage { get; set; }
    }

    public class IndexModel : PageModel
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IStudentProgressService _progressService;
        private readonly ILessonService _lessonService; 

        public IndexModel(IEnrollmentService enrollmentService,
                          IStudentProgressService progressService,
                          ILessonService lessonService)
        {
            _enrollmentService = enrollmentService;
            _progressService = progressService;
            _lessonService = lessonService;
        }

        public List<CourseProgressViewModel> CourseProgressList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!(User.Identity?.IsAuthenticated ?? false))
            {
                return RedirectToPage("/Login");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            // Get user enrollments
            var allEnrollments = await _enrollmentService.GetAllAsync();
            var enrollments = allEnrollments.Where(e => e.UserId == userId).ToList();

            // Get progress records for this user
            var allProgress = await _progressService.GetAllAsync();
            var progressList = allProgress.Where(p => p.UserId == userId).ToList();

            // Get all lessons
            var allLessons = await _lessonService.GetAllAsync();

            foreach (var enrollment in enrollments)
            {
                // Lessons for this course
                var lessons = allLessons.Where(l => l.CourseId == enrollment.CourseId).ToList();
                int totalLessons = lessons.Count;

                // Completed lessons for this course
                var courseProgress = progressList.Where(p => p.CourseId == enrollment.CourseId && p.IsCompleted == true).ToList();
                int completedLessons = courseProgress.Count;

                decimal progressPercentage = totalLessons > 0
                    ? Math.Round((decimal)completedLessons / totalLessons * 100, 2)
                    : 0;

                CourseProgressList.Add(new CourseProgressViewModel
                {
                    Enrollment = enrollment,
                    CompletedLessons = completedLessons,
                    TotalLessons = totalLessons,
                    ProgressPercentage = progressPercentage
                });
            }

            return Page();
        }
    }
}
