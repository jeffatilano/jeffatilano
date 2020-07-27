using JIL_Attendance.Helpers;
using JIL_Attendance.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JIL_Attendance.Controllers
{
    public class HomeController : Controller
    {
        private DefaultConnection db = new DefaultConnection();
        public ActionResult Index()
        {
            return View();
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public PartialViewResult Event()
        {
            var list = db.Admin_Event.Where(x=> x.Status == "Open").ToList();
            return PartialView(list);
        }

        public async Task<PartialViewResult> Churches(string id)
        {
            var getvancouver = await db.main_Church.Where(x=> x.Church == "Vancouver").SingleOrDefaultAsync();
            var getsurrey = await db.main_Church.Where(x => x.Church == "Surrey").SingleOrDefaultAsync();
            var getnorthvan = await db.main_Church.Where(x => x.Church == "North Vancouver").SingleOrDefaultAsync();
            var getBurnaby = await db.main_Church.Where(x => x.Church == "Burnaby-New Westminster").SingleOrDefaultAsync();
            var getRichmond = await db.main_Church.Where(x => x.Church == "Richmond").SingleOrDefaultAsync();
            var getLangley = await db.main_Church.Where(x => x.Church == "Langley").SingleOrDefaultAsync();

            ViewBag.Vancouver = UrlEncryptionHelper.EncryptId(getvancouver.ChurchID);
            ViewBag.Surrey = UrlEncryptionHelper.EncryptId(getsurrey.ChurchID);
            ViewBag.NorthVancouver = UrlEncryptionHelper.EncryptId(getnorthvan.ChurchID);
            ViewBag.Burnaby = UrlEncryptionHelper.EncryptId(getBurnaby.ChurchID);
            ViewBag.Richmond = UrlEncryptionHelper.EncryptId(getRichmond.ChurchID);
            ViewBag.Langley = UrlEncryptionHelper.EncryptId(getLangley.ChurchID);

            return PartialView();
        }
        public JsonResult VerifyEvent(string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Json(new { verified = true, eventID = id},JsonRequestBehavior.AllowGet);
            }

            return Json(new { notverified = true },JsonRequestBehavior.AllowGet);
        }

        public ActionResult SeatingArrangement()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}