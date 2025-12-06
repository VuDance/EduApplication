using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EduApplication.EduApplication.Winforms.Shared.Enums;

namespace EduApplication.EduApplication.Core.Entities
{
    public class Attendance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public DateOnly Date { get; set; }
        public AttendanceStatus Status { get; set; }
        public virtual Student Student { get; set; }
        public virtual Class Class { get; set; }
    }
}
