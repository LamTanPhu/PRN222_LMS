using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interface;

namespace RazorPages_PRN222.Pages.Progress
{
    public class CourseProgressViewModel
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public decimal ProgressPercentage { get; set; }
        public int CompletedLessons { get; set; }
        public int TotalLessons { get; set; }
    }

    public class IndexModel : PageModel
    {
        private readonly IStudentProgressService _progressService;
        private readonly IEnrollmentService _enrollmentService;

        public IndexModel(IStudentProgressService progressService, IEnrollmentService enrollmentService)
        {
            _progressService = progressService;
            _enrollmentService = enrollmentService;
        }

        public List<CourseProgressViewModel> CourseProgressList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!(User.Identity?.IsAuthenticated ?? false))
                return RedirectToPage("/Login");

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            // Get enrollments
            var allEnrollments = await _enrollmentService.GetAllAsync();
            var enrollments = allEnrollments.Where(e => e.UserId == userId).ToList();

            foreach (var enrollment in enrollments)
            {
                var progressDto = await _progressService.GetCourseProgressAsync(userId, enrollment.CourseId);
                if (progressDto == null) continue;

                CourseProgressList.Add(new CourseProgressViewModel
                {
                    CourseId = enrollment.CourseId,
                    CourseTitle = enrollment.Course.Title,
                    Status = progressDto.Status,
                    PaymentStatus = enrollment.PaymentStatus,
                    EnrollmentDate = enrollment.EnrollmentDate,
                    ProgressPercentage = progressDto.Progress,
                    CompletedLessons = progressDto.CompletedLessons,
                    TotalLessons = progressDto.TotalLessons
                });
            }

            return Page();
        }
    }
}