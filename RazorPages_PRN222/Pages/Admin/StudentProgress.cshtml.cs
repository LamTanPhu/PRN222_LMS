using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Models;
using Repository.Repositories;
using Service.Interface;

namespace RazorPages_PRN222.Pages.Admin
{
    public class StudentProgressModel : PageModel
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IStudentProgressService _progressService;
        private readonly IStudentQuizAttemptService _quizAttemptService;
        private readonly IUserService _userService;
        private readonly ILogger<StudentProgressModel> _logger;

        public List<User> Users { get; set; }
        public List<Enrollment> Enrollments { get; set; }
        public List<StudentProgress> Progress { get; set; }
        public List<StudentQuizAttempt> QuizAttempts { get; set; }
        [BindProperty(SupportsGet = true)]
        public int SelectedUserId { get; set; }

        public StudentProgressModel(
            IEnrollmentService enrollmentService,
            IStudentProgressService progressService,
            IStudentQuizAttemptService quizAttemptService,
            IUserService userService,
            ILogger<StudentProgressModel> logger)
        {
            _enrollmentService = enrollmentService ?? throw new ArgumentNullException(nameof(enrollmentService));
            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));
            _quizAttemptService = quizAttemptService ?? throw new ArgumentNullException(nameof(quizAttemptService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Users = await _userService.GetUsersByRoleAsync("Student") ?? new List<User>();
            if (Users.Count == 0)
            {
                _logger.LogWarning("No users with 'Student' role found in the database.");
                ModelState.AddModelError(string.Empty, "No students available to monitor.");
            }

            if (SelectedUserId > 0)
            {
                Enrollments = await _enrollmentService.GetEnrollmentsByUserAsync(SelectedUserId);
                Progress = await _progressService.GetStudentProgressesByUserAsync(SelectedUserId);
                QuizAttempts = await _quizAttemptService.GetAttemptsByUserAsync(SelectedUserId);

                if (Enrollments.Count == 0 && Progress.Count == 0 && QuizAttempts.Count == 0)
                {
                    _logger.LogInformation("No enrollments, progress, or quiz attempts found for user ID: {UserId}", SelectedUserId);
                    ModelState.AddModelError(string.Empty, "No data found for the selected student.");
                }
            }
            else
            {
                Enrollments = new List<Enrollment>();
                Progress = new List<StudentProgress>();
                QuizAttempts = new List<StudentQuizAttempt>();
            }

            return Page();
        }
    }
}
