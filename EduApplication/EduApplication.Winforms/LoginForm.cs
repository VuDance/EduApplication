using EduApplication.EduApplication.Core.Entities;
using EduApplication.EduApplication.Data;
using EduApplication.EduApplication.Services;
using EduApplication.EduApplication.Winforms.Dtos;
using EduApplication.EduApplication.Winforms.Report;
using EduApplication.EduApplication.Winforms.Shared;
using Microsoft.EntityFrameworkCore;

namespace EduApplication.EduApplication.Winforms
{
    public partial class LoginForm : Form
    {
        private readonly AuthService _authService;
        public LoginForm()
        {
            InitializeComponent();
            string connString = Properties.Settings.Default.DefaultConnection;

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connString)
                .Options;
            var context = new AppDbContext(options);
            _authService = new AuthService(context);
            txtPassword.PasswordChar = '*'; // Ẩn password
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (await _authService.LoginAsync(username, password))
            {
                var user = await _authService.GetUserByUsernameAsync(username);
                if (!user.IsActive)
                {
                    MessageBox.Show("Tài khoản đã bị vô hiệu hóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                AppSession.CurrentUser = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Role = user.Role,
                    IsActive = user.IsActive
                };
                this.Hide();
                var main = new MainForm();
                main.FormClosed += (s, args) => this.Close();
                main.Show();
            }
            else
            {
                MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AttendenceForm f = new AttendenceForm();
            f.Show();          // mở không chặn
                               // f.ShowDialog(); // nếu muốn mở dạng modal
        }

    }
}
