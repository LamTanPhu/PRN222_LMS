using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Models;
using Service.Interface;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPages_PRN222.Pages.Courses
{
    public class DetailsModel : PageModel
    {
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly ICourseReviewService _courseReviewService;

        public DetailsModel(ICourseService courseService, IEnrollmentService enrollmentService, ICourseReviewService courseReviewService)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
            _courseReviewService = courseReviewService;
        }

        public int? CourseId { get; set; }
        public Course Course { get; set; }
        public int? EnrollmentCount { get; set; }
        public decimal? AverageRating { get; set; }
        public List<Course> AllCourses { get; set; } = new List<Course>();

        public async Task OnGetAsync(int? id)
        {
            CourseId = id;
            
            // Get all courses for the dropdown
            AllCourses = await _courseService.GetAllAsync();
            
            if (id.HasValue)
            {
                int courseId = id.Value;
                
                // Get course details
                Course = await _courseService.GetByIdAsync(courseId);
                
                if (Course != null)
                {
                    // Get enrollment count
                    var allEnrollments = await _enrollmentService.GetAllAsync();
                    EnrollmentCount = allEnrollments.Count(e => e.CourseId == courseId);
                    
                    // Get average rating
                    var allReviews = await _courseReviewService.GetAllAsync();
                    var courseReviews = allReviews.Where(r => r.CourseId == courseId).ToList();
                    if (courseReviews.Any())
                    {
                        AverageRating = (decimal)courseReviews.Average(r => r.Rating);
                    }
                }
            }
        }
    }
}