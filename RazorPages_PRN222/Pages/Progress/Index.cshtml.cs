using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interface;
using Repository.Models;

namespace RazorPages_PRN222.Pages.Progress
{
    public class CourseProgressViewModel
    {
        public Enrollment Enrollment { get; set; }
        public StudentProgress Progress { get; set; }
    }

    public class IndexModel : PageModel
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IStudentProgressService _progressService;

        public IndexModel(IEnrollmentService enrollmentService, IStudentProgressService progressService)
        {
            _enrollmentService = enrollmentService;
            _progressService = progressService;
        }

        public List<CourseProgressViewModel> CourseProgressList { get; set; } = new List<CourseProgressViewModel>();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!(User.Identity?.IsAuthenticated ?? false))
            {
                return RedirectToPage("/Login");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            // Get user enrollments with course details
            var allEnrollments = await _enrollmentService.GetAllAsync();
            var enrollments = allEnrollments.Where(e => e.UserId == userId).ToList();
            
            // Get progress for each enrollment
            var allProgress = await _progressService.GetAllAsync();
            var progressList = allProgress.Where(p => p.UserId == userId).ToList();

            foreach (var enrollment in enrollments)
            {
                var progress = progressList.FirstOrDefault(p => p.UserId == enrollment.UserId);
                CourseProgressList.Add(new CourseProgressViewModel
                {
                    Enrollment = enrollment,
                    Progress = progress
                });
            }

            return Page();
        }
    }
}