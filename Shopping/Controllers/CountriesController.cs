using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Data;
using Shopping.Data.Entities;
using Shopping.Models;

namespace Shopping.Controllers;

[Authorize(Roles = "Admin")]
public class CountriesController : Controller
{
    private readonly DataContext _context;

    public CountriesController(DataContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return _context.Countries != null ?
                    View(await _context.Countries.Include(c => c.States).ToListAsync()) :
                    Problem("Entity set 'DataContext.Countries'  is null.");
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Country? country = await _context.Countries!
            .Include(c => c.States!)
            .ThenInclude(s => s.Cities)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (country == null)
        {
            return NotFound();
        }

        return View(country);
    }

    public IActionResult Create()
    {
        Country country = new() { States = new List<State>() };
        return View(country);
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

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Country? country = await _context.Countries!
            .Include(c => c.States)
            .FirstOrDefaultAsync(C => C.Id == id);
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

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Country? country = await _context.Countries!
            .Include(c=>c.States)
            .FirstOrDefaultAsync(c => c.Id == id);
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
        Country? country = await _context
            .Countries
            .FindAsync(id);
        if (country != null)
        {
            _context.Countries.Remove(country);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> AddState(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Country? country = await _context.Countries!.FindAsync(id);
        if (country == null)
        {
            return NotFound();
        }

        StateViewModel model = new() { CountryId = country.Id };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddState(StateViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                State state = new()
                {
                    Cities = new List<City>(),
                    Country = await _context.Countries!.FindAsync(model.CountryId),
                    Name = model.Name
                };

                _context.Add(state);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = model.CountryId });
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException!.Message.Contains("duplicate"))
                {
                    ModelState.AddModelError(string.Empty, "Ya existe un departamento/Estado con el mismo nombre en este país.");
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
        return View(model);
    }

    public async Task<IActionResult> EditState(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        State? state = await _context.States!
            .Include(s => s.Country)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (state == null)
        {
            return NotFound();
        }

        StateViewModel model = new()
        {
            CountryId = state.Country!.Id,
            Id = state.Id,
            Name = state.Name
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditState(int id, StateViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                State state = new()
                {
                    Id = model.Id,
                    Name = model.Name
                };

                _context.Update(state);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { Id = model.CountryId });
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException!.Message.Contains("duplicate"))
                {
                    ModelState.AddModelError(string.Empty, "Ya existe un departamento/estado en este país.");
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
        return View(model);
    }

    public async Task<IActionResult> DetailsState(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        State? state = await _context.States!
            .Include(s => s.Country)
            .Include(s => s.Cities)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (state == null)
        {
            return NotFound();
        }

        return View(state);
    }

    public async Task<IActionResult> AddCity(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        State? state = await _context.States!.FindAsync(id);
        if (state == null)
        {
            return NotFound();
        }

        CityViewModel model = new() { StateId = state.Id };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCity(CityViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                City city = new()
                {
                    State = await _context.States!
                    .FindAsync(model.StateId),
                    Name = model.Name
                };
                _context.Add(city);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DetailsState), new { id = model.StateId });
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException!.Message.Contains("duplicate"))
                {
                    ModelState.AddModelError(string.Empty, "Ya existe un ciudad con el mismo nombre en este departamento/estado.");
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
        return View(model);
    }

    public async Task<IActionResult> EditCity(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        City? city = await _context.Cities!
            .Include(c => c.State)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (city == null)
        {
            return NotFound();
        }

        CityViewModel model = new()
        {
            StateId = city.State!.Id,
            Id = city.Id,
            Name = city.Name
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCity(int id, CityViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                City city = new()
                {
                    Id = model.Id,
                    Name = model.Name
                };

                _context.Update(city);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DetailsState), new { Id = model.StateId });
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException!.Message.Contains("duplicate"))
                {
                    ModelState.AddModelError(string.Empty, "Ya existe una ciudad en este departamento/estado.");
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
        return View(model);
    }

    public async Task<IActionResult> DetailsCity(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        City? city = await _context.Cities!
            .Include(c => c.State)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (city == null)
        {
            return NotFound();
        }

        return View(city);
    }

    public async Task<IActionResult> DeleteState(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        State? state = await _context.States!
            .Include(s => s.Country)
            .Include(s => s.Cities)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (state == null)
        {
            return NotFound();
        }

        return View(state);
    }

    [HttpPost, ActionName("DeleteState")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteStateConfirmed(int id)
    {
        if (_context.Countries == null)
        {
            return Problem("Entity set 'DataContext.Countries'  is null.");
        }
        State? state = await _context.States!
            .Include(s => s.Country)
            .Include(s => s.Cities)
            .FirstOrDefaultAsync(s => s.Id == id);
        int countryId = 0;
        if (state != null)
        {
            _context.States!.Remove(state);
            countryId = state.Country!.Id;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new {Id = countryId});
    }

    public async Task<IActionResult> DeleteCity(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        City? city = await _context.Cities!
            .Include(c => c.State)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (city == null)
        {
            return NotFound();
        }

        return View(city);
    }

    [HttpPost, ActionName("DeleteCity")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCityConfirmed(int id)
    {
        if (_context.Countries == null)
        {
            return Problem("Entity set 'DataContext.Countries'  is null.");
        }
        City? city = await _context.Cities!
            .Include(c => c.State)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (city != null)
        {
            _context.Cities!.Remove(city);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(DetailsState), new {Id = city!.State!.Id});
    }


}
