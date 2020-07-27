using JIL_Attendance.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JIL_Attendance.Helpers
{
    public class DdlHelpers
    {
        public static async Task<List<SelectListItem>> ChurchList()
        {
            DefaultConnection db = new DefaultConnection();

            var list = await db.main_Church.ToListAsync();

            var eventlist = list.Select(x => new SelectListItem
            {
                Text = x.Church,
                Value = x.ChurchID.ToString()
            }).ToList();

            return eventlist;
        }

        public static List<SelectListItem> Network()
        {
            var church = new List<SelectListItem>();

            church.Add(new SelectListItem() { Text = "Children", Value = "1" });
            church.Add(new SelectListItem() { Text = "Vancouver", Value = "1" });

            return church.ToList();
        }

        public static List<SelectListItem> Status()
        {
            var church = new List<SelectListItem>();

            church.Add(new SelectListItem() { Text = "Open", Value = "Open" });
            church.Add(new SelectListItem() { Text = "Close", Value = "Close" });

            return church.ToList();
        }

        public static async Task<List<SelectListItem>> getEvent(int id)
        {
            DefaultConnection db = new DefaultConnection();

            var list = await db.Admin_Event.Where(x =>  x.ChurchID == id && x.Status == "Open").ToListAsync();

            var eventlist = list.Select(x => new SelectListItem
            {
                Text = x.EventName,
                Value = UrlEncryptionHelper.EncryptId(x.EventID)
            }).ToList();

            return eventlist;
        }

        public static async Task<List<SelectListItem>> getChurchEvent(string id)
        {
            DefaultConnection db = new DefaultConnection();
            var guid = UrlEncryptionHelper.DecryptId(id);
            var list = await db.Admin_Event.Where(x => x.ChurchID == guid && x.Status == "Open").ToListAsync();

            var churcheventList = list.Select(x => new SelectListItem
            {
                Text = x.EventName,
                Value = UrlEncryptionHelper.EncryptId(x.EventID)
            }).ToList();

            return churcheventList;
        }
    }
}