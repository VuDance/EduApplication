namespace EduApplication.EduApplication.Winforms.Dtos
{
    public class ClassDto
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Schedule { get; set; }
        public int? MaxStudent { get; set; }
    }
}
