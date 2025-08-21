using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Models;
using Repository.Models.ViewModel;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RazorPages_PRN222.Pages.Courses
{
    public class CreateModel : PageModel
    {
        private readonly ICourseService _courseService;
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;
        private readonly ILogger<CreateModel> _logger;

        [BindProperty]
        public CourseCreateViewModel CourseVM { get; set; }

        public List<Category> Categories { get; set; }
        public List<User> Instructors { get; set; }

        public CreateModel(
            ICourseService courseService,
            ICategoryService categoryService,
            IUserService userService,
            ILogger<CreateModel> logger)
        {
            _courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadData();
            CourseVM = new CourseCreateViewModel();
            return Page();
        }

        private async Task LoadData()
        {
            Categories = await _categoryService.GetAllAsync() ?? new List<Category>();
            Instructors = await _userService.GetAllAsync() ?? new List<User>();
            
            if (Categories.Count == 0)
            {
                _logger.LogWarning("No categories available in the database.");
                ModelState.AddModelError(string.Empty, "No categories available. Please contact an administrator to add categories.");
            }
            
            if (Instructors.Count == 0)
            {
                _logger.LogWarning("No instructors available in the database.");
                ModelState.AddModelError(string.Empty, "No instructors available. Please contact an administrator to add instructors.");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadData();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Create the course with selected instructor
            var course = new Course
            {
                Title = CourseVM.Title,
                Subtitle = CourseVM.Subtitle,
                Description = CourseVM.Description,
                CategoryId = CourseVM.CategoryId,
                DifficultyLevel = CourseVM.DifficultyLevel,
                Language = CourseVM.Language,
                Price = CourseVM.Price,
                CreatedAt = DateTime.UtcNow
            };
            
            var createdCourse = await _courseService.CreateAsync(course, CourseVM.InstructorId);
            
            if (createdCourse != null)
            {
                _logger.LogInformation("Course created successfully: {CourseId} - {Title}", createdCourse.CourseId, createdCourse.Title);
                TempData["SuccessMessage"] = $"Course '{createdCourse.Title}' created successfully! You can now add lessons and quizzes through the admin pages.";
                return RedirectToPage("/Admin/Courses");
            }
            else
            {
                _logger.LogError("Failed to create course: {Title}", CourseVM.Title);
                ModelState.AddModelError(string.Empty, "Failed to create course. Please try again.");
                return Page();
            }
        }
    }
}