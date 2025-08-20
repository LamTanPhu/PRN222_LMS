using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
namespace RazorPages_PRN222.Pages.Certificate
{
    public class IndexModel : PageModel
    {
        private readonly ICertificateService _certificateService;
        private readonly ICourseService _courseService;
        private readonly IUserService _userService;

        public IndexModel(ICertificateService certificateService, ICourseService courseService, IUserService userService)
        {
            _certificateService = certificateService;
            _courseService = courseService;
            _userService = userService;
        }

        public string UserName { get; set; }
        public List<CertificateViewModel> Certificates { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Login");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            var user = await _userService.GetByIdAsync(userId);
            UserName = user?.FullName ?? "Unknown User";

            var allCertificates = await _certificateService.GetAllAsync();
            var userCertificates = allCertificates.Where(c => c.UserId == userId).ToList();

            foreach (var cert in userCertificates)
            {
                var course = await _courseService.GetByIdAsync(cert.CourseId);
                Certificates.Add(new CertificateViewModel
                {
                    CertificateId = cert.CertificateId,
                    CertificateCode = cert.CertificateCode,
                    IssuedDate = cert.IssuedDate,
                    CourseTitle = course?.Title ?? "Unknown Course"
                });
            }

            return Page();
        }

        public class CertificateViewModel
        {
            public int CertificateId { get; set; }
            public string CertificateCode { get; set; }
            public DateTime? IssuedDate { get; set; }
            public string CourseTitle { get; set; }
        }
    }

}
