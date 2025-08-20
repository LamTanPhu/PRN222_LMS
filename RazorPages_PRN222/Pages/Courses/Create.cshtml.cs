using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Models;
using Repository.Models.ViewModel;
using Service.Interface;
using Service.Service;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RazorPages_PRN222.Pages.Courses
{
    public class CreateModel : PageModel
    {
        private readonly ICourseService _courseService;
        private readonly ICategoryService _categoryService;
        private readonly IInstructorProfileService _instructorProfileService;
        private readonly ILessonService _lessonService;
        private readonly IQuizService _quizService;
        private readonly ILogger<CreateModel> _logger;

        [BindProperty]
        public CourseCreateViewModel CourseVM { get; set; }

        public List<Category> Categories { get; set; }

        public CreateModel(
            ICourseService courseService,
            ICategoryService categoryService,
            IInstructorProfileService instructorProfileService,
            ILessonService lessonService,
            IQuizService quizService,
            ILogger<CreateModel> logger)
        {
            _courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _instructorProfileService = instructorProfileService ?? throw new ArgumentNullException(nameof(instructorProfileService));
            _lessonService = lessonService ?? throw new ArgumentNullException(nameof(lessonService));
            _quizService = quizService ?? throw new ArgumentNullException(nameof(quizService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                _logger.LogWarning("Unauthenticated user attempted to access /Courses/Create.");
                return RedirectToPage("/Account/Login");
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogError("Invalid user ID claim: {UserIdClaim}", userIdClaim);
                ModelState.AddModelError(string.Empty, "Invalid user ID. Please ensure you are logged in correctly.");
                return Page();
            }

            var instructorProfile = await _instructorProfileService.GetByIdAsync(userId);
            if (instructorProfile == null)
            {
                _logger.LogWarning("No instructor profile found for user ID: {UserId}", userId);
                ModelState.AddModelError(string.Empty, "You must have an instructor profile to create a course.");
                return Page();
            }

            Categories = await _categoryService.GetAllAsync();
            if (Categories == null)
            {
                _logger.LogError("Categories list is null from CategoryService.GetAllAsync.");
                Categories = new List<Category>();
            }
            if (Categories.Count == 0)
            {
                _logger.LogWarning("No categories available in the database.");
                ModelState.AddModelError(string.Empty, "No categories available. Please contact an administrator to add categories.");
                return Page();
            }

            CourseVM = new CourseCreateViewModel();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Categories = await _categoryService.GetAllAsync();
            if (Categories == null)
            {
                _logger.LogError("Categories list is null from CategoryService.GetAllAsync.");
                Categories = new List<Category>();
            }
            if (Categories.Count == 0)
            {
                _logger.LogWarning("No categories available in the database.");
                ModelState.AddModelError(string.Empty, "No categories available. Please contact an administrator to add categories.");
                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!User.Identity.IsAuthenticated)
            {
                _logger.LogWarning("Unauthenticated user attempted to post to /Courses/Create.");
                return RedirectToPage("/Account/Login");
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogError("Invalid user ID claim: {UserIdClaim}", userIdClaim);
                ModelState.AddModelError(string.Empty, "Invalid user ID. Please ensure you are logged in correctly.");
                return Page();
            }

            var instructorProfile = await _instructorProfileService.GetByIdAsync(userId);
            if (instructorProfile == null)
            {
                _logger.LogWarning("No instructor profile found for user ID: {UserId}", userId);
                ModelState.AddModelError(string.Empty, "You must have an instructor profile to create a course.");
                return Page();
            }

            // Create the course
            var course = new Course
            {
                Title = CourseVM.Title,
                Description = CourseVM.Description,
                CategoryId = CourseVM.CategoryId
            };
            var createdCourse = await _courseService.CreateAsync(course, instructorProfile.InstructorId);

            // Create lessons and their quizzes
            foreach (var lessonVM in CourseVM.Lessons)
            {
                if (!string.IsNullOrEmpty(lessonVM.Title))
                {
                    var lesson = new Lesson
                    {
                        Title = lessonVM.Title,
                        Description = lessonVM.Content
                    };
                    var createdLesson = await _lessonService.CreateAsync(lesson, createdCourse.CourseId);

                    // Create quizzes for this lesson
                    foreach (var quizVM in lessonVM.Quizzes)
                    {
                        if (!string.IsNullOrEmpty(quizVM.Title))
                        {
                            var quiz = new Repository.Models.Quiz
                            {
                                Title = quizVM.Title,
                                Description = quizVM.Description
                            };
                            await _quizService.CreateAsync(quiz, createdLesson.LessonId);
                        }
                    }
                }
            }

            return RedirectToPage("/Admin/Courses");
        }
    }
}