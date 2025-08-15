using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPages_PRN222.Pages.Courses
{
    public class DetailsModel : PageModel
    {
        public int? CourseId { get; set; }

        public void OnGet(int? id)
        {
            CourseId = id;
        }
    }
}