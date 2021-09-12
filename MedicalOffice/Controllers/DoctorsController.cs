using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MedicalOffice.Data;
using MedicalOffice.Models;
using MedicalOffice.ViewModels;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Authorization;
using MedicalOffice.Utilities;

namespace MedicalOffice.Controllers
{
    [Authorize(Roles ="Admin,Supervisor")]
    public class DoctorsController : Controller
    {
        private readonly MedicalOfficeContext _context;

        public DoctorsController(MedicalOfficeContext context)
        {
            _context = context;
        }

        // GET: Doctors
        public async Task<IActionResult> Index(int? page, int? pageSizeID)
        {
            var doctors = from d in _context.Doctors
                .Include(d => d.DoctorSpecialties).ThenInclude(d => d.Specialty)
                          select d;
            //Handle Paging
            int pageSize;//This is the value we will pass to PaginatedList
            if (pageSizeID.HasValue)
            {
                //Value selected from DDL so use and save it to Cookie
                pageSize = pageSizeID.GetValueOrDefault();
                CookieHelper.CookieSet(HttpContext, "pageSizeValue", pageSize.ToString(), 30);
            }
            else
            {
                //Not selected so see if it is in Cookie
                pageSize = Convert.ToInt32(HttpContext.Request.Cookies["pageSizeValue"]);
            }
            pageSize = (pageSize == 0) ? 2 : pageSize;//Neither Selected or in Cookie so go with default
            ViewData["pageSizeID"] =
                new SelectList(new[] { "2", "5", "10", "20", "30", "40", "50", "100", "500" }, pageSize.ToString());
            var pagedData = await PaginatedList<Doctor>.CreateAsync(doctors.AsNoTracking(), page ?? 1, pageSize);
            return View(pagedData);
        }

        // GET: Doctors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors.Include(d=>d.Patients)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // GET: Doctors/Create
        public IActionResult Create()
        {
            Doctor doctor = new Doctor();
            PopulateAssignedSpecialtyData(doctor);
            return View();
        }

        // POST: Doctors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,MiddleName,LastName")] Doctor doctor, string[] selectedOptions)
        {
            try
            {
                UpdateDoctorSpecialties(selectedOptions, doctor);
                if (ModelState.IsValid)
                {
                    _context.Add(doctor);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Something went wrong in the database.");
            }
            PopulateAssignedSpecialtyData(doctor);
            return View(doctor);
        }

        // GET: Doctors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
               .Include(d => d.DoctorSpecialties).ThenInclude(d => d.Specialty)
               .AsNoTracking()
               .SingleOrDefaultAsync(d => d.ID == id);

            if (doctor == null)
            {
                return NotFound();
            }
            PopulateAssignedSpecialtyData(doctor);
            return View(doctor);
        }

        // POST: Doctors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string[] selectedOptions)
        {
            //Get the object from the database
            var doctorToUpdate = await _context.Doctors
                .Include(d => d.DoctorSpecialties).ThenInclude(d => d.Specialty)
                .SingleOrDefaultAsync(d => d.ID == id);
            if (doctorToUpdate==null)
            {
                return NotFound();
            }

            //Update the Doctor's Specialties
            UpdateDoctorSpecialties(selectedOptions, doctorToUpdate);


            //Try updating it wiht the posted values
            if (await TryUpdateModelAsync<Doctor>(doctorToUpdate, "",
                d=>d.FirstName, d => d.MiddleName, d => d.LastName))
            {
                try
                {
                    _context.Update(doctorToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctorToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Something went wrong in the database.");
                }
                return RedirectToAction(nameof(Index));
            }
            //Validaiton Error so give the user another chance.
            PopulateAssignedSpecialtyData(doctorToUpdate);
            return View(doctorToUpdate);
        }

        // GET: Doctors/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(m => m.ID == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            try
            {
                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dex)
            {
                if (dex.InnerException.Message.Contains("FK_Patients_Doctors_DoctorID"))
                {
                    ModelState.AddModelError("", "Unable to save changes. Remember, you cannot delete a Doctor that has Patients.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            return View(doctor);

        }

        public PartialViewResult ListOfSpecialtiesDetails(int id)
        {
            var query = from s in _context.DoctorSpecialties.Include(p => p.Specialty)
                        where s.DoctorID == id
                        select s;
            return PartialView("_ListOfSpecialities", query.ToList());
        }

        public PartialViewResult ListOfPatientsDetails(int id)
        {
            var query = from s in _context.Patients
                        where s.DoctorID == id
                        select s;
            return PartialView("_ListOfPatients", query.ToList());
        }
        public PartialViewResult DoctorPD(int id)
        {
            //Now get the MASTER record, the Doctor, so it can be displayed at the top of the screen
            Doctor doctor = _context.Doctors
                .Where(p => p.ID == id).FirstOrDefault();
            ViewBag.Doctor = doctor;
            var query = from d in _context.DoctorPhysicalDescriptions
                        where d.DoctorID == id
                        select d;
            return PartialView("_DoctorPhysicalDescription", query.FirstOrDefault());
        }
                
        private void PopulateAssignedSpecialtyData(Doctor doctor)
        {
            var allSpecialties = _context.Specialties;
            var docSpecialties = new HashSet<int>(doctor.DoctorSpecialties.Select(b => b.SpecialtyID));
            var selected = new List<OptionVM>();
            var available = new List<OptionVM>();
            foreach (var s in allSpecialties)
            {
                if (docSpecialties.Contains(s.ID))
                {
                    selected.Add(new OptionVM
                    {
                        ID = s.ID,
                        DisplayText = s.SpecialtyName
                    });
                }
                else
                {
                    available.Add(new OptionVM
                    {
                        ID = s.ID,
                        DisplayText = s.SpecialtyName
                    });
                }
            }

            ViewData["selOpts"] = new MultiSelectList(selected.OrderBy(s => s.DisplayText), "ID", "DisplayText");
            ViewData["availOpts"] = new MultiSelectList(available.OrderBy(s => s.DisplayText), "ID", "DisplayText");
        }
        private void UpdateDoctorSpecialties(string[] selectedOptions, Doctor doctorToUpdate)
        {
            if (selectedOptions == null)
            {
                doctorToUpdate.DoctorSpecialties = new List<DoctorSpecialty>();
                return;
            }

            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var docSpecialties = new HashSet<int>(doctorToUpdate.DoctorSpecialties.Select(b => b.SpecialtyID));
            foreach (var s in _context.Specialties)
            {
                if (selectedOptionsHS.Contains(s.ID.ToString()))
                {
                    if (!docSpecialties.Contains(s.ID))
                    {
                        doctorToUpdate.DoctorSpecialties.Add(new DoctorSpecialty
                        {
                            SpecialtyID = s.ID,
                            DoctorID = doctorToUpdate.ID
                        });
                    }
                }
                else
                {
                    if (docSpecialties.Contains(s.ID))
                    {
                        DoctorSpecialty specToRemove = doctorToUpdate.DoctorSpecialties.SingleOrDefault(d => d.SpecialtyID == s.ID);
                        _context.Remove(specToRemove);
                    }
                }
            }
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.ID == id);
        }
    }
}
