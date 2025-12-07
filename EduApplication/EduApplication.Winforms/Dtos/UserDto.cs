using EduApplication.EduApplication.Winforms.Shared.Enums;

namespace EduApplication.EduApplication.Winforms.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public Role Role { get; set; } = Role.Student;
        public bool IsActive { get; set; } = true;
        public int OrderId { get; set; }
    }
}
