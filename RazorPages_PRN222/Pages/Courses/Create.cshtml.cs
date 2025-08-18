using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Models;
using Service.Interface;
using Service.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RazorPages_PRN222.Pages.Courses
{
    public class CreateModel : PageModel
    {
        private readonly ICourseService courseService;
        private readonly ICategoryService categoryService;
        private readonly IInstructorProfileService instructorProfileService;

        [BindProperty]
        public Course Course { get; set; }

        public List<Category> Categories { get; set; }
        public List<InstructorProfile> Instructors { get; set; }

        public CreateModel(ICourseService courseService, ICategoryService categoryService, IInstructorProfileService instructorProfileService)
        {
            this.courseService = courseService;
            this.categoryService = categoryService;
            this.instructorProfileService = instructorProfileService;
        }

        public async Task OnGetAsync()
        {
            Categories = await categoryService.GetAllAsync();
            Instructors = await instructorProfileService.GetAllAsync();
            Course = new Course();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await courseService.CreateAsync(Course);
            return RedirectToPage("/Admin/Courses");
        }
    }
}