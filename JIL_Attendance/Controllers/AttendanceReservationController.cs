using JIL_Attendance.Helpers;
using JIL_Attendance.Models;
using JIL_Attendance.ViewModel;
using Postal;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebGrease.Css.Visitor;

namespace JIL_Attendance.Controllers
{
 
    [Authorize]
    public class AttendanceReservationController : Controller
    {
        private DefaultConnection db = new DefaultConnection();
        // GET: AttendanceReservation
        public ActionResult Index()
        {
            ViewData["Status"] = GetStatus();
            return View();
        }

        public async Task<ActionResult> Vancouver(string id)
        {
            var guid = UrlEncryptionHelper.DecryptId(id);          
            var getchurch = await db.main_Church.FindAsync(guid);           
            ViewBag.ChurchId = getchurch.ChurchID;         
            ViewData["Event"] = await DdlHelpers.getChurchEvent(id);

            return View();
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public PartialViewResult PartialVancouver()
        {
            return PartialView();
        }

        public async Task<ActionResult> Surrey(string id)
        {
            var guid = UrlEncryptionHelper.DecryptId(id);
            var getchurch = await db.main_Church.FindAsync(guid);
         
            ViewBag.ChurchId = getchurch.ChurchID;
            ViewData["Event"] = await DdlHelpers.getChurchEvent(id);

            return View();
        }

        public PartialViewResult PartialSurrey()
        {
            return PartialView();
        }


        public async Task<ActionResult> NorthVancouver(string id)
        {
            var guid = UrlEncryptionHelper.DecryptId(id);          
            var chuchInfo = await db.main_Church.FindAsync(guid);           
            ViewBag.ChurchId = chuchInfo.ChurchID;
          
            ViewData["Event"] = await DdlHelpers.getChurchEvent(id);

            return View();
        }

        public PartialViewResult PartialNorthVancouver()
        {
            return PartialView();
        }

        [HttpGet]
        public async Task<JsonResult> CancelView(string ChurchID, string EventID)
        {
            var guidChurch = UrlEncryptionHelper.DecryptId(ChurchID);
            var guidEvent = UrlEncryptionHelper.DecryptId(EventID);
            var getData = await db.SeatReservations.Where(x => x.EventID == guidEvent && x.ChurchID == guidChurch && x.CreatedBy == User.Identity.Name).SingleOrDefaultAsync();

            return Json(new { Name = getData.Fullname, SeatNo = getData.SeatNo }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Burnaby(string id)
        {
            var guid = UrlEncryptionHelper.DecryptId(id);
            var chuchInfo = await db.main_Church.FindAsync(guid);         
            ViewBag.ChurchId = chuchInfo.ChurchID;
            ViewData["Event"] = await DdlHelpers.getChurchEvent(id);
            return View();
        }

        public PartialViewResult PartialBurnaby()
        {
            return PartialView();
        }

        public async Task<ActionResult> Richmond(string id)
        {
            var guid = UrlEncryptionHelper.DecryptId(id);
            ViewBag.ChurchId = id;
            ViewData["Event"] = await DdlHelpers.getChurchEvent(id);

            return View();
        }

        public PartialViewResult PartialRichmond()
        {
            return PartialView();
        }

        public async Task<ActionResult> Langley(string id)
        {
            var guid = UrlEncryptionHelper.DecryptId(id);
            var getEvent = await db.Admin_Event.Where(x => x.EventID == guid).SingleOrDefaultAsync();
            var getchurch = await db.main_Church.Where(x => x.Church == "Langley").SingleOrDefaultAsync();
            EventDetailsViewModel model = new EventDetailsViewModel();
            model.DateAndTime = getEvent.EventDate.Value.DayOfWeek + ", " + getEvent.EventDate.Value.ToString("MMMM dd, yyyy") + " " + getEvent.EventStart + " - " + getEvent.EventEnd + " PDT";
            model.Location = "Douglas Crescent Community School 5409 206 Ave, Cor. Douglas Crescent Road, Langley, BC";
            model.Event = getEvent.EventName;
            ViewBag.ChurchId = getchurch.ChurchID;
            ViewBag.EventID = id;

            return View(model);
        }

        public PartialViewResult PartialLangley()
        {
            return PartialView();
        }

        public async Task<JsonResult> getData(string ChurchID, string EventID)
        {
            var guidChurch = UrlEncryptionHelper.DecryptId(ChurchID);
            var guidEvent = UrlEncryptionHelper.DecryptId(EventID);
            var booleanResult = "";

            if (!User.IsInRole("Church")) {
                var checkData = await db.SeatReservations.Where(x => x.ChurchID == guidChurch && x.EventID == guidEvent && x.CreatedBy == User.Identity.Name).SingleOrDefaultAsync();
                if (checkData != null)
                {
                    booleanResult = "show";
                }
                else {
                    booleanResult = "hide";
                }
            }
               var getData = await (from x in db.SeatReservations
                                     join y in db.Admin_Event on x.EventID equals y.EventID
                                     where x.ChurchID == guidChurch && y.EventID == guidEvent
                                     select new Seating
                                     {
                                         Fullname = x.Remarks + " " + x.Fullname  ,
                                         SeatNo = x.SeatNo
                                     }).ToArrayAsync();

                return Json(new { List = getData, showBtn = booleanResult }, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult GuidelinesView()
        {
            return PartialView();
        }

        [HttpGet]
        public PartialViewResult FamilyMembers(string id)
        {
            //var guid = UrlEncryptionHelper.DecryptId(id);
            ViewData["encryptRecordID"] = id;
            return PartialView();
        }

        [HttpPost]
        public async Task<JsonResult> FamilyMembers(Attend_FamilyMember model, string encryptRecordID)
        {
            if (model.FMember != null) {
                var guid = UrlEncryptionHelper.DecryptId(encryptRecordID);
                var NameRestriction = await db.Attend_FamilyMembers.Where(x => x.RecordID == guid && x.FMember == model.FMember).ToListAsync();
                var MemberCOunt = await db.Attend_FamilyMembers.Where(x => x.RecordID == guid).ToListAsync();

                if (MemberCOunt.Count < 4)
                {
                    if (NameRestriction.Count == 0)
                    {
                        model.RecordID = guid;

                        db.Attend_FamilyMembers.Add(model);
                        await db.SaveChangesAsync();
                        return Json(new { Save = true }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { alreadyexist = true }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Exceed = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        public async Task<PartialViewResult> PartialFamilyMembers(string id)
        {
            var guid = UrlEncryptionHelper.DecryptId(id);
            var list = await db.Attend_FamilyMembers.Where(x=> x.RecordID == guid).ToListAsync();
            return PartialView(list);
        }

        [HttpGet]
        public PartialViewResult FamCreate(string ChurchID, int SeatNo, string EventID)
        {
            ViewData["ChurchIDEncrypt"] = ChurchID;
            ViewData["SeatEncrypt"] = UrlEncryptionHelper.EncryptId(SeatNo);
            ViewData["EventEncrypt"] = EventID;
            return PartialView();
        }


        [HttpPost]
        [PreventDuplicateRequest]
        public async Task<JsonResult> FamCreate(SeatReservations model, string encryptchurch, string encryptseatNo, string encryptEvent)
        {
            var churchguid = UrlEncryptionHelper.DecryptId(encryptchurch);
            var eventguid = UrlEncryptionHelper.DecryptId(encryptEvent);
            var filter = await db.SeatReservations.Where(x => x.Fullname == model.Fullname && x.EventID == eventguid && x.ChurchID == churchguid).ToListAsync();
            var getEvent = await db.Admin_Event.Where(x => x.EventID == eventguid).SingleOrDefaultAsync();
            var getchurch = await db.main_Church.Where(x => x.ChurchID == churchguid).SingleOrDefaultAsync();
            var addRestrictions = await db.SeatReservations.Where(x => x.ChurchID == churchguid && x.EventID == eventguid && x.CreatedBy == User.Identity.Name).ToListAsync();
            if (addRestrictions.Count == 0)
            {
                if (filter.Count() == 0)
                {
                    model.Fullname = model.Fullname;
                    model.ChurchID = churchguid;
                    model.Status = "Active";
                    model.SeatNo = UrlEncryptionHelper.DecryptId(encryptseatNo);
                    model.EventID = eventguid;
                    model.DateSchedule = getEvent.EventDate;
                    model.CreatedBy = User.Identity.Name;

                    db.SeatReservations.Add(model);
                    await db.SaveChangesAsync();

                var getRecord = UrlEncryptionHelper.EncryptId(model.RecordID);
                    return Json(new { Save = true, id = getRecord }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { AlreadyExists = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Exceedreserved = true });
        }

        [HttpGet]
        public PartialViewResult Create(string ChurchID,int SeatNo, string EventID)
        {
            ViewData["ChurchIDEncrypt"] = ChurchID;
            ViewData["SeatEncrypt"] = UrlEncryptionHelper.EncryptId(SeatNo);
            ViewData["EventEncrypt"] = EventID;
            //ViewData["Location"] = Location;
            return PartialView();
        }

        [HttpPost]
        [PreventDuplicateRequest]
        public async Task<JsonResult> Create(SeatReservations model, string encryptchurch, string encryptseatNo, string encryptEvent)
        {
            var churchguid = UrlEncryptionHelper.DecryptId(encryptchurch);
            var eventguid = UrlEncryptionHelper.DecryptId(encryptEvent);
            var filter = await db.SeatReservations.Where(x => x.Fullname == model.Fullname && x.EventID == eventguid && x.ChurchID == churchguid).ToListAsync();
            var getEvent = await db.Admin_Event.Where(x => x.EventID == eventguid).SingleOrDefaultAsync();
            var getchurch = await db.main_Church.Where(x => x.ChurchID == churchguid).SingleOrDefaultAsync();
        var addRestrictions = await db.SeatReservations.Where(x => x.ChurchID == churchguid && x.EventID == eventguid && x.CreatedBy == User.Identity.Name).ToListAsync();
        if (addRestrictions.Count == 0)
        {
        if (filter.Count() == 0)
            {
                model.ChurchID = churchguid;
                model.Status = "Active";
                model.SeatNo = UrlEncryptionHelper.DecryptId(encryptseatNo);
                model.EventID = eventguid;
                model.DateSchedule = getEvent.EventDate;
                model.CreatedBy = User.Identity.Name;

                db.SeatReservations.Add(model);
                await db.SaveChangesAsync();

                    //dynamic email = new Email("Registration");
                    //email.Subject = "JIL " + getchurch.Church+ " Pre-Registration";
                    //email.to = User.Identity.Name;
                    //email.Location = getchurch.CompleteAddress;
                    //email.SeatNo = model.SeatNo;
                    //email.Date = getEvent.EventDate.Value.DayOfWeek + ", " + getEvent.EventDate.Value.ToString("MMMM dd, yyyy");
                    //email.Time = getEvent.EventStart + " - " + getEvent.EventEnd;
                    //email.Event = getEvent.EventName;
                    //email.Name = model.Fullname;
                    //email.Note = "We've recieved your reservation for " + getEvent.EventName;
                    //await email.SendAsync();

                return Json(new { Save = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { AlreadyExists = true },JsonRequestBehavior.AllowGet);
        }
            return Json(new { Exceedreserved = true });
        }
        public async Task<string> getUniqueID()
        {
            var getData = await db.Admin_AttendanceReservations.Where(x => x.DateCreated.Value.Year == DateTime.Now.Year).ToListAsync();
            var countData = (getData.Count() + 1).ToString("D3");

            var newID = DateTime.Now.Year.ToString();
            var uniqueID = newID + "-" + countData;

            return uniqueID;
        }
        public async Task<JsonResult> GetTimeSelectedByEvent(string id)
        {
            var guid = UrlEncryptionHelper.DecryptId(id);
            var getTime = await db.Admin_Event.FindAsync(guid);
            var Time = getTime.EventDate.Value.DayOfWeek + ", " + getTime.EventDate.Value.ToString("MMMM dd, yyyy") + " " + getTime.EventStart + " - " + getTime.EventEnd + " PDT";
            return Json(new { thisTime = Time }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> CancelSeat(string encryptchurch, string encryptEvent)
        {
            var churchID = UrlEncryptionHelper.DecryptId(encryptchurch);
            var eventId = UrlEncryptionHelper.DecryptId(encryptEvent);
            var getData = await db.SeatReservations.Where(x => x.ChurchID == churchID && x.CreatedBy == User.Identity.Name && x.EventID == eventId).SingleOrDefaultAsync();
            db.SeatReservations.Remove(getData);
            await db.SaveChangesAsync();

            return Json(new { save = true }, JsonRequestBehavior.AllowGet);
        }

        public static List<SelectListItem> GetStatus()
        {
            var Status = new List<SelectListItem>();

            Status.Add(new SelectListItem() { Text = "Vancouver", Value = "1" });
            Status.Add(new SelectListItem() { Text = "Surrey", Value = "2" });
            Status.Add(new SelectListItem() { Text = "North Vancouver", Value = "3" });
            Status.Add(new SelectListItem() { Text = "Burnaby-New Westminster", Value = "4" });
            Status.Add(new SelectListItem() { Text = "Richmond", Value = "5" });
            Status.Add(new SelectListItem() { Text = "Langley", Value = "6" });
            return Status.ToList();
        }
    }
}
