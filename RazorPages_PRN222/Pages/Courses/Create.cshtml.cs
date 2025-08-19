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

        [BindProperty]
        public Course Course { get; set; }
        public List<Category> Categories { get; set; }
        public List<InstructorProfile> Instructors { get; set; }
        public CourseCreateViewModel CourseVM { get; set; }

        public CreateModel(
            ICourseService courseService,
            ICategoryService categoryService,
            IInstructorProfileService instructorProfileService,
            ILessonService lessonService,
            IQuizService quizService)
        {
            _courseService = courseService;
            _categoryService = categoryService;
            _instructorProfileService = instructorProfileService;
            _lessonService = lessonService;
            _quizService = quizService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var instructorProfile = await _instructorProfileService.GetByIdAsync(userId);
            if (instructorProfile == null)
            {
                return Forbid();
            }

            Categories = await _categoryService.GetAllAsync();
            if (Categories == null || Categories.Count == 0)
            {
                ModelState.AddModelError("", "No categories available. Please contact an administrator.");
                return Page();
            }

            CourseVM = new CourseCreateViewModel();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Categories = await _categoryService.GetAllAsync();
            if (Categories == null || Categories.Count == 0)
            {
                ModelState.AddModelError("", "No categories available. Please contact an administrator.");
                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var instructorProfile = await _instructorProfileService.GetByIdAsync(userId);
            if (instructorProfile == null)
            {
                return Forbid();
            }

            // Create the course
            var course = new Course
            {
                Title = CourseVM.Title,
                Description = CourseVM.Description,
                CategoryId = CourseVM.CategoryId
            };
            var createdCourse = await _courseService.CreateAsync(course, instructorProfile.InstructorId);

            // Create lessons
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