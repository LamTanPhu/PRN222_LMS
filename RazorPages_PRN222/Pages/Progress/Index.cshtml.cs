using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interface;
using Repository.Models;

namespace RazorPages_PRN222.Pages.Progress
{
    public class IndexModel : PageModel
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IStudentProgressService _progressService;

        public IndexModel(IEnrollmentService enrollmentService, IStudentProgressService progressService)
        {
            _enrollmentService = enrollmentService;
            _progressService = progressService;
        }

        public List<Repository.Models.Enrollment> Enrollments { get; set; } = new List<Repository.Models.Enrollment>();
        public List<Repository.Models.StudentProgress> ProgressList { get; set; } = new List<Repository.Models.StudentProgress>();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Login");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            // Get user enrollments with course details
            var allEnrollments = await _enrollmentService.GetAllAsync();
            Enrollments = allEnrollments.Where(e => e.UserId == userId).ToList();
            
            // Get progress for each enrollment
            var allProgress = await _progressService.GetAllAsync();
            ProgressList = allProgress.Where(p => p.UserId == userId).ToList();

            return Page();
        }
    }
}

