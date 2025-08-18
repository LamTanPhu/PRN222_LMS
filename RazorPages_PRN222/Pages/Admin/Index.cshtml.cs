using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interface;
using Service.Service;

namespace RazorPages_PRN222.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly IUserService userService;
        private readonly ICourseService courseService;
        private readonly IAnnouncementService announcementService;

        public int TotalUsers { get; set; }
        public int TotalCourses { get; set; }
        public int TotalAnnouncements { get; set; }

        public IndexModel(IUserService userService, ICourseService courseService, IAnnouncementService announcementService)
        {
            this.userService = userService;
            this.courseService = courseService;
            this.announcementService = announcementService;
        }

        public async Task OnGetAsync()
        {
            TotalUsers = (await userService.GetAllAsync()).Count;
            TotalCourses = (await courseService.GetAllAsync()).Count;
            TotalAnnouncements = (await announcementService.GetAllAsync()).Count;
        }
    }
}