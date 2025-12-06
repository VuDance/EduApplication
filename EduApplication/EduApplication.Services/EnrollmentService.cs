using EduApplication.EduApplication.Core.Entities;
using EduApplication.EduApplication.Data;
using Microsoft.EntityFrameworkCore;

namespace EduApplication.EduApplication.Services
{
    public class EnrollmentService
    {
        private readonly AppDbContext _context;
        public EnrollmentService(AppDbContext dbContext) 
        {
            _context = dbContext;
        }
        public async Task EnrollStudentToClassAsync(List<int> studentId, int classId)
        {
            foreach (var id in studentId)
            {
                var enrollment = new Enrollment
                {
                    StudentId = id,
                    ClassId = classId,
                    Status = Winforms.Shared.Enums.StudentStatus.Active
                };
                _context.Enrollments.Add(enrollment);
            }

            await _context.SaveChangesAsync();
        }
        public async Task<List<Enrollment>> GetEnrollmentsByClassIdAsync(int classId)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Where(e => e.ClassId == classId)
                .ToListAsync();
        }
    }
}
