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
    public class AdminController : Controller
    {
        private DefaultConnection db = new DefaultConnection();
        // GET: Admin
        public async Task<ActionResult> Index()
        {
            ViewData["Church"] = await DdlHelpers.ChurchList();
            return View();
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public PartialViewResult partialIndex(int id)
        {
            var list = db.Admin_Event.Where(x => x.ChurchID == id).OrderBy(x=> x.EventDate).ToList();
            //var list = db.Admin_Event.OrderBy(x=> x.EventDate).ToList();
            return PartialView(list);
        }

        [HttpGet]
        public PartialViewResult Create(int id)
        {
            var encryptID = UrlEncryptionHelper.EncryptId(id);
            ViewData["ChurchIDEncrypt"] = encryptID;
            return PartialView();
        }

        [HttpPost]
        public async Task<JsonResult> Create(Admin_Event model, string encryptchurch)
        {
            model.ChurchID = UrlEncryptionHelper.DecryptId(encryptchurch);
            db.Admin_Event.Add(model);
            await db.SaveChangesAsync();
           
            return Json(new { Save = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string id)
        {
            var guid = UrlEncryptionHelper.DecryptId(id);
            Admin_Event model = await db.Admin_Event.FindAsync(guid);
            ViewData["Status"] = DdlHelpers.Status();
            return PartialView(model);
        }

        [HttpPost]
        public async Task<JsonResult> Edit(Admin_Event model)
        {
            db.Entry(model).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Json(new {save = true },JsonRequestBehavior.AllowGet) ;
        }

    }
}