using JIL_Attendance.Helpers;
using JIL_Attendance.Models;
using JIL_Attendance.ViewModel;
using Postal;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JIL_Attendance.Controllers
{
    [Authorize]
    public class SeatingArrangementController : Controller
    {
        private DefaultConnection db = new DefaultConnection();
        // GET: SeatingArrangement
        public async Task<ActionResult> Index()
        {
            ViewData["Church"] = await DdlHelpers.ChurchList();

            return View();
        }

        public PartialViewResult Vancouver()
        {
            return PartialView();
        }

        public PartialViewResult Surrey()
        {
            return PartialView();
        }

        public PartialViewResult NorthVancouver()
        {

            return PartialView();
        }

        public PartialViewResult Burnaby()
        { 
        return PartialView();
        }

        public async Task<JsonResult> getData(int ChurchID, string EventID)
        {
            //var guidChurch = UrlEncryptionHelper.DecryptId(ChurchID);
            var guidEvent = UrlEncryptionHelper.DecryptId(EventID);

            var getData = await (from x in db.SeatReservations
                                 join y in db.Admin_Event on x.EventID equals y.EventID
                                 where x.ChurchID == ChurchID && y.EventID == guidEvent
                                 select new Seating
                                 {
                                     Fullname = x.Remarks + " " + x.Fullname,
                                     SeatNo = x.SeatNo
                                 }).ToArrayAsync();

            return Json(new { List = getData }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult Create(int ChurchID, int SeatNo, string EventID)
        {
            ViewData["ChurchIDEncrypt"] = UrlEncryptionHelper.EncryptId(ChurchID);
            ViewData["SeatEncrypt"] = UrlEncryptionHelper.EncryptId(SeatNo);
            ViewData["EventEncrypt"] = EventID;
            return PartialView();
        }


        [HttpPost]
        [PreventDuplicateRequest]
        public async Task<JsonResult> Create(SeatReservations model, string encryptchurch, string encryptseatNo, string encryptEvent)
        {
            var Church = UrlEncryptionHelper.DecryptId(encryptchurch);
            var SeatID = UrlEncryptionHelper.DecryptId(encryptseatNo);
            var eventguid = UrlEncryptionHelper.DecryptId(encryptEvent);

            var filter = await db.SeatReservations.Where(x => x.Fullname == model.Fullname && x.EventID == eventguid && x.ChurchID == Church).ToListAsync();
            var getEvent = await db.Admin_Event.Where(x => x.EventID == eventguid).SingleOrDefaultAsync();
            var getChurch = await db.main_Church.Where(x => x.ChurchID == Church).SingleOrDefaultAsync();
            var verifytoEdit = await db.SeatReservations.Where(x => x.SeatNo == SeatID && x.ChurchID == Church && x.EventID == eventguid).SingleOrDefaultAsync();
            if (verifytoEdit == null)
            {
                if (filter.Count() == 0)
                {
                    model.ChurchID = Church;
                    model.Status = "Active";
                    model.SeatNo = SeatID;
                    model.EventID = eventguid;
                    model.DateSchedule = getEvent.EventDate;
                    model.CreatedBy = User.Identity.Name;

                    db.SeatReservations.Add(model);
                    await db.SaveChangesAsync();

                    return Json(new { Save = true }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { AlreadyExists = true }, JsonRequestBehavior.AllowGet);
            }

          
          
            SeatReservations editmodel = await db.SeatReservations.Where(x => x.RecordID == verifytoEdit.RecordID).FirstAsync();
            editmodel.Fullname = model.Fullname;
            editmodel.Network = model.Network;    

            //dynamic email = new Email("Registration");
            //email.Subject = "JIL " + getChurch.Church + " Pre-Registration Update";
            //email.to = editmodel.CreatedBy;
            //email.Location = getChurch.CompleteAddress;
            //email.SeatNo = model.SeatNo;
            //email.Date = getEvent.EventDate.Value.DayOfWeek + ", " + getEvent.EventDate.Value.ToString("MMMM dd, yyyy");
            //email.Time = getEvent.EventStart + " - " + getEvent.EventEnd;
            //email.Event = getEvent.EventName;
            //email.Name = model.Fullname;
            //email.Note = "We have updated your seat.";
            //await email.SendAsync();

            db.Entry(editmodel).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Json(new { EditSuccess = true });
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
            if (model.FMember != null)
            {
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
            var list = await db.Attend_FamilyMembers.Where(x => x.RecordID == guid).ToListAsync();
            return PartialView(list);
        }

        [HttpGet]
        public PartialViewResult FamCreate(int ChurchID, int SeatNo, string EventID)
        {
            ViewData["ChurchIDEncrypt"] = UrlEncryptionHelper.EncryptId(ChurchID);
            ViewData["SeatEncrypt"] = UrlEncryptionHelper.EncryptId(SeatNo);
            ViewData["EventEncrypt"] = EventID;
            return PartialView();
        }

        public async Task<JsonResult> OpenSeat(int encryptchurch, int encryptseatNo, string encryptEvent)
        {
            var eventId = UrlEncryptionHelper.DecryptId(encryptEvent);
            var getData = await db.SeatReservations.Where(x=> x.ChurchID == encryptchurch && x.SeatNo == encryptseatNo && x.EventID == eventId).SingleOrDefaultAsync();
            db.SeatReservations.Remove(getData);
            await db.SaveChangesAsync();

            return Json(new { save = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [PreventDuplicateRequest]
        public async Task<JsonResult> FamCreate(SeatReservations model, string encryptchurch, string encryptseatNo, string encryptEvent)
        {
            var churchguid = UrlEncryptionHelper.DecryptId(encryptchurch);
            var SeatID = UrlEncryptionHelper.DecryptId(encryptseatNo);
            var eventguid = UrlEncryptionHelper.DecryptId(encryptEvent);
            var filter = await db.SeatReservations.Where(x => x.Fullname == model.Fullname && x.EventID == eventguid && x.ChurchID == churchguid).ToListAsync();
            var getEvent = await db.Admin_Event.Where(x => x.EventID == eventguid).SingleOrDefaultAsync();
            var getchurch = await db.main_Church.Where(x => x.ChurchID == churchguid).SingleOrDefaultAsync();
            var verifytoEdit = await db.SeatReservations.Where(x => x.SeatNo == SeatID && x.ChurchID == churchguid && x.EventID == eventguid).SingleOrDefaultAsync();
            if (verifytoEdit == null)
            {
                if (filter.Count() == 0)
                {
                    model.Fullname = model.Fullname;
                    model.ChurchID = churchguid;
                    model.Status = "Active";
                    model.SeatNo = SeatID;
                    model.EventID = eventguid;
                    model.DateSchedule = getEvent.EventDate;
                    model.CreatedBy = User.Identity.Name;

                    db.SeatReservations.Add(model);
                    await db.SaveChangesAsync();

                    var getRecord = UrlEncryptionHelper.EncryptId(model.RecordID);
                    return Json(new { Save = true, id = getRecord }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { AlreadyExists = true }, JsonRequestBehavior.AllowGet);
                //}
            }

            SeatReservations editmodel = await db.SeatReservations.Where(x => x.RecordID == verifytoEdit.RecordID).FirstAsync();
            editmodel.Fullname = model.Fullname;
            editmodel.Network = model.Network;

            //dynamic email = new Email("Registration");
            //email.Subject = "JIL " + getChurch.Church + " Pre-Registration Update";
            //email.to = editmodel.CreatedBy;
            //email.Location = getChurch.CompleteAddress;
            //email.SeatNo = model.SeatNo;
            //email.Date = getEvent.EventDate.Value.DayOfWeek + ", " + getEvent.EventDate.Value.ToString("MMMM dd, yyyy");
            //email.Time = getEvent.EventStart + " - " + getEvent.EventEnd;
            //email.Event = getEvent.EventName;
            //email.Name = model.Fullname;
            //email.Note = "We have updated your seat.";
            //await email.SendAsync();

            db.Entry(editmodel).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Json(new { EditSuccess = true });
        }


        public async Task<JsonResult> Reserve(int encryptchurch, int encryptseatNo, string encryptEvent)
        {
            //var Church = UrlEncryptionHelper.DecryptId(encryptchurch);
            //var SeatID = UrlEncryptionHelper.DecryptId(encryptseatNo);
            var eventguid = UrlEncryptionHelper.DecryptId(encryptEvent);
            var existing = await db.SeatReservations.Where(x=> x.ChurchID == encryptchurch && x.SeatNo == encryptseatNo && x.EventID == eventguid).SingleOrDefaultAsync();
            if (existing != null)
            {
                db.SeatReservations.Remove(existing);
                await db.SaveChangesAsync();           
            }
            SeatReservations model = new SeatReservations();
            model.ChurchID = encryptchurch;
            model.SeatNo = encryptseatNo;
            model.EventID = eventguid;
            model.Fullname = "Reserved";
            model.Status = "Active";

            db.SeatReservations.Add(model);
            await db.SaveChangesAsync();

            return Json(new {save = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult>Edit(int ChurchID, string EventID, int SeatNo)
        {
            var eventguid = UrlEncryptionHelper.DecryptId(EventID);
            var getdata = await db.SeatReservations.Where(x => x.ChurchID == ChurchID && x.EventID == eventguid && x.SeatNo == SeatNo).SingleOrDefaultAsync();

            if (getdata != null)
            {
                var famData = await db.Attend_FamilyMembers.Where(x => x.RecordID == getdata.RecordID).ToListAsync();
                if (famData.Count != 0)
                {
                    var FamilyList = await (from x in db.SeatReservations
                                            join y in db.Attend_FamilyMembers on x.RecordID equals y.RecordID
                                            where x.RecordID == getdata.RecordID
                                            select new EditViewforSeats
                                            {
                                                Fullname = y.FMember
                                            }).ToArrayAsync();
                    return Json(new { List = FamilyList, Name = getdata.Fullname, withFam = true, seatNo = SeatNo }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Name = getdata.Fullname, NameOnly = true, seatNo = SeatNo }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { noData = true, seatNo = SeatNo }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetTimeSelectedByEvent(string id)
        {
            var guid = UrlEncryptionHelper.DecryptId(id);
            var getTime = await db.Admin_Event.FindAsync(guid);
            var Time = getTime.EventDate.Value.DayOfWeek + ", " + getTime.EventDate.Value.ToString("MMMM dd, yyyy") + " " + getTime.EventStart + " - " + getTime.EventEnd + " PDT";
            return Json(new { thisTime = Time }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetEventbySelectedChurchID(int id)
        {
            return Json(await DdlHelpers.getEvent(id));
        }
    }

    
}