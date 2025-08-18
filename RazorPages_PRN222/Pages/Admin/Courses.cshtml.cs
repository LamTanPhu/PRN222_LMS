using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Models;
using Service.Interface;
using Service.Service;
using System.Threading.Tasks;

namespace RazorPages_PRN222.Pages.Admin
{
    public class CoursesModel : PageModel
    {
        private readonly ICourseService courseService;

        public List<Course> Courses { get; set; }

        public CoursesModel(ICourseService courseService)
        {
            this.courseService = courseService;
        }

        public async Task OnGetAsync()
        {
            Courses = await courseService.GetAllAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await courseService.DeleteAsync(id);
            return RedirectToPage();
        }
    }
}