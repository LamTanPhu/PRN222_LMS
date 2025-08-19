using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Models;
using Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPages_PRN222.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICourseService _courseService;
        private readonly IAnnouncementService _announcementService;
        private readonly ICategoryService _categoryService;

        public IndexModel(ICourseService courseService, IAnnouncementService announcementService, ICategoryService categoryService)
        {
            _courseService = courseService;
            _announcementService = announcementService;
            _categoryService = categoryService;
        }

        public IList<Course> FeaturedCourses { get; set; }
        public IList<Announcement> Announcements { get; set; }
        public IList<Category> Categories { get; set; }

        public async Task OnGetAsync()
        {
            if (User.Claims.FirstOrDefault(c => c.Type == "Role") is null)
                Response.Redirect("/Login");
            else
            switch (User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value)
            {
                case "Admin":
                    Response.Redirect("/Admin/Index");
                    break;
                case "Instructor":
                    Response.Redirect("/Instructor/Index");
                    break;
            }
            FeaturedCourses = (await _courseService.GetAllAsync()).Take(6).ToList();
            Announcements = (await _announcementService.GetAllAsync())
                .Where(a => a.IsActive ?? false)
                .Take(5)
                .ToList();
            Categories = (await _categoryService.GetAllAsync()).Take(8).ToList();
        }
    }
}