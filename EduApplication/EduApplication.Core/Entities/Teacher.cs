using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EduApplication.EduApplication.Winforms.Shared.Enums;

namespace EduApplication.EduApplication.Core.Entities
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }
        public required string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required Specialty Specialty { get; set; }
        public DateTime HireDate { get; set; }
        public int UserId { get; set; }
        public virtual required User User { get; set; }
        public virtual ICollection<Class> Classes { get; set; }
    }
}
