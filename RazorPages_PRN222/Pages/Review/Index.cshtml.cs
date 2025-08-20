using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interface;
using Repository.Models;

namespace RazorPages_PRN222.Pages.Review
{
    public class IndexModel : PageModel
    {
        private readonly ICourseReviewService _reviewService;
        private readonly ICourseService _courseService;

        public IndexModel(ICourseReviewService reviewService, ICourseService courseService)
        {
            _reviewService = reviewService;
            _courseService = courseService;
        }

        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string CourseDescription { get; set; }
        public List<Repository.Models.CourseReview> CourseReviews { get; set; } = new List<Repository.Models.CourseReview>();
        public double AverageRating { get; set; }
        [BindProperty]
        public Repository.Models.CourseReview EditingReview { get; set; }

        public async Task<IActionResult> OnGetAsync(int? courseId)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Login");
            }

            if (courseId == null)
            {
                return RedirectToPage("/Courses/Index");
            }
            CourseId = courseId.Value;
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            // Get course information
            var course = await _courseService.GetByIdAsync(CourseId);
            if (course == null)
            {
                return NotFound();
            }
            CourseTitle = course.Title;
            CourseDescription = course.Description;

            // Get reviews for this course
            var allReviews = await _reviewService.GetAllAsync();
            CourseReviews = allReviews.Where(r => r.CourseId == CourseId).ToList();

            // Calculate average rating
            if (CourseReviews.Any())
            {
                AverageRating = CourseReviews.Average(r => r.Rating);
            }

            return Page();
        }

        public async Task<IActionResult> OnGetEditReviewAsync(int id, int courseId)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            CourseId = courseId;
            var review = await _reviewService.GetByIdAsync(id);
            if (review == null) return NotFound();

            // Authorization: owner or Admin
            var isOwner = review.UserId == userId;
            var isAdmin = User.IsInRole("Admin");
            if (!isOwner && !isAdmin) return Forbid();

            EditingReview = review;

            // Reload list to render page normally
            await OnGetAsync(courseId);
            return Page();
        }

        public async Task<IActionResult> OnPostCreateReviewAsync(int courseId, int rating, string reviewTitle, string reviewText)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Login");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            var targetCourseId = courseId > 0 ? courseId : CourseId;
            // Check if user already reviewed this course
            var allReviews = await _reviewService.GetAllAsync();
            var existingReview = allReviews.FirstOrDefault(r => r.UserId == userId && r.CourseId == targetCourseId);
            if (existingReview != null)
            {
                ModelState.AddModelError(string.Empty, "You have already reviewed this course.");
                return Page();
            }

            var review = new Repository.Models.CourseReview
            {
                UserId = userId,
                CourseId = targetCourseId,
                Rating = rating,
                ReviewTitle = reviewTitle,
                ReviewText = reviewText,
                ReviewDate = DateTime.Now,
                IsPublished = true
            };

            await _reviewService.CreateAsync(review);

            return RedirectToPage(new { courseId = targetCourseId });
        }

        public async Task<IActionResult> OnPostDeleteReviewAsync(int id, int courseId)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            var review = await _reviewService.GetByIdAsync(id);
            if (review == null) return RedirectToPage(new { courseId });

            // Authorization: owner or Admin
            var isOwner = review.UserId == userId;
            var isAdmin = User.IsInRole("Admin");
            if (!isOwner && !isAdmin) return Forbid();

            await _reviewService.DeleteAsync(id);
            return RedirectToPage(new { courseId });
        }

        public async Task<IActionResult> OnPostUpdateReviewAsync(int id, int courseId, int rating, string reviewTitle, string reviewText)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            var review = await _reviewService.GetByIdAsync(id);
            if (review == null) return RedirectToPage(new { courseId });

            // Authorization: owner or Admin
            var isOwner = review.UserId == userId;
            var isAdmin = User.IsInRole("Admin");
            if (!isOwner && !isAdmin) return Forbid();

            review.Rating = rating;
            review.ReviewTitle = reviewTitle;
            review.ReviewText = reviewText;
            await _reviewService.UpdateAsync(review);
            return RedirectToPage(new { courseId });
        }
    }
}

