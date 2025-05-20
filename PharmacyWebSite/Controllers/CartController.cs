using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyWebSite.Data;
using PharmacyWebSite.Models;
using System.Security.Claims;

namespace PharmacyWebSite.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Medicine)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return View(cart);
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItemCount()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Content("0");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var count = await _context.CartItems
                .Where(ci => ci.Cart.UserId == userId)
                .SumAsync(ci => ci.Quantity);

            return Content(count.ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int medicineId, int quantity = 1)
        {
            try
            {
                var medicine = await _context.Medicines.FindAsync(medicineId);
                if (medicine == null)
                {
                    return Json(new { success = false, message = "Medicine not found" });
                }

                if (medicine.Stock < quantity)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Only {medicine.Stock} items available in stock"
                    });
                }

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var cart = await GetOrCreateCartAsync(userId);

                var existingItem = cart.CartItems.FirstOrDefault(ci => ci.MedicineId == medicineId);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                    existingItem.Price = medicine.Price;
                }
                else
                {
                    cart.CartItems.Add(new CartItem
                    {
                        MedicineId = medicineId,
                        Quantity = quantity,
                        Price = medicine.Price
                    });
                }

                await _context.SaveChangesAsync();

                var count = await _context.CartItems
                    .Where(ci => ci.Cart.UserId == userId)
                    .SumAsync(ci => ci.Quantity);

                return Json(new
                {
                    success = true,
                    count,
                    message = $"{medicine.Name} added to cart!"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occurred: " + ex.Message
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    var count = await _context.CartItems
                        .Where(ci => ci.Cart.UserId == userId)
                        .SumAsync(ci => ci.Quantity);

                    return Json(new
                    {
                        success = true,
                        message = "Item removed from cart",
                        count
                    });
                }

                TempData["SuccessMessage"] = "Item removed from cart";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems
                .Include(ci => ci.Medicine)
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);

            if (cartItem == null)
            {
                return Json(new { success = false, message = "Item not found" });
            }

            if (quantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                if (cartItem.Medicine.Stock < quantity)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Only {cartItem.Medicine.Stock} available in stock"
                    });
                }
                cartItem.Quantity = quantity;
            }

            await _context.SaveChangesAsync();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var count = await _context.CartItems
                .Where(ci => ci.Cart.UserId == userId)
                .SumAsync(ci => ci.Quantity);

            var total = cartItem.Quantity * cartItem.Price;
            var cartTotal = await _context.CartItems
                .Where(ci => ci.Cart.UserId == userId)
                .SumAsync(ci => ci.Quantity * ci.Price);

            return Json(new
            {
                success = true,
                count,
                itemTotal = total.ToString("C"),
                cartTotal = cartTotal.ToString("C")
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                return RedirectToAction("Index");
            }

            foreach (var item in cart.CartItems)
            {
                if (item.Medicine.Stock < item.Quantity)
                {
                    TempData["ErrorMessage"] = $"{item.Medicine.Name} only has {item.Medicine.Stock} items in stock";
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Checkout", "Order");
        }

        private async Task<Cart> GetOrCreateCartAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }
    }
}