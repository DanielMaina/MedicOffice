using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MedicalOffice.Data;
using MedicalOffice.Models;
using MedicalOffice.Utilities;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class PatientApptController : Controller
{
    private readonly MedicalOfficeContext _context;

    public PatientApptController(MedicalOfficeContext context)
    {
        _context = context;
    }

    // GET: Appointments
    public async Task<IActionResult> Index(int? PatientID, int? page, int? pageSizeID, int? ApptReasonID, string actionButton,
        string SearchString, string sortDirection = "desc", string sortField = "Appointment")
    {
        if (!PatientID.HasValue)
        {
            //Can't work without a patient so go back to Patient Index
            return RedirectToAction("Index", "Patients");
        }
        PopulateDropDownLists();
        ViewData["Filtering"] = "btn-outline-secondary";//Instead of keeping the filter open, change colour of button

        var appts = from a in _context.Appointments
                    .Include(a => a.ApptReason)
                    .Include(a => a.Patient)
                    where a.PatientID == PatientID.GetValueOrDefault()
                    select a;

        if (ApptReasonID.HasValue)
        {
            appts = appts.Where(p => p.ApptReasonID == ApptReasonID);
            ViewData["Filtering"] = "btn-danger";
        }
        if (!String.IsNullOrEmpty(SearchString))
        {
            appts = appts.Where(p => p.Notes.ToUpper().Contains(SearchString.ToUpper()));
            ViewData["Filtering"] = "btn-danger";
        }
        //Before we sort, see if we have called for a change of filtering or sorting
        if (!String.IsNullOrEmpty(actionButton)) //Form Submitted so lets sort!
        {
            page = 1;//Reset back to first page when sorting or filtering

            if (actionButton != "Filter")//Change of sort is requested
            {
                if (actionButton == sortField) //Reverse order on same field
                {
                    sortDirection = sortDirection == "asc" ? "desc" : "asc";
                }
                sortField = actionButton;//Sort by the button clicked
            }
        }
        //Now we know which field and direction to sort by
        if (sortField == "Extra Fees")//Sorting by Patient Name
        {
            if (sortDirection == "asc")
            {
                appts = appts
                    .OrderBy(p => p.extraFee);
            }
            else
            {
                appts = appts
                    .OrderByDescending(p => p.extraFee);
            }
        }
        else if (sortField.Contains("Reason"))
        {
            if (sortDirection == "asc")
            {
                appts = appts
                    .OrderBy(p => p.ApptReason.ReasonName);
            }
            else
            {
                appts = appts
                    .OrderByDescending(p => p.ApptReason.ReasonName);
            }
        }
        else //Date
        {
            if (sortDirection == "asc")
            {
                appts = appts
                    .OrderByDescending(p => p.appDate);
            }
            else
            {
                appts = appts
                    .OrderBy(p => p.appDate);
            }
        }
        //Set sort for next time
        ViewData["sortField"] = sortField;
        ViewData["sortDirection"] = sortDirection;

        //Now get the MASTER record, the patient, so it can be displayed at the top of the screen
        Patient patient = _context.Patients
            .Include(p => p.Doctor)
            .Include(p => p.PatientConditions)
            .ThenInclude(pc => pc.Condition)
            .Where(p => p.ID == PatientID.GetValueOrDefault()).FirstOrDefault();
        ViewBag.Patient = patient;

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
        var pagedData = await PaginatedList<Appointment>.CreateAsync(appts.AsNoTracking(), page ?? 1, pageSize);
        return View(pagedData);
    }

    // GET: PatientAppt/Add
    public IActionResult Add(int? PatientID, string PatientName)
    {
        if (!PatientID.HasValue)
        {
            return RedirectToAction("Index", "Patients");
        }
        ViewData["PatientName"] = PatientName;
        Appointment a = new Appointment()
        {
            PatientID = PatientID.GetValueOrDefault()
        };
        PopulateDropDownLists();
        return View(a);
    }

    // POST: PatientAppt/Add
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add([Bind("ID,Notes,appDate,PatientID,extraFee,ApptReasonID")] Appointment appointment, string PatientName)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { appointment.PatientID });
            }
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
        }
        PopulateDropDownLists(appointment);
        ViewData["PatientName"] = PatientName;
        return View(appointment);
    }

    // GET: PatientAppt/Edit/5
    public async Task<IActionResult> Update(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var appointment = await _context.Appointments
            .Include(a => a.ApptReason)
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(m => m.ID == id);
        if (appointment == null)
        {
            return NotFound();
        }
        PopulateDropDownLists(appointment);
        return View(appointment);
    }

    // POST: PatientAppt/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id)
    {
        var appointmentToUpdate = await _context.Appointments
            .Include(a => a.ApptReason)
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(m => m.ID == id);
        //Check that you got it or exit with a not found error
        if (appointmentToUpdate == null)
        {
            return NotFound();
        }

        //Try updating it with the values posted
        if (await TryUpdateModelAsync<Appointment>(appointmentToUpdate, "",
            p => p.Notes, p => p.appDate, p => p.extraFee, p => p.ApptReasonID))
        {
            try
            {
                _context.Update(appointmentToUpdate);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { appointmentToUpdate.PatientID });
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(appointmentToUpdate.ID))
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
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
        }
        PopulateDropDownLists(appointmentToUpdate);
        return View(appointmentToUpdate);
    }

    // GET: PatientAppt/Delete/5
    public async Task<IActionResult> Remove(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var appointment = await _context.Appointments
            .Include(a => a.ApptReason)
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(m => m.ID == id);
        if (appointment == null)
        {
            return NotFound();
        }
        return View(appointment);
    }

    // POST: PatientAppt/Delete/5
    [HttpPost, ActionName("Remove")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveConfirmed(int id)
    {
        var appointment = await _context.Appointments
            .Include(a => a.ApptReason)
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(m => m.ID == id);

        try
        {
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { appointment.PatientID });
        }
        catch (Exception)
        {
            ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
        }

        return View(appointment);
    }

    private SelectList ApptReasonSelectList(int? id)
    {
        var dQuery = from d in _context.ApptReasons
                     orderby d.ReasonName
                     select d;
        return new SelectList(dQuery, "ID", "ReasonName", id);
    }

    private void PopulateDropDownLists(Appointment appointment = null)
    {
        ViewData["ApptReasonID"] = ApptReasonSelectList(appointment?.ApptReasonID);
    }

    private bool AppointmentExists(int id)
    {
        return _context.Appointments.Any(e => e.ID == id);
    }

}
