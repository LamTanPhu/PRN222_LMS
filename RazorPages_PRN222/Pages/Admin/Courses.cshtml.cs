using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repository.Models;
using Service.Interface;
using Service.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RazorPages_PRN222.Pages.Admin
{
    public class CoursesModel : PageModel
    {
        private readonly ICourseService courseService;
        private readonly IUserService userService; // For instructors
        private readonly ICategoryService categoryService;

        public List<Course> Courses { get; set; } = new List<Course>();
        public SelectList InstructorOptions { get; set; }
        public SelectList CategoryOptions { get; set; }
        [BindProperty]
        public Course NewCourse { get; set; }
        [BindProperty]
        public Course EditCourse { get; set; }

        public CoursesModel(ICourseService courseService, IUserService userService, ICategoryService categoryService)
        {
            this.courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }

        public async Task OnGetAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            Courses = await courseService.GetAllAsync() ?? new List<Course>();
            var instructors = await userService.GetAllAsync() ?? new List<User>();
            var categories = await categoryService.GetAllAsync() ?? new List<Category>();
            InstructorOptions = new SelectList(instructors, "UserId", "UserId"); // Adjust to display name if available
            CategoryOptions = new SelectList(categories, "CategoryId", "Name");
            NewCourse = new Course();
            EditCourse = new Course();
        }

        public async Task<IActionResult> OnPostCreateAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                await LoadData();
                return Page();
            }

            try
            {
                var course = await courseService.GetByIdAsync(id);
                if (course == null)
                {
                    ModelState.AddModelError(string.Empty, "Course not found.");
                    await LoadData();
                    return Page();
                }

                course.Title = EditCourse.Title;
                course.Description = EditCourse.Description;
                course.InstructorId = EditCourse.InstructorId;
                course.CategoryId = EditCourse.CategoryId;
                await courseService.UpdateAsync(course);
                await LoadData();
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error updating course: {ex.Message}");
                await LoadData();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            if (!ModelState.IsValid) return Page();
            try
            {
                var course = await courseService.GetByIdAsync(id);
                if (course != null)
                {
                    course.Title = EditCourse.Title ?? course.Title;
                    course.Description = EditCourse.Description ?? course.Description;
                    course.Price = EditCourse.Price != 0m ? EditCourse.Price : course.Price;
                    course.InstructorId = EditCourse.InstructorId != 0 ? EditCourse.InstructorId : course.InstructorId;
                    course.CategoryId = EditCourse.CategoryId != 0 ? EditCourse.CategoryId : course.CategoryId;
                    await courseService.UpdateAsync(course);
                }
                await LoadData();
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error updating course: " + ex.Message);
                await LoadData();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                await courseService.DeleteAsync(id);
                await LoadData();
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error deleting course: " + ex.Message);
                await LoadData();
                return Page();
            }
        }
    }
}