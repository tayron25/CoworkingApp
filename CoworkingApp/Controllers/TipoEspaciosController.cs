using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CoworkingApp.Data;
using CoworkingApp.Models;

namespace CoworkingApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TipoEspaciosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TipoEspaciosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TipoEspacios
        public async Task<IActionResult> Index()
        {
            return View(await _context.TiposEspacio.ToListAsync());
        }

        // GET: TipoEspacios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoEspacio = await _context.TiposEspacio
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoEspacio == null)
            {
                return NotFound();
            }

            return View(tipoEspacio);
        }

        // GET: TipoEspacios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoEspacios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Description,CostoCreditosHora")] TipoEspacio tipoEspacio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoEspacio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoEspacio);
        }

        // GET: TipoEspacios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoEspacio = await _context.TiposEspacio.FindAsync(id);
            if (tipoEspacio == null)
            {
                return NotFound();
            }
            return View(tipoEspacio);
        }

        // POST: TipoEspacios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Description,CostoCreditosHora")] TipoEspacio tipoEspacio)
        {
            if (id != tipoEspacio.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoEspacio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoEspacioExists(tipoEspacio.Id))
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
            return View(tipoEspacio);
        }

        // GET: TipoEspacios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoEspacio = await _context.TiposEspacio
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoEspacio == null)
            {
                return NotFound();
            }

            return View(tipoEspacio);
        }

        // POST: TipoEspacios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoEspacio = await _context.TiposEspacio.FindAsync(id);
            if (tipoEspacio != null)
            {
                _context.TiposEspacio.Remove(tipoEspacio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoEspacioExists(int id)
        {
            return _context.TiposEspacio.Any(e => e.Id == id);
        }
    }
}
