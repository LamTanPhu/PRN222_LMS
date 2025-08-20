using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interface;
using Repository.Models;
using Microsoft.AspNetCore.Http;

namespace RazorPages_PRN222.Pages.Checkout
{
    public class IndexModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IOrderItemService _orderItemService;
        private readonly IPaymentService _paymentService;
        private readonly ICouponService _couponService;
        private readonly ICourseService _courseService;

        public IndexModel(IOrderService orderService, IOrderItemService orderItemService, 
                         IPaymentService paymentService, ICouponService couponService, ICourseService courseService)
        {
            _orderService = orderService;
            _orderItemService = orderItemService;
            _paymentService = paymentService;
            _couponService = couponService;
            _courseService = courseService;
        }

        public List<Course> CartItems { get; set; } = new List<Course>();
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string AppliedCoupon { get; set; }
        public string CouponMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Login");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            // Get cart items from session
            CartItems = await GetCartItemsAsync();
            CalculateTotals();

            return Page();
        }

        public async Task<IActionResult> OnPostApplyCouponAsync(string couponCode)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Login");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            // Get cart items
            CartItems = await GetCartItemsAsync();
            CalculateTotals();

            // Validate coupon
            var coupon = await _couponService.GetCouponByCodeAsync(couponCode);
            if (coupon != null && coupon.IsActive == true && coupon.ValidUntil > DateTime.Now)
            {
                AppliedCoupon = couponCode;
                if (coupon.DiscountType == "Percentage")
                {
                    DiscountAmount = Subtotal * (coupon.DiscountValue / 100m);
                }
                else
                {
                    DiscountAmount = coupon.DiscountValue;
                }
                TotalAmount = Subtotal - DiscountAmount;
                CouponMessage = $"Coupon applied successfully! {coupon.DiscountValue}% discount.";
            }
            else
            {
                CouponMessage = "Invalid or expired coupon code.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostProcessPaymentAsync(string cardNumber, string expiryDate, 
                                                                  string cvv, string cardholderName)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Login");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return RedirectToPage("/Login");

            // Get cart items
            CartItems = await GetCartItemsAsync();
            CalculateTotals();

            // Apply coupon if exists
            if (!string.IsNullOrEmpty(AppliedCoupon))
            {
                var coupon = await _couponService.GetCouponByCodeAsync(AppliedCoupon);
                if (coupon != null)
                {
                    if (coupon.DiscountType == "Percentage")
                    {
                        DiscountAmount = Subtotal * (coupon.DiscountValue / 100m);
                    }
                    else
                    {
                        DiscountAmount = coupon.DiscountValue;
                    }
                    TotalAmount = Subtotal - DiscountAmount;
                }
            }

            // Create order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = Subtotal,
                DiscountAmount = DiscountAmount,
                FinalAmount = TotalAmount,
                OrderStatus = "Pending",
                PaymentMethod = "Credit Card"
            };

            await _orderService.CreateAsync(order);

            // Create order items
            foreach (var course in CartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    CourseId = course.CourseId,
                    Price = course.Price
                };
                await _orderItemService.CreateAsync(orderItem);
            }

            // Create payment
            var payment = new Payment
            {
                OrderId = order.OrderId,
                Amount = TotalAmount,
                PaymentDate = DateTime.Now,
                PaymentGateway = "Credit Card",
                PaymentStatus = "Completed"
            };

            await _paymentService.CreateAsync(payment);

            // Clear cart
            await ClearCartAsync();

            return RedirectToPage("/Checkout/Success", new { orderId = order.OrderId });
        }

        private void CalculateTotals()
        {
            Subtotal = CartItems.Sum(c => c.Price);
            TotalAmount = Subtotal - DiscountAmount;
        }

        public async Task<IActionResult> OnPostRemoveItemAsync(int courseId)
        {
            if (!User.Identity?.IsAuthenticated ?? true) return RedirectToPage("/Login");
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var sessionKey = $"cart_{userIdClaim}";
            var json = HttpContext.Session.GetString(sessionKey);
            var ids = string.IsNullOrEmpty(json) ? new List<int>() : System.Text.Json.JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
            if (ids.Contains(courseId))
            {
                ids.Remove(courseId);
                HttpContext.Session.SetString(sessionKey, System.Text.Json.JsonSerializer.Serialize(ids));
            }
            CartItems = await GetCartItemsAsync();
            CalculateTotals();
            return Page();
        }

        private async Task<List<Course>> GetCartItemsAsync()
        {
            // Session key per user
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var sessionKey = $"cart_{userIdClaim}";

            var json = HttpContext.Session.GetString(sessionKey);
            if (string.IsNullOrEmpty(json))
            {
                return new List<Course>();
            }

            try
            {
                var ids = System.Text.Json.JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
                var allCourses = await _courseService.GetAllAsync();
                return allCourses.Where(c => ids.Contains(c.CourseId)).ToList();
            }
            catch
            {
                return new List<Course>();
            }
        }

        private async Task ClearCartAsync()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var sessionKey = $"cart_{userIdClaim}";
            HttpContext.Session.Remove(sessionKey);
            await Task.CompletedTask;
        }
    }
}
