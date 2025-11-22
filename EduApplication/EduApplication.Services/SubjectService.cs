using EduApplication.EduApplication.Core.Entities;
using EduApplication.EduApplication.Data;
using EduApplication.EduApplication.Winforms.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EduApplication.EduApplication.Services
{
    public class SubjectService
    {
        private readonly AppDbContext _context;
        public SubjectService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Subject> CreateSubjectAsync(SubjectDto subject)
        {
            var subjectEntity = new Subject
            {
                Code = subject.Code,
                Name = subject.Name,
                Specialty = subject.Specialty,
                Description = subject.Description
            };
            _context.Subjects.Add(subjectEntity);
            await _context.SaveChangesAsync();
            return subjectEntity;
        }
        public async Task<List<Subject>> GetAllSubjectsAsync()
        {
            return await _context.Subjects
                .ToListAsync();
        }
        public async Task<Subject> UpdateSubjectAsync(SubjectDto subjectDto)
        {
            var subject = await _context.Subjects.FindAsync(subjectDto.Id);
            if (subject == null)
                throw new Exception("Subject not found");
            subject.Code = subjectDto.Code;
            subject.Name = subjectDto.Name;
            subject.Specialty = subjectDto.Specialty;
            subject.Description = subjectDto.Description;
            _context.Subjects.Update(subject);
            await _context.SaveChangesAsync();
            return subject;
        }
        public async Task<Subject> GetSubjectByIdAsync(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
                throw new Exception("Subject not found");
            return subject;
        }
        public async Task DeleteSubjectAsync(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
                throw new Exception("Subject not found");
            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
        }
    }
}
