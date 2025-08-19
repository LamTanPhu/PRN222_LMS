using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interface;
using Repository.Models;

namespace RazorPages_PRN222.Pages.Forum
{
    public class IndexModel : PageModel
    {
        private readonly IForumService _forumService;
        private readonly IForumReplyService _replyService;
        private readonly ICourseService _courseService;

        public IndexModel(IForumService forumService, IForumReplyService replyService, ICourseService courseService)
        {
            _forumService = forumService;
            _replyService = replyService;
            _courseService = courseService;
        }

        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string CourseDescription { get; set; }
        public List<Repository.Models.Forum> ForumPosts { get; set; } = new List<Repository.Models.Forum>();
        [BindProperty]
        public Repository.Models.Forum EditingPost { get; set; }
        [BindProperty]
        public Repository.Models.ForumReply EditingReply { get; set; }

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
            CourseDescription = course.Description;

            // Get forum posts for this course
            var allForums = await _forumService.GetAllAsync();
            ForumPosts = allForums.Where(f => f.CourseId == courseId).ToList();

            return Page();
        }

        public async Task<IActionResult> OnGetEditPostAsync(int id, int courseId)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");
            CourseId = courseId;
            var post = await _forumService.GetByIdAsync(id);
            if (post == null) return NotFound();
            if (post.UserId != userId && !User.IsInRole("Admin")) return Forbid();
            EditingPost = post;
            await OnGetAsync(courseId);
            return Page();
        }

        public async Task<IActionResult> OnPostDeletePostAsync(int id, int courseId)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");
            var post = await _forumService.GetByIdAsync(id);
            if (post == null) return RedirectToPage(new { courseId });
            if (post.UserId != userId && !User.IsInRole("Admin")) return Forbid();
            await _forumService.DeleteAsync(id);
            return RedirectToPage(new { courseId });
        }

        public async Task<IActionResult> OnPostUpdatePostAsync(int id, int courseId, string title, string content)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");
            var post = await _forumService.GetByIdAsync(id);
            if (post == null) return RedirectToPage(new { courseId });
            if (post.UserId != userId && !User.IsInRole("Admin")) return Forbid();
            post.Title = title;
            post.Content = content;
            await _forumService.UpdateAsync(post);
            return RedirectToPage(new { courseId });
        }

        public async Task<IActionResult> OnGetEditReplyAsync(int id, int courseId)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");
            CourseId = courseId;
            var reply = await _replyService.GetByIdAsync(id);
            if (reply == null) return NotFound();
            if (reply.UserId != userId && !User.IsInRole("Admin")) return Forbid();
            EditingReply = reply;
            await OnGetAsync(courseId);
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteReplyAsync(int id, int courseId)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");
            var reply = await _replyService.GetByIdAsync(id);
            if (reply == null) return RedirectToPage(new { courseId });
            if (reply.UserId != userId && !User.IsInRole("Admin")) return Forbid();
            await _replyService.DeleteAsync(id);
            return RedirectToPage(new { courseId });
        }

        public async Task<IActionResult> OnPostUpdateReplyAsync(int id, int courseId, string replyContent)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");
            var reply = await _replyService.GetByIdAsync(id);
            if (reply == null) return RedirectToPage(new { courseId });
            if (reply.UserId != userId && !User.IsInRole("Admin")) return Forbid();
            reply.Content = replyContent;
            await _replyService.UpdateAsync(reply);
            return RedirectToPage(new { courseId });
        }

        public async Task<IActionResult> OnPostCreatePostAsync(int courseId, string title, string content)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Login");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            var forumPost = new Repository.Models.Forum
            {
                CourseId = courseId > 0 ? courseId : CourseId,
                UserId = userId,
                Title = title,
                Content = content,
                PostDate = DateTime.Now
            };

            await _forumService.CreateAsync(forumPost);

            return RedirectToPage(new { courseId = forumPost.CourseId });
        }

        public async Task<IActionResult> OnPostCreateReplyAsync(int courseId, int forumId, string replyContent)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Login");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            var reply = new Repository.Models.ForumReply
            {
                PostId = forumId,
                UserId = userId,
                Content = replyContent,
                ReplyDate = DateTime.Now
            };

            await _replyService.CreateAsync(reply);

            return RedirectToPage(new { courseId = courseId > 0 ? courseId : CourseId });
        }
    }
}

