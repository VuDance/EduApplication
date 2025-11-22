namespace EduApplication.EduApplication.Core.Entities
{
    public class ClassStudent
    {
        public int ClassId { get; set; }
        public required Class Class { get; set; }

        public int StudentId { get; set; }
        public required Student Student { get; set; }
    }
}
