using System.ComponentModel.DataAnnotations;
using EduApplication.EduApplication.Winforms.Shared.Enums;

namespace EduApplication.EduApplication.Core.Entities
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }
        public required string Code { get; set; }
        [Required]
        public required string Name { get; set; }
        public required Specialty Specialty { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<Class> Classes { get; set; }
    }
}
