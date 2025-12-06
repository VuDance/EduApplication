using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EduApplication.EduApplication.Winforms.Shared.Enums;

namespace EduApplication.EduApplication.Core.Entities
{
    public class Enrollment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public StudentStatus Status { get; set; }
        public virtual Student Student { get; set; }
        public virtual Class Class { get; set; }
    }
}
