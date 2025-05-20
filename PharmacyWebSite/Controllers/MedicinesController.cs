using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyWebSite.Data;
using PharmacyWebSite.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace PharmacyWebSite.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MedicinesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const string ImageUploadFolder = "images/medicines";

        public MedicinesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchString, string categoryFilter)
        {
            var medicines = from m in _context.Medicines
                            select m;

            if (!string.IsNullOrEmpty(searchString))
            {
                medicines = medicines.Where(m => m.Name.Contains(searchString)
                                             || m.Description.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(categoryFilter))
            {
                medicines = medicines.Where(m => m.Category == categoryFilter);
            }

            ViewBag.Categories = await _context.Medicines
                .Select(m => m.Category)
                .Distinct()
                .ToListAsync();

            return View(await medicines.OrderByDescending(m => m.Id).ToListAsync());
        }


        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var medicine = await _context.Medicines.FindAsync(id);
            return medicine == null ? NotFound() : View(medicine);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Medicine medicine, IFormFile imageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        medicine.ImagePath = await SaveImage(imageFile);
                    }

                    _context.Add(medicine);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"{medicine.Name} added successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error adding medicine: {ex.Message}";
            }
            return View(medicine);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var medicine = await _context.Medicines.FindAsync(id);
            return medicine == null ? NotFound() : View(medicine);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Medicine medicine, IFormFile imageFile)
        {
            if (id != medicine.Id) return NotFound();

            try
            {
                var existingMedicine = await _context.Medicines.FindAsync(id);
                if (existingMedicine == null) return NotFound();

                existingMedicine.Name = medicine.Name;
                existingMedicine.Description = medicine.Description;
                existingMedicine.Price = medicine.Price;
                existingMedicine.Category = medicine.Category;
                existingMedicine.Stock = medicine.Stock;

                if (imageFile != null && imageFile.Length > 0)
                {
                    var newImagePath = await SaveImage(imageFile);
                    DeleteImage(existingMedicine.ImagePath);
                    existingMedicine.ImagePath = newImagePath;
                }

                _context.Update(existingMedicine);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"{medicine.Name} updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating medicine: {ex.Message}";
                return View(medicine);
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var medicine = await _context.Medicines.FindAsync(id);
            return medicine == null ? NotFound() : View(medicine);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var medicine = await _context.Medicines.FindAsync(id);
                if (medicine != null)
                {
                    DeleteImage(medicine.ImagePath);
                    _context.Medicines.Remove(medicine);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"{medicine.Name} deleted successfully!";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting medicine: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool MedicineExists(int id) => _context.Medicines.Any(e => e.Id == id);

        private async Task<string> SaveImage(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, ImageUploadFolder);
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"/{ImageUploadFolder}/{uniqueFileName}";
        }

        private void DeleteImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;

            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
    }
}