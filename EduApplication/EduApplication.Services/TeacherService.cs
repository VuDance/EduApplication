using EduApplication.EduApplication.Core.Entities;
using EduApplication.EduApplication.Data;
using EduApplication.EduApplication.Winforms.Dtos;
using EduApplication.EduApplication.Winforms.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace EduApplication.EduApplication.Services
{
    public class TeacherService
    {
        private readonly AppDbContext _context;
        private readonly AuthService _authService;
        public TeacherService(AppDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }
        public async Task<Teacher> CreateTeacherAsync(TeacherDto teacher)
        {
            var user = new User
            {
                Username = teacher.Email,
                PasswordHash = "123456",
                Role = Role.Teacher,
                IsActive = true
            };
            await _authService.CreateUserAsync(user);
            var teacherEntity = new Teacher
            {
                FullName = teacher.FullName,
                Email = teacher.Email,
                PhoneNumber = teacher.PhoneNumber,
                DateOfBirth = teacher.DateOfBirth,
                HireDate = teacher.HireDate,
                Specialty = teacher.Specialty,
                UserId = user.Id,
                User = user
            };
            _context.Teachers.Add(teacherEntity);
            await _context.SaveChangesAsync();
            return teacherEntity;
        }
        public async Task<List<Teacher>> GetAllTeachersAsync()
        {
            return await _context.Teachers
                .ToListAsync();
        }
        public async Task<Teacher> UpdateTeacherAsync(TeacherDto teacherDto)
        {
            var teacher = await _context.Teachers.FindAsync(teacherDto.Id);
            if (teacher == null)
                throw new Exception("Teacher not found");

            teacher.FullName = teacherDto.FullName;
            teacher.Email = teacherDto.Email;
            teacher.PhoneNumber = teacherDto.PhoneNumber;
            teacher.DateOfBirth = teacherDto.DateOfBirth;
            teacher.HireDate = teacherDto.HireDate;
            teacher.Specialty = teacherDto.Specialty;

            await _context.SaveChangesAsync();
            return teacher;
        }
        public async Task<Teacher?> GetTeacherByIdAsync(int id)
        {
            return await _context.Teachers.FindAsync(id);
        }
        public async Task DeleteTeacherAsync(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
                throw new Exception("Teacher not found");
            var user = await _context.Users.FindAsync(teacher.UserId);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
        }
    }
}
