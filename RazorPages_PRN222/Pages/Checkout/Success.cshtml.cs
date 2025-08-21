using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Models;
using Service.Interface;
using Service.Service;

namespace RazorPages_PRN222.Pages.Checkout
{
    public class SuccessModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IEnrollmentService _enrollmentService;

        public SuccessModel(IOrderService orderService, IEnrollmentService enrollmentService)
        {
            _orderService = orderService;
            _enrollmentService = enrollmentService;
        }

        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public async Task<IActionResult> OnGetAsync(int orderId)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Login");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            // Get order details
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null || order.UserId != userId)
            {
                return NotFound();
            }

            OrderId = order.OrderId;
            OrderDate = order.OrderDate ?? DateTime.Now;
            TotalAmount = order.FinalAmount;

            if (order.OrderItems != null && order.OrderItems.Any())
            {
                foreach (var item in order.OrderItems)
                {
                    await _enrollmentService.EnrollAsync(new Enrollment
                    {
                        UserId = userId,
                        CourseId = item.CourseId,
                    });
                }
            }

            return Page();
        }
    }
}

