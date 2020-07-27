using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace JIL_Attendance.Models
{
    public class DefaultConnection : DbContext
    {
        public DefaultConnection() : base("DefaultConnection")
        {
            // Get the ObjectContext related to this DbContext
            var objectContext = (this as IObjectContextAdapter).ObjectContext;

            // Sets the command timeout for all the commands
            objectContext.CommandTimeout = 120;
        }
        //PIS 
        public DbSet<cmms_PersonalData> cmms_PersonalData { get; set; }
        public DbSet<Admin_AttendanceReservations> Admin_AttendanceReservations { get; set; }
        public DbSet<SeatReservations> SeatReservations { get; set; }
        public DbSet<Admin_Event> Admin_Event { get; set; }
        public DbSet<main_Church> main_Church { get; set; }
        public DbSet<Attend_FamilyMember> Attend_FamilyMembers { get; set; }
    }
}