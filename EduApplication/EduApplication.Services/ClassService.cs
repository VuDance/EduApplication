using EduApplication.EduApplication.Data;
using EduApplication.EduApplication.Winforms.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EduApplication.EduApplication.Services
{
    public class ClassService
    {
        private readonly AppDbContext _context;
        public ClassService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Core.Entities.Class>> GetAllClassesAsync()
        {
            return await _context.Classes
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                .Include(x => x.Enrollments)
                .ToListAsync();
        }
        public async Task<Core.Entities.Class> GetClassByIdAsync(int id)
        {
            var classEntity = await _context.Classes
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                .FirstOrDefaultAsync(c => c.ClassId == id);

            if (classEntity == null)
                throw new Exception("Class not found");

            return classEntity;
        }
        public async Task<Core.Entities.Class> CreateClassAsync(ClassDto classDto)
        {
            var classEntity = new Core.Entities.Class
            {
                ClassName = classDto.ClassName,
                SubjectId = classDto.SubjectId,
                TeacherId = classDto.TeacherId,
                StartDate = classDto.StartDate,
                EndDate = classDto.EndDate,
                Schedule = classDto.Schedule,
                MaxStudent = classDto.MaxStudent
            };
            _context.Classes.Add(classEntity);
            await _context.SaveChangesAsync();
            return classEntity;
        }
        public async Task<Core.Entities.Class> UpdateClassAsync(ClassDto classDto)
        {
            var classEntity = await _context.Classes.FindAsync(classDto.Id);
            if (classEntity == null)
                throw new Exception("Class not found");
            classEntity.ClassName = classDto.ClassName;
            classEntity.SubjectId = classDto.SubjectId;
            classEntity.TeacherId = classDto.TeacherId;
            classEntity.StartDate = classDto.StartDate;
            classEntity.EndDate = classDto.EndDate;
            classEntity.Schedule = classDto.Schedule;
            classEntity.MaxStudent = classDto.MaxStudent;
            await _context.SaveChangesAsync();
            return classEntity;
        }
        public async Task DeleteClassAsync(int id)
        {
            var classEntity = await _context.Classes.FindAsync(id);
            if (classEntity == null)
                throw new Exception("Class not found");
            _context.Classes.Remove(classEntity);
            await _context.SaveChangesAsync();
        }
    }
}
