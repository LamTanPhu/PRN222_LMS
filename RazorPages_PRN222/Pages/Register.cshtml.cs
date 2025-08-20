using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interface;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace RazorPages_PRN222.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IUserService _userService;

        public RegisterModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required]
            public string FullName { get; set; } = string.Empty;
            [Required]
            public string UserName { get; set; } = string.Empty;

            [Required, EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required, DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Required, DataType(DataType.Password), Compare("Password")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userService.RegisterAsync(Input.FullName, Input.Email, Input.Password, Input.UserName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email already exists.");
                return Page();
            }

            // sign user in
            var claims = new List<Claim>
        {
            new Claim("UserId", user.UserId.ToString()),
            new Claim("Email", user.Email),
            new Claim("FullName", user.FullName),
            new Claim("Role", user.Role?.RoleName ?? "Student")
        };

            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("Cookies", principal);
            User.AddIdentity(identity);

            return RedirectToPage("/Index");
        }
    }

}
