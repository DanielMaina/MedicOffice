using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MedicalOffice.Data;
using MedicalOffice.Models;

namespace MedicalOffice.Controllers
{
    public class DoctorPhysicalDescriptionsController : Controller
    {
        private readonly MedicalOfficeContext _context;

        public DoctorPhysicalDescriptionsController(MedicalOfficeContext context)
        {
            _context = context;
        }

        // GET: DoctorPhysicalDescriptions
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Doctors");
        }

        // GET: DoctorPhysicalDescriptions/Create
        public IActionResult Add(int? DoctorID, string DoctorName)
        {
            if (!DoctorID.HasValue)
            {
                return RedirectToAction("Index", "Doctors");
            }
            ViewData["DoctorName"] = DoctorName;
            DoctorPhysicalDescription d = new DoctorPhysicalDescription()
            {
                DoctorID = DoctorID.GetValueOrDefault()
            };
            return View(d);
        }

        // POST: DoctorPhysicalDescriptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("DoctorID,Height,Weight,HairColour,IdentifyingMarks")] DoctorPhysicalDescription doctorPhysicalDescription, string DoctorName)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(doctorPhysicalDescription);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details","Doctors", new { id=doctorPhysicalDescription.DoctorID });
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            ViewData["DoctorName"] = DoctorName;
            return View(doctorPhysicalDescription);
        }

        // GET: DoctorPhysicalDescriptions/Edit/5
        public async Task<IActionResult> Update(int? id, string DoctorName)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorPhysicalDescription = await _context.DoctorPhysicalDescriptions.FindAsync(id);
            if (doctorPhysicalDescription == null)
            {
                return NotFound();
            }
            ViewData["DoctorName"] = DoctorName;
            return View(doctorPhysicalDescription);
        }

        // POST: DoctorPhysicalDescriptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("DoctorID,Height,Weight,HairColour,IdentifyingMarks")] DoctorPhysicalDescription doctorPhysicalDescription, string DoctorName)
        {
            if (id != doctorPhysicalDescription.DoctorID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctorPhysicalDescription);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", "Doctors", new { id = doctorPhysicalDescription.DoctorID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorPhysicalDescriptionExists(doctorPhysicalDescription.DoctorID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["DoctorName"] = DoctorName;
            return View(doctorPhysicalDescription);
        }

        private bool DoctorPhysicalDescriptionExists(int id)
        {
            return _context.DoctorPhysicalDescriptions.Any(e => e.DoctorID == id);
        }
    }
}
