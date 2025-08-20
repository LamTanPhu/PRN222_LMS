using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interface;
using Repository.Models;

namespace RazorPages_PRN222.Pages.Quiz
{
    public class ResultModel : PageModel
    {
        private readonly IStudentQuizAttemptService _attemptService;

        public ResultModel(IStudentQuizAttemptService attemptService)
        {
            _attemptService = attemptService;
        }

        public StudentQuizAttempt? Attempt { get; set; }

        public async Task<IActionResult> OnGetAsync(int attemptId)
        {
            try
            {
                if (attemptId <= 0)
                {
                    return BadRequest();
                }

                Attempt = await _attemptService.GetByIdAsync(attemptId);
                if (Attempt == null)
                {
                    return NotFound();
                }

                return Page();
            }
            catch (Exception ex)
            {
                // Log error and return error page
                return RedirectToPage("/Error");
            }
        }
    }
} 