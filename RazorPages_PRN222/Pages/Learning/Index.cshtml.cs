using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Models;
using Service.Interface;
using System.Security.Claims;

namespace RazorPages_PRN222.Pages.Learning
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IStudentProgressService _studentProgressService;

        public IndexModel(
            IEnrollmentService enrollmentService,
            IStudentProgressService studentProgressService)
        {
            _enrollmentService = enrollmentService;
            _studentProgressService = studentProgressService;
        }

        public List<Enrollment> EnrolledCourses { get; set; } = new List<Enrollment>();
        public List<StudentProgress> RecentProgress { get; set; } = new List<StudentProgress>();

        public async Task OnGetAsync()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return;
            }
            
            // Get enrolled courses
            var allEnrollments = await _enrollmentService.GetAllAsync();
            EnrolledCourses = allEnrollments
                .Where(e => e.UserId == userId && e.Status == "Active")
                .OrderByDescending(e => e.LastAccessedAt)
                .ToList();

            // Get recent progress
            var allProgress = await _studentProgressService.GetAllAsync();
            RecentProgress = allProgress
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.LastAccessedAt)
                .Take(10)
                .ToList();
        }
    }
}
