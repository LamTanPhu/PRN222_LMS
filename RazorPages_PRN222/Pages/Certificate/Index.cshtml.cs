using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interface;
using Repository.Models;

namespace RazorPages_PRN222.Pages.Certificate
{
    public class IndexModel : PageModel
    {
        private readonly ICertificateService _certificateService;
        private readonly ICourseService _courseService;
        private readonly IUserService _userService;
        private readonly IStudentProgressService _progressService;

        public IndexModel(ICertificateService certificateService, ICourseService courseService, 
                         IUserService userService, IStudentProgressService progressService)
        {
            _certificateService = certificateService;
            _courseService = courseService;
            _userService = userService;
            _progressService = progressService;
        }

        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string UserName { get; set; }
        public Repository.Models.Certificate Certificate { get; set; }

        public async Task<IActionResult> OnGetAsync(int courseId)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Login");
            }

            CourseId = courseId;
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            // Get course information
            var course = await _courseService.GetByIdAsync(courseId);
            if (course == null)
            {
                return NotFound();
            }
            CourseTitle = course.Title;

            // Get user information
            var user = await _userService.GetByIdAsync(userId);
            if (user != null)
            {
                UserName = user.FullName;
            }

            // Check if certificate already exists
            var allCertificates = await _certificateService.GetAllAsync();
            Certificate = allCertificates.FirstOrDefault(c => c.UserId == userId && c.CourseId == courseId);

            // If no certificate exists, check if user is eligible for one
            if (Certificate == null)
            {
                var allProgress = await _progressService.GetAllAsync();
                var progress = allProgress.FirstOrDefault(p => p.UserId == userId && p.CourseId == courseId);
                if (progress != null && progress.IsCompleted == true)
                {
                    // Create certificate
                    Certificate = new Repository.Models.Certificate
                    {
                        UserId = userId,
                        CourseId = courseId,
                        CertificateCode = GenerateCertificateCode(),
                        IssuedDate = DateTime.Now,
                        IsVerified = true
                    };

                    await _certificateService.CreateAsync(Certificate);
                }
            }

            return Page();
        }

        private string GenerateCertificateCode()
        {
            // Generate a unique certificate code
            return $"CERT-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
    }
}

