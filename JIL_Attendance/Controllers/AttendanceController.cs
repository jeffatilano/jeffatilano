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
    public class AttendanceController : Controller
    {
        private DefaultConnection db = new DefaultConnection();
        // GET: Attendance
        public ActionResult Index()
        {
            return View();
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public PartialViewResult partialIndex()
        {
            var list = db.cmms_PersonalData.ToList();
            return PartialView(list);
        }

        [HttpPost]
        public PartialViewResult Create()
        { 

        return PartialView();
        }
    }
}