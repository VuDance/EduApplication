using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduApplication.EduApplication.Core.Entities
{
    public class Class
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClassId { get; set; }

        [Required]
        [StringLength(100)]
        public required string ClassName { get; set; }

        [ForeignKey(nameof(Subject))]
        public int SubjectId { get; set; }

        [ForeignKey(nameof(Teacher))]
        public int TeacherId { get; set; }

        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }

        [StringLength(100)]
        public string? Schedule { get; set; }
        public int? MaxStudent { get; set; }

        public virtual Subject Subject { get; set; }
        public virtual Teacher Teacher { get; set; }
    }
}
