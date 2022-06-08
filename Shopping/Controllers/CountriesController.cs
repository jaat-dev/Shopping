using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Data;
using Shopping.Data.Entities;

namespace Shopping.Controllers;

public class CountriesController : Controller
{
    private readonly DataContext _context;

    public CountriesController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return _context.Countries != null ?
                    View(await _context.Countries.ToListAsync()) :
                    Problem("Entity set 'DataContext.Countries'  is null.");
    }

    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Countries == null)
        {
            return NotFound();
        }

        var country = await _context.Countries
            .FirstOrDefaultAsync(m => m.Id == id);
        if (country == null)
        {
            return NotFound();
        }

        return View(country);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Country country)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _context.Add(country);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException!.Message.Contains("duplicate"))
                {
                    ModelState.AddModelError(string.Empty, "Ya existe un país con el mismo nombre.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, dbEx.InnerException.Message);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }
        return View(country);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Countries == null)
        {
            return NotFound();
        }

        var country = await _context.Countries.FindAsync(id);
        if (country == null)
        {
            return NotFound();
        }
        return View(country);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Country country)
    {
        if (id != country.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(country);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException!.Message.Contains("duplicate"))
                {
                    ModelState.AddModelError(string.Empty, "Ya existe un país con el mismo nombre.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, dbEx.InnerException.Message);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }
        return View(country);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Countries == null)
        {
            return NotFound();
        }

        var country = await _context.Countries.FindAsync(id);
        if (country == null)
        {
            return NotFound();
        }

        return View(country);
    }


    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Countries == null)
        {
            return Problem("Entity set 'DataContext.Countries'  is null.");
        }
        var country = await _context.Countries.FindAsync(id);
        if (country != null)
        {
            _context.Countries.Remove(country);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
