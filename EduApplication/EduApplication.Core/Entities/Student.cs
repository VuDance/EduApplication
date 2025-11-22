using System.ComponentModel.DataAnnotations;
using EduApplication.EduApplication.Winforms.Shared.Enums;

namespace EduApplication.EduApplication.Core.Entities
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public DateTime RegisterDate { get; set; } = DateTime.Now;
        public int UserId { get; set; }
        public virtual required User User { get; set; }
    }
}
