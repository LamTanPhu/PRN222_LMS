using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interface;
using Repository.Models;

namespace RazorPages_PRN222.Pages.Wishlist
{
    public class IndexModel : PageModel
    {
        private readonly IWishlistService _wishlistService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IUserService _userService;

        public IndexModel(IWishlistService wishlistService, IEnrollmentService enrollmentService, IUserService userService)
        {
            _wishlistService = wishlistService;
            _enrollmentService = enrollmentService;
            _userService = userService;
        }

        public List<Repository.Models.Wishlist> WishlistItems { get; set; } = new List<Repository.Models.Wishlist>();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Login");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            // Get all wishlist items and filter by user
            var allWishlists = await _wishlistService.GetAllAsync();
            WishlistItems = allWishlists.Where(w => w.UserId == userId).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveFromWishlistAsync(int courseId)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Login");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            // Find and delete the wishlist item
            var allWishlists = await _wishlistService.GetAllAsync();
            var wishlistItem = allWishlists.FirstOrDefault(w => w.UserId == userId && w.CourseId == courseId);
            if (wishlistItem != null)
            {
                await _wishlistService.DeleteAsync(wishlistItem.WishlistId);
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEnrollCourseAsync(int courseId)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Login");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            // Create enrollment
            var enrollment = new Repository.Models.Enrollment
            {
                UserId = userId,
                CourseId = courseId,
                EnrollmentDate = DateTime.Now,
                Status = "Active",
                PaymentStatus = "Paid"
            };

            await _enrollmentService.CreateAsync(enrollment);
            
            // Remove from wishlist after enrollment
            var allWishlists = await _wishlistService.GetAllAsync();
            var wishlistItem = allWishlists.FirstOrDefault(w => w.UserId == userId && w.CourseId == courseId);
            if (wishlistItem != null)
            {
                await _wishlistService.DeleteAsync(wishlistItem.WishlistId);
            }

            return RedirectToPage("/MyProfile/Index");
        }
    }
}

