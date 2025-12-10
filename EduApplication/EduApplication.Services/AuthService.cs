using EduApplication.EduApplication.Core.Entities;
using EduApplication.EduApplication.Data;
using Microsoft.EntityFrameworkCore;

namespace EduApplication.EduApplication.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null || !user.IsActive) return false;

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
        public async Task<Teacher> GetTeacherAsync(int userId)
        {
            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.UserId == userId);
            if (teacher == null)
            {
                throw new Exception("Teacher not found for the given user ID");
            }
            return teacher;
        }
        public async Task<Student> GetStudentAsync(int userId)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null)
            {
                throw new Exception("Student not found for the given user ID");
            }
            return student;
        }
        public async Task<User> CreateUserAsync(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            {
                throw new Exception("Username already exists");
            }
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<bool> ChangePassword(int id, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                throw new Exception("User not exist");
            }
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
