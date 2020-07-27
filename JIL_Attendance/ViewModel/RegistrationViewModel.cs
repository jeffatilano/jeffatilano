using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JIL_Attendance.ViewModel
{
    public class RegistrationViewModel
    {
    }

    public class Seating
    { 
    public string Fullname { get; set; }
    public int SeatNo { get; set; }
    }

    public class EventDetailsViewModel
    { 
    public string DateAndTime { get; set; }
   public string Location { get; set; }
    public string Event { get; set; }
     
    }

    public class ChurchEventViewModel
    { 
        public string Event { get; set; }
        public string TimeAndDate { get; set; }


    }


    public class EditViewforSeats
    {
        public string Fullname { get; set; }
     
    }

}