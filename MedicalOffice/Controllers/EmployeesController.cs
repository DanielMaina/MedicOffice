using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MedicalOffice.Data;
using MedicalOffice.Models;
using Microsoft.AspNetCore.Authorization;

namespace MedicalOffice.Controllers
{
    [Authorize(Roles ="Admin")]
    public class EmployeesController : Controller
    {
        private readonly MedicalOfficeContext _context;
        private readonly ApplicationDbContext _identityContext;

        public EmployeesController(MedicalOfficeContext context, ApplicationDbContext identityContext)
        {
            _context = context;
            _identityContext = identityContext;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            return View(await _context.Employees.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.ID == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        //// GET: Employees/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Employees/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Phone,FavouriteIceCream,eMail,Active")] Employee employee)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(employee);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(employee);
        //}

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, bool Active, Byte[] RowVersion)
        {
            var employeeToUpdate = await _context.Employees
               .SingleOrDefaultAsync(d => d.ID == id);
            if (employeeToUpdate == null)
            {
                return NotFound();
            }

            //Check to see if you are making them inactive
            if (employeeToUpdate.Active == true && Active == false)
            {
                //This deletes the user's login from the security system
                await DeleteIdentityUser(employeeToUpdate.eMail);

            }

            if (await TryUpdateModelAsync<Employee>(employeeToUpdate, "",
                d => d.FirstName, d => d.LastName, d => d.Phone, d => d.FavouriteIceCream, d => d.Active))
            {
                try
                {
                    _context.Entry(employeeToUpdate).Property("RowVersion").OriginalValue = RowVersion;
                    _context.Update(employeeToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employeeToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Concurrency Error.");
                    }
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Something went wrong in the database.");
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employeeToUpdate);
        }

        private async Task DeleteIdentityUser(string eMail)
        {
            var userToDelete = await _identityContext.Users.Where(u => u.Email == eMail).FirstOrDefaultAsync();
            _identityContext.Users.Remove(userToDelete);
            await _identityContext.SaveChangesAsync();
        }

        //// GET: Employees/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var employee = await _context.Employees
        //        .FirstOrDefaultAsync(m => m.ID == id);
        //    if (employee == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(employee);
        //}

        //// POST: Employees/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var employee = await _context.Employees.FindAsync(id);
        //    _context.Employees.Remove(employee);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.ID == id);
        }
    }
}
