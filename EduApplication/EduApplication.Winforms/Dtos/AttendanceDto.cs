using EduApplication.EduApplication.Winforms.Shared.Enums;

namespace EduApplication.EduApplication.Winforms.Dtos
{
    public class AttendanceDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public DateOnly Date { get; set; }
        public AttendanceStatus Status { get; set; }
    }
}
