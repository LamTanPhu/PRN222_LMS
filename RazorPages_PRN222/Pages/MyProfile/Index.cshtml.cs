using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interface;

namespace RazorPages_PRN222.Pages.Profile
{
    public class MyProfileModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _env;

        public MyProfileModel(IUserService userService, IWebHostEnvironment env)
        {
            _userService = userService;
            _env = env;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [BindProperty]
        public IFormFile? ProfileImageFile { get; set; }

        public string? StatusMessage { get; set; }

        public class InputModel
        {
            public string Username { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? Bio { get; set; } = string.Empty;
            public string? ProfileImage { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            var user = await _userService.GetByIdAsync(userId);
            if (user == null) return NotFound();

            Input = new InputModel
            {
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Bio = user.Bio,
                ProfileImage = user.ProfileImage
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            var user = await _userService.GetByIdAsync(userId);
            if (user == null) return NotFound();

            user.FullName = Input.FullName;
            user.Bio = Input.Bio;

            if (ProfileImageFile != null)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ProfileImageFile.FileName)}";
                var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfileImageFile.CopyToAsync(stream);
                }

                user.ProfileImage = "/uploads/" + fileName;
            }

            await _userService.UpdateAsync(user);

            StatusMessage = "Profile updated successfully!";
            return RedirectToPage();
        }
    }
}
