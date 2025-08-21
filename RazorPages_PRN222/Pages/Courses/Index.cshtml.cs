using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Models;
using Repository.Repositories;
using Service.Interface;
using Service.Service;
using System;

namespace RazorPages_PRN222.Pages.Courses
{
    public class IndexModel : PageModel
    {
        private readonly ICourseService _courseService;
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;
        private readonly IWishlistService _wishlistService;

        public IndexModel(ICourseService courseService, ICategoryService categoryService, IUserService userService, IWishlistService wishlistService)
        {
            _courseService = courseService;
            _categoryService = categoryService;
            _userService = userService;
            _wishlistService = wishlistService;
        }

        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }

        public IList<Course> Courses { get; set; }
        public IList<Category> Categories { get; set; }
        public IList<int> WishlistCourseIds { get; set; } = new List<int>();

        public async Task<IActionResult> OnGetAsync()
        {
            Categories = (await _categoryService.GetAllAsync()).ToList();
            Courses = (await _courseService.GetAllAsync())
                .Where(c => !CategoryId.HasValue || c.CategoryId == CategoryId)
                .ToList();

            if (User.Identity?.IsAuthenticated ?? false)
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (int.TryParse(userIdClaim, out var userId))
                {
                    WishlistCourseIds = (await _wishlistService.GetWishlistAsync(userId))
                                        .Select(c => c.CourseId)
                                        .ToList();
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostEnrollAsync(int courseId)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Login");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            var user = await _userService.GetByIdAsync(userId);
            if (user == null) return Page();

            var course = await _courseService.GetByIdAsync(courseId);
            if (course == null) return Page();

            // Enrollment logic here: add to a simple session cart to simulate purchase
            var sessionKey = $"cart_{userId}";
            var json = HttpContext.Session.GetString(sessionKey);
            var courseIds = string.IsNullOrEmpty(json)
                ? new List<int>()
                : System.Text.Json.JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();

            if (!courseIds.Contains(courseId))
            {
                courseIds.Add(courseId);
                HttpContext.Session.SetString(sessionKey, System.Text.Json.JsonSerializer.Serialize(courseIds));
            }

            // Redirect to checkout for demo flow
            return RedirectToPage("/Checkout/Index");
        }

        public async Task<IActionResult> OnPostWishlistAsync(int courseId, string action)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Login");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            var user = await _userService.GetByIdAsync(userId);
            if (user == null) return Page();

            var course = await _courseService.GetByIdAsync(courseId);
            if (course == null) return Page();

            await _wishlistService.AddToWishlistAsync(userId, courseId);

            if (action == "add")
            {
                await _wishlistService.AddToWishlistAsync(userId, courseId);
                return RedirectToPage(); // stay on current page
            }

            if (action == "goto")
            {
                return RedirectToPage("/Wishlist/Index");
            }

            return RedirectToPage();
        }
    }
}
