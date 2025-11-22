using EduApplication.EduApplication.Core.Entities;
using EduApplication.EduApplication.Data;
using EduApplication.EduApplication.Winforms.Dtos;
using EduApplication.EduApplication.Winforms.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace EduApplication.EduApplication.Services
{
    public class StudentService
    {
        private readonly AppDbContext _context;
        private readonly AuthService _authService;
        public StudentService(AppDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }
        public async Task<Student> CreateStudentAsync(StudentDto student)
        {
            var user = new User
            {
                Username = student.Email,
                PasswordHash = "123456",
                Role = Role.Student,
                IsActive = true
            };
            await _authService.CreateUserAsync(user);
            var studentEntity = new Student
            {
                FullName = student.FullName,
                Email = student.Email,
                Address = student.Address,
                PhoneNumber = student.PhoneNumber,
                DateOfBirth = student.DateOfBirth,
                Gender = student.Gender,
                RegisterDate = student.RegisterDate,
                UserId = user.Id,
                User = user
            };
            _context.Students.Add(studentEntity);
            await _context.SaveChangesAsync();
            return studentEntity;
        }
        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _context.Students
                .ToListAsync();
        }
        public async Task<Student> UpdateStudentAsync(StudentDto studentDto)
        {
            var student = await _context.Students.FindAsync(studentDto.Id);
            if (student == null)
                throw new Exception("Student not found");

            student.FullName = studentDto.FullName;
            student.Email = studentDto.Email;
            student.PhoneNumber = studentDto.PhoneNumber;
            student.Address = studentDto.Address;
            student.DateOfBirth = studentDto.DateOfBirth;
            student.Gender = studentDto.Gender;
            student.RegisterDate = studentDto.RegisterDate;

            await _context.SaveChangesAsync();
            return student;
        }
        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _context.Students.FindAsync(id);
        }
        public async Task DeleteStudentAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
                throw new Exception("Student not found");
            var user = await _context.Users.FindAsync(student.UserId);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }
    }
}
