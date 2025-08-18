using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Models;
using Service.Interface;
using Service.Service;
using System.Threading.Tasks;

namespace RazorPages_PRN222.Pages.Admin
{
    public class AnnouncementsModel : PageModel
    {
        private readonly IAnnouncementService announcementService;

        public List<Announcement> Announcements { get; set; }

        public AnnouncementsModel(IAnnouncementService announcementService)
        {
            this.announcementService = announcementService;
        }

        public async Task OnGetAsync()
        {
            Announcements = await announcementService.GetAllAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await announcementService.DeleteAsync(id);
            return RedirectToPage();
        }
    }
}