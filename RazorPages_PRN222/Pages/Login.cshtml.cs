using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interface;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace RazorPages_PRN222.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IUserService _userService;

        public LoginModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password is required.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userService.LoginAsync(Input.Email, Input.Password);
            if (user != null)
            {
                // Create claims for the user
                var claims = new List<Claim>
                {
                    new Claim("UserId", user.UserId.ToString()),
                    new Claim("Email", user.Email),
                    new Claim("FullName", user.FullName ?? ""),
                    new Claim("Role", user.Role?.RoleName ?? "User")
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync("Cookies", claimsPrincipal);
                // After successful SignInAsync
                User.AddIdentity(claimsIdentity);
                switch (User.Claims.First(c => c.Type == "Role").Value)
                {
                    case "Admin":
                        return RedirectToPage("/Admin/Index");
                    case "Instructor":
                        return RedirectToPage("/Instructor/Index");
                    case "Student":
                        return RedirectToPage("/Index");
                    default:
                        return RedirectToPage("/Privacy");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
}
