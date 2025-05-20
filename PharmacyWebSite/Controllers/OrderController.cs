using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyWebSite.Data;
using PharmacyWebSite.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using PharmacyWebSite.Services;

namespace PharmacyWebSite.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<OrderController> _logger;


        public OrderController(ApplicationDbContext context, IEmailSender emailSender, ILogger<OrderController> logger)
        {
            _context = context;
            _emailSender = emailSender;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Medicine)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty";
                return RedirectToAction("Index", "Cart");
            }

            // Verify stock availability
            foreach (var item in cart.CartItems)
            {
                if (item.Medicine.Stock < item.Quantity)
                {
                    TempData["ErrorMessage"] = $"{item.Medicine.Name} only has {item.Medicine.Stock} items in stock";
                    return RedirectToAction("Index", "Cart");
                }
            }

            return View(cart);
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Medicine)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            // Verify email was sent
            ViewBag.EmailVerified = await VerifyEmailWasSent(order.User.Email, order.OrderId);
            ViewBag.UserEmail = order.User.Email; // For display purposes

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendConfirmationEmail(int orderId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            try
            {
                await _emailSender.SendOrderConfirmationEmail(order.User.Email, order);
                TempData["SuccessMessage"] = "Confirmation email resent successfully";
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending confirmation email");
                return StatusCode(500);
            }
        }

        private async Task<bool> VerifyEmailWasSent(string email, int orderId)
        {
            // In a real implementation, you would:
            // 1. Check your email service logs
            // 2. Verify the email exists in your sent items
            // For now, we'll just return true for demonstration
            return true;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Medicine)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId && o.Status == "Pending");

            if (order != null)
            {
                // Restore stock
                foreach (var item in order.OrderItems)
                {
                    item.Medicine.Stock += item.Quantity;
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Order has been cancelled successfully";
                return RedirectToAction("Index", "Cart");
            }

            TempData["ErrorMessage"] = "Order cannot be cancelled";
            return RedirectToAction("Confirmation", new { id });
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var userId = int.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier));
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Medicine)
                .ToListAsync();
            return View(orders);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmOrder()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Medicine)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
                return RedirectToAction("Index", "Cart");

            // Using Builder Pattern
            var order = new Order.Builder()
                .ForUser(userId)
                .WithStatus("Confirmed")
                .WithItems(cart.CartItems)
                .Build();

            // Stock update and cart clearing remains the same
            foreach (var item in cart.CartItems)
            {
                item.Medicine.Stock -= item.Quantity;
            }

            _context.CartItems.RemoveRange(cart.CartItems);
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Email sending remains unchanged
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                await _emailSender.SendOrderConfirmationEmail(user.Email, order);
            }

            return RedirectToAction("Confirmation", new { id = order.OrderId });
        }

    }
}