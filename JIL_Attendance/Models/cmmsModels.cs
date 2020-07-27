using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JIL_Attendance.Models
{
    [Table("cmms_PersonalData")]
    public class cmms_PersonalData
    {
        [Key]
        public int PISRecordID { get; set; }
        public int ChurchID { get; set; }

        //[Remote("doesIDExist", "PIS", HttpMethod = "POST", AdditionalFields = "InitialValue", ErrorMessage = "UnifiedID number already exists.")]
        public string UnifiedID { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Display(Name = "Nickname")]
        public string Nickame { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Complete Address (Current)")]
        public string CompleteAddress { get; set; }

        //[Remote("birthdayInvalid", "PIS", HttpMethod = "POST", AdditionalFields = "model", ErrorMessage = "Must be 4 years old and above.")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Birthday { get; set; }

        [Required]
        public string Gender { get; set; }

        [Display(Name = "Civil Status")]
        public string CivilStatus { get; set; }
        public string Remarks { get; set; }

        [Display(Name = "Residence Status")]
        public string ResidenceStatus { get; set; }
        public string Citizenship { get; set; }

        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Date Modified")]
        public DateTime DateModified { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }
    }

    public class Admin_AttendanceReservations
    { 
        [Key]
        public int RecordID { get; set; }
        [Display(Name = "Reference Number")]
        [Required]
        public string RefNo { get; set; }
        [Required]
        public string Name { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
     
        public DateTime? Schedule { get; set; }
        public string Status { get; set; }
        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }
        public DateTime? DateCreated { get; set; }
    }

    public class SeatReservations
    {
        [Key]
        public int RecordID { get; set; }
        public int ChurchID { get; set; }
        public int EventID { get; set; }
        [Required]
        public string Fullname { get; set; }
        public string Network { get; set; }
        public DateTime? DateSchedule { get; set; }
        public string Status { get; set; }
        public int SeatNo { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public virtual Admin_Event Admin_Event { get; set; }
        public virtual main_Church Main_Church { get; set; }
    }

    public class Admin_Event
    { 
        [Key]
        public int EventID { get; set; }
        [Display(Name = "Event Name")]
        public string EventName { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [Display(Name = "Date")]
        public DateTime? EventDate { get; set; }
        [Display(Name = "Time Start")]
        public string EventStart { get; set; }
        [Display(Name = "Time End")]
        public string EventEnd { get; set; }
        public string Status { get; set; }
        public int ChurchID { get; set; }
    }

    public class main_Church
    { 
    [Key]
        public int ChurchID { get; set; }
        public string Church { get; set; }
        public string ChurchPastor { get; set; }
        public string CompleteAddress { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class Attend_FamilyMember
    { 
    [Key]
        public int FID { get; set; }
        [Display(Name = "Family Member")]
        public string FMember { get; set; }
        public int RecordID { get; set; }

    }
}