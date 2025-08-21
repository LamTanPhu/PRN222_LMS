using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Models;
using Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPages_PRN222.Pages.Courses
{
    public class ForumsModel : PageModel
    {
        private readonly IForumService _forumService;
        private readonly ICourseService _courseService;
        private readonly IUserService _userService;

        public ForumsModel(IForumService forumService, ICourseService courseService, IUserService userService)
        {
            _forumService = forumService;
            _courseService = courseService;
            _userService = userService;
        }

        public List<Repository.Models.Forum> Forums { get; set; } = new List<Repository.Models.Forum>();
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }

        [BindProperty]
        public Repository.Models.Forum NewPost { get; set; }

        public async Task OnGetAsync(int id)
        {
            CourseId = id;
            
            // Get course title
            var course = await _courseService.GetByIdAsync(id);
            CourseTitle = course?.Title ?? "Unknown Course";
            
            // Get all forum posts for this course
            var allForums = await _forumService.GetAllAsync();
            Forums = allForums.Where(f => f.CourseId == id).ToList();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            CourseId = id;
            
            // Get course title
            var course = await _courseService.GetByIdAsync(id);
            CourseTitle = course?.Title ?? "Unknown Course";

            // Create new forum post
            var newForum = new Repository.Models.Forum
            {
                CourseId = id,
                UserId = 1, // TODO: Get current user ID from session/authentication
                Title = NewPost.Title,
                Content = NewPost.Content,
                PostDate = System.DateTime.Now,
                IsSticky = false,
                IsClosed = false,
                ReplyCount = 0
            };

            await _forumService.CreateAsync(newForum);

            // Redirect to refresh the page
            return RedirectToPage(new { id = id });
        }
    }
}
