using CoworkingApp.Data;
using CoworkingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoworkingApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CreditPackagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CreditPackagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CreditPackages
        public async Task<IActionResult> Index()
        {
            return View(await _context.CreditPackages.ToListAsync());
        }

        // GET: CreditPackages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var creditPackage = await _context.CreditPackages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (creditPackage == null)
            {
                return NotFound();
            }

            return View(creditPackage);
        }

        // GET: CreditPackages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CreditPackages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,Credits,IsActive")] CreditPackage creditPackage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(creditPackage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(creditPackage);
        }

        // GET: CreditPackages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var creditPackage = await _context.CreditPackages.FindAsync(id);
            if (creditPackage == null)
            {
                return NotFound();
            }
            return View(creditPackage);
        }

        // POST: CreditPackages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Credits,IsActive")] CreditPackage creditPackage)
        {
            if (id != creditPackage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(creditPackage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CreditPackageExists(creditPackage.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(creditPackage);
        }

        // GET: CreditPackages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var creditPackage = await _context.CreditPackages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (creditPackage == null)
            {
                return NotFound();
            }

            return View(creditPackage);
        }

        // POST: CreditPackages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var creditPackage = await _context.CreditPackages.FindAsync(id);
            if (creditPackage != null)
            {
                _context.CreditPackages.Remove(creditPackage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CreditPackageExists(int id)
        {
            return _context.CreditPackages.Any(e => e.Id == id);
        }
    }
}
