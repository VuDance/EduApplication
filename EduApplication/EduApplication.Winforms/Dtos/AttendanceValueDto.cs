using EduApplication.EduApplication.Winforms.Shared.Enums;

namespace EduApplication.EduApplication.Winforms.Dtos
{
    public class AttendanceValueDto
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public string Date { get; set; }
        public bool Status { get; set; }
    }
}
