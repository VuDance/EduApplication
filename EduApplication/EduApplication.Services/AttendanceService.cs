using EduApplication.EduApplication.Data;
using EduApplication.EduApplication.Winforms.Dtos;
using EduApplication.EduApplication.Winforms.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace EduApplication.EduApplication.Services
{
    public class AttendanceService
    {
        private readonly AppDbContext _context;
        private readonly EnrollmentService _enrollmentService;
        public AttendanceService(AppDbContext dbContext) 
        {
            _context = dbContext;
            _enrollmentService = new EnrollmentService(dbContext);
        }
        public async Task GenerateAttendance(DateOnly dateTime, int classId)
        {
            var existingRecords = await _context.Attendances
                .Where(a => a.ClassId == classId && a.Date == dateTime)
                .ToListAsync();
            if (existingRecords.Any())
            {
                return;
            }
            var enrollments = await _enrollmentService.GetEnrollmentsByClassIdAsync(classId);
            foreach (var enrollment in enrollments)
            {
                var attendance = new Core.Entities.Attendance
                {
                    ClassId = classId,
                    StudentId = enrollment.StudentId,
                    Date = dateTime,
                    Status = Winforms.Shared.Enums.AttendanceStatus.Absent
                };
                _context.Attendances.Add(attendance);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<Core.Entities.Attendance>> GetAttendanceByClassAndDateAsync(int classId, DateOnly date)
        {
            return await _context.Attendances
                .Include(x=>x.Student)
                .Where(a => a.ClassId == classId && a.Date == date)
                .ToListAsync();
        }
        public async Task UpdateAttendanceAsync(List<AttendanceDto> attendances)
        {
            foreach (var attendance in attendances)
            {
                var existingAttendance = await _context.Attendances.FindAsync(attendance.Id);
                if (existingAttendance != null)
                {
                    existingAttendance.Status = attendance.Status;
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
