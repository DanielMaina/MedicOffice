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
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using MedicalOffice.Utilities;

namespace MedicalOffice.Controllers
{
    [Authorize]
    public class PatientsController : Controller
    {
        private readonly MedicalOfficeContext _context;

        public PatientsController(MedicalOfficeContext context)
        {
            _context = context;
        }

        // GET: Patients
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? page, int? pageSizeID, int? DoctorID, string SearchString, 
            int? ConditionID, string actionButton, string sortDirection = "asc", string sortField = "Patient"
)
        {
            ViewData["ConditionID"] = new SelectList(_context
                .Conditions
                .OrderBy(c => c.ConditionName), "ID", "ConditionName");
            PopulateDropDownLists();
            ViewData["Filtering"] = "";  //Assume not filtering

            //Start with Includes but make sure your expression returns an
            //IQueryable<Patient> so we can add filter and sort 
            //options later.
            var patients =from d in _context.Patients
                .Include(p => p.Doctor)
                .Include(p => p.MedicalTrial)
                .Include(p=>p.PatientConditions).ThenInclude(pc=>pc.Condition)
                select d;

            //Add as many filters as needed
            if(DoctorID.HasValue)
            {
                patients = patients.Where(p => p.DoctorID == DoctorID);
                ViewData["Filtering"] = " show";
            }
            if (ConditionID.HasValue)
            {
                patients = patients.Where(p => p.PatientConditions.Any(c => c.ConditionID == ConditionID));
                ViewData["Filtering"] = " show";
            }
            if (!String.IsNullOrEmpty(SearchString))
            {
                patients = patients.Where(p => p.LastName.ToUpper().Contains(SearchString.ToUpper())
                                       || p.FirstName.ToUpper().Contains(SearchString.ToUpper()));
                ViewData["Filtering"] = " show";
            }

            //Before we sort, see if we have called for a change of filtering or sorting
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted so lets sort!
            {
                page = 1;//Reset page to start

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
            if (sortField == "Visits/Yr")
            {
                if (sortDirection == "asc")
                {
                    patients = patients
                        .OrderBy(p => p.ExpYrVisits);
                }
                else
                {
                    patients = patients
                        .OrderByDescending(p => p.ExpYrVisits);
                }
            }
            else if (sortField == "Age")
            {
                if (sortDirection == "asc")
                {
                    patients = patients
                        .OrderByDescending(p => p.DOB);
                }
                else
                {
                    patients = patients
                        .OrderBy(p => p.DOB);
                }
            }
            else if (sortField == "Doctor")
            {
                if (sortDirection == "asc")
                {
                    patients = patients
                        .OrderBy(p => p.Doctor.LastName)
                        .ThenBy(p => p.Doctor.FirstName);
                }
                else
                {
                    patients = patients
                        .OrderByDescending(p => p.Doctor.LastName)
                        .ThenByDescending(p => p.Doctor.FirstName);
                }
            }
            else if (sortField == "Medical Trial")
            {
                if (sortDirection == "asc")
                {
                    patients = patients
                        .OrderBy(p => p.MedicalTrial.TrialName);
                }
                else
                {
                    patients = patients
                        .OrderByDescending(p => p.MedicalTrial.TrialName);
                }
            }
            else //Sorting by Patient Name
            {
                if (sortDirection == "asc")
                {
                    patients = patients
                        .OrderBy(p => p.LastName)
                        .ThenBy(p => p.FirstName);
                }
                else
                {
                    patients = patients
                        .OrderByDescending(p => p.LastName)
                        .ThenByDescending(p => p.FirstName);
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

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
            var pagedData = await PaginatedList<Patient>.CreateAsync(patients.AsNoTracking(), page ?? 1, pageSize);

            if(User.IsInRole("Admin"))
            {
                return View("IndexAdmin", pagedData);
            }
            else if(User.Identity.IsAuthenticated)
            {
                return View("IndexLoggedIn", pagedData);
            }
            return View(pagedData);
        }

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.Doctor)
                .Include(p => p.MedicalTrial)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            var patient = new Patient();
            //patient.PatientConditions = new List<PatientCondition>();
            PopulateAssignedConditionData(patient);
            PopulateDropDownLists();
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OHIP,FirstName,MiddleName,LastName,DOB,ExpYrVisits,Phone,eMail,DoctorID,MedicalTrialID")] Patient patient, string[] selectedOptions, IFormFile thePicture)
        {
            try
            {
                //Add the selected conditions
                if (selectedOptions != null)
                {
                    //Build a dictionary of selected options with comments
                    var allComments = new Dictionary<int, string>();
                    var commentInputs = Request.Form.Where(rs => rs.Key.StartsWith("txtComment")).ToArray();
                    foreach (var comment in commentInputs)
                    {
                        allComments.Add(int.Parse(comment.Key.Substring(10)), comment.Value);

                    }
                    UpdatePatientConditions(selectedOptions, allComments, patient);
                }
                if (ModelState.IsValid)
                {
                    if (thePicture != null)
                    {
                        string mimeType = thePicture.ContentType;
                        long fileLength = thePicture.Length;
                        if (!(mimeType == "" || fileLength == 0))//Looks like we have a file!!!
                        {
                            if (mimeType.Contains("image"))
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    await thePicture.CopyToAsync(memoryStream);
                                    patient.imageContent = memoryStream.ToArray();
                                }
                                patient.imageMimeType = mimeType;
                                patient.imageFileName = thePicture.FileName;
                            }
                        }
                    }
                    _context.Add(patient);
                    await _context.SaveChangesAsync();
                    //return RedirectToAction(nameof(Index));
                    //Send on to add appointments
                    return RedirectToAction("Index", "PatientAppt", new { PatientID = patient.ID });
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException dex)
            {
                if (dex.InnerException.Message.Contains("IX_Patients_OHIP"))
                {
                    ModelState.AddModelError("OHIP", "Unable to save changes. Remember, you cannot have duplicate OHIP numbers.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unnknown error!");
            }
            
            PopulateAssignedConditionData(patient);
            PopulateDropDownLists(patient);
            return View(patient);
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.Doctor)
                .Include(p => p.MedicalTrial)
                .Include(p => p.PatientConditions).ThenInclude(p => p.Condition)
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.ID == id);
            if (patient == null)
            {
                return NotFound();
            }
            PopulateAssignedConditionData(patient);
            PopulateDropDownLists(patient);
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string[] selectedOptions, Byte[] RowVersion, string chkRemoveImage, IFormFile thePicture)
        {
            //Go get the patient to update
            var patientToUpdate = await _context.Patients
                .Include(p => p.Doctor)
                .Include(p => p.MedicalTrial)
                .Include(p => p.PatientConditions)
                .ThenInclude(p => p.Condition)
                .SingleOrDefaultAsync(p => p.ID == id);
            //Check that you got it or exit with a not found error
            if (patientToUpdate == null)
            {
                return NotFound();
            }


            //Check if any medical history and Update as required
            if (selectedOptions == null)
            {
                //No medical history
                patientToUpdate.PatientConditions = new List<PatientCondition>();
            }
            else
            {
                //Build a dictionary of selected options with comments
                var allComments = new Dictionary<int, string>();
                var commentInputs = Request.Form.Where(rs => rs.Key.StartsWith("txtComment")).ToArray();
                foreach (var comment in commentInputs)
                {
                    allComments.Add(int.Parse(comment.Key.Substring(10)), comment.Value);

                }
                UpdatePatientConditions(selectedOptions, allComments, patientToUpdate);
            }

            //Try updating it with the values posted
            if (await TryUpdateModelAsync<Patient>(patientToUpdate, "",
                p => p.OHIP, p => p.FirstName, p => p.MiddleName, p => p.LastName, p => p.DOB,
                p => p.ExpYrVisits, p => p.MedicalTrialID, p => p.Phone, p => p.eMail, p => p.DoctorID))
            {
                try
                {
                    //For the image
                    if (chkRemoveImage != null)
                    {
                        patientToUpdate.imageContent = null;
                        patientToUpdate.imageMimeType = null;
                        patientToUpdate.imageFileName = null;
                    }
                    else
                    {
                        if (thePicture != null)
                        {
                            string mimeType = thePicture.ContentType;
                            long fileLength = thePicture.Length;
                            if (!(mimeType == "" || fileLength == 0))//Looks like we have a file!!!
                            {
                                if (mimeType.Contains("image"))
                                {
                                    using (var memoryStream = new MemoryStream())
                                    {
                                        await thePicture.CopyToAsync(memoryStream);
                                        patientToUpdate.imageContent = memoryStream.ToArray();
                                    }
                                    patientToUpdate.imageMimeType = mimeType;
                                    patientToUpdate.imageFileName = thePicture.FileName;
                                }
                            }
                        }
                    }

                    //Put the original RowVersion value in the OriginalValues collection for the entity
                    _context.Entry(patientToUpdate).Property("RowVersion").OriginalValue = RowVersion;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
                catch (DbUpdateConcurrencyException ex)// Added for concurrency
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Patient)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError("",
                            "Unable to save changes. The Patient was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Patient)databaseEntry.ToObject();
                        if (databaseValues.FirstName != clientValues.FirstName)
                            ModelState.AddModelError("FirstName", "Current value: "
                                + databaseValues.FirstName);
                        if (databaseValues.MiddleName != clientValues.MiddleName)
                            ModelState.AddModelError("MiddleName", "Current value: "
                                + databaseValues.MiddleName);
                        if (databaseValues.LastName != clientValues.LastName)
                            ModelState.AddModelError("LastName", "Current value: "
                                + databaseValues.LastName);
                        if (databaseValues.OHIP != clientValues.OHIP)
                            ModelState.AddModelError("OHIP", "Current value: "
                                + databaseValues.OHIP);
                        if (databaseValues.DOB != clientValues.DOB)
                            ModelState.AddModelError("DOB", "Current value: "
                                + String.Format("{0:d}", databaseValues.DOB));
                        if (databaseValues.Phone != clientValues.Phone)
                            ModelState.AddModelError("Phone", "Current value: "
                                + String.Format("{0:(###) ###-####}", databaseValues.Phone));
                        if (databaseValues.eMail != clientValues.eMail)
                            ModelState.AddModelError("eMail", "Current value: "
                                + databaseValues.eMail);
                        if (databaseValues.ExpYrVisits != clientValues.ExpYrVisits)
                            ModelState.AddModelError("ExpYrVisits", "Current value: "
                                + databaseValues.ExpYrVisits);
                        //For the foreign key, we need to go to the database to get the information to show
                        if (databaseValues.DoctorID != clientValues.DoctorID)
                        {
                            Doctor databaseDoctor = await _context.Doctors.SingleOrDefaultAsync(i => i.ID == databaseValues.DoctorID);
                            ModelState.AddModelError("DoctorID", $"Current value: {databaseDoctor?.FullName}");
                        }
                        //A little extra work for the nullable foreign key.  No sense going to the database and asking for something
                        //we already know is not there.
                        if (databaseValues.MedicalTrialID != clientValues.MedicalTrialID)
                        {
                            if (databaseValues.MedicalTrialID.HasValue)
                            {
                                MedicalTrial databaseMedicalTrial = await _context.MedicalTrials.SingleOrDefaultAsync(i => i.ID == databaseValues.MedicalTrialID);
                                ModelState.AddModelError("MedicalTrialID", $"Current value: {databaseMedicalTrial?.TrialName}");
                            }
                            else
                            {
                                ModelState.AddModelError("MedicalTrialID", $"Current value: None");
                            }
                        }
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                + "was modified by another user after you received your values. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. If you still want to save your version of this record, click "
                                + "the Save button again. Otherwise click the 'Back to List' hyperlink.");
                        patientToUpdate.RowVersion = (byte[])databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
                catch (DbUpdateException dex)
                {
                    if (dex.InnerException.Message.Contains("IX_Patients_OHIP"))
                    {
                        ModelState.AddModelError("OHIP", "Unable to save changes. Remember, you cannot have duplicate OHIP numbers.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    }
                }
            }
            //Validaiton Error so give the user another chance.
            PopulateAssignedConditionData(patientToUpdate);
            PopulateDropDownLists(patientToUpdate);
            return View(patientToUpdate);
        }

        // GET: Patients/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.Doctor)
                .Include(p => p.MedicalTrial)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients
                .Include(p => p.Doctor)
                .Include(p => p.MedicalTrial)
                .FirstOrDefaultAsync(m => m.ID == id);
            try
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                //Note: there is really no reason a delete should fail if you can "talk" to the database.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(patient);
        }

        private void PopulateAssignedConditionData(Patient patient)
        {
            var allConditions = _context.Conditions;
            var pConditions = new HashSet<int>(patient.PatientConditions.Select(b => b.ConditionID));
            var checkBoxes = new List<AssignedOptionVM>();
            foreach (var condition in allConditions)
            {
                if (pConditions.Contains(condition.ID))
                {
                    //Condition in history so we need to get the comment
                    checkBoxes.Add(new AssignedOptionVM
                    {
                        ID = condition.ID,
                        DisplayText = condition.ConditionName,
                        Assigned = true,
                        Comment = (patient.PatientConditions
                            .SingleOrDefault(pc => pc.ConditionID == condition.ID)).Comment
                    });
                }
                else
                {
                    checkBoxes.Add(new AssignedOptionVM
                    {
                        ID = condition.ID,
                        DisplayText = condition.ConditionName,
                        Assigned = false
                    });
                }
            }
            ViewData["Conditions"] = checkBoxes;
        }
        private void UpdatePatientConditions(string[] selectedOptions, Dictionary<int, string> allComments, Patient patientToUpdate)
        {
            var selectedConditionsHS = new HashSet<string>(selectedOptions);
            var patientConditionsHS = new HashSet<int>
                (patientToUpdate.PatientConditions.Select(c => c.ConditionID));//IDs of the currently selected conditions
            foreach (var condition in _context.Conditions)
            {
                if (selectedConditionsHS.Contains(condition.ID.ToString()))
                {
                    if (!patientConditionsHS.Contains(condition.ID))
                    {
                        patientToUpdate.PatientConditions.Add(new
                            PatientCondition
                        {
                            PatientID = patientToUpdate.ID,
                            ConditionID = condition.ID,
                            Comment = allComments[condition.ID]
                        });
                    }
                    else //Still checked but comment might have changed
                    {
                        PatientCondition conditionToUpdate = patientToUpdate.PatientConditions.SingleOrDefault(c => c.ConditionID == condition.ID);
                        conditionToUpdate.Comment = allComments[condition.ID];
                    }
                }
                else
                {
                    if (patientConditionsHS.Contains(condition.ID))
                    {
                        PatientCondition conditionToRemove = patientToUpdate.PatientConditions.SingleOrDefault(c => c.ConditionID == condition.ID);
                        _context.Remove(conditionToRemove);
                    }
                }
            }
        }

        //This is a twist on the PopulateDropDownLists approach
        //  Create methods that return each SelectList separately
        //  and one method to put them all into ViewData.
        //This approach allows for AJAX requests to refresh
        //DDL Data at a later date.
        private SelectList DoctorSelectList(int? selectedId)
        {
            return new SelectList(_context.Doctors
                .OrderBy(d => d.LastName)
                .ThenBy(d => d.FirstName), "ID", "FormalName", selectedId);
        }
        [HttpGet]
        public JsonResult GetDoctors(int? id)
        {
            return Json(DoctorSelectList(id));
        }
        private void PopulateDropDownLists(Patient patient = null)
        {
            ViewData["DoctorID"] = DoctorSelectList(patient?.DoctorID);
            ViewData["MedicalTrialID"] = new SelectList(_context
                .MedicalTrials
                .OrderBy(m=>m.TrialName), "ID", "TrialName", patient?.MedicalTrialID);
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.ID == id);
        }
    }
}
