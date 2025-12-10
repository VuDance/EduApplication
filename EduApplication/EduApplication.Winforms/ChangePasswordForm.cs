using EduApplication.EduApplication.Data;
using EduApplication.EduApplication.Services;
using EduApplication.EduApplication.Winforms.Shared;
using Microsoft.EntityFrameworkCore;

namespace EduApplication.EduApplication.Winforms
{
    public partial class ChangePasswordForm : Form
    {
        private readonly AuthService _authService;
        public ChangePasswordForm()
        {
            InitializeComponent();
            string connString = Properties.Settings.Default.DefaultConnection;
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connString)
                .Options;
            var context = new AppDbContext(options);
            _authService = new AuthService(context);
            textBox1.PasswordChar = '*';
            textBox2.PasswordChar = '*';
            textBox3.PasswordChar = '*';
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var curUser = AppSession.CurrentUser;
            var user = await _authService.LoginAsync(curUser.Username, textBox1.Text);
            if (user == false)
            {
                MessageBox.Show("Người dùng không đúng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (textBox2.Text != textBox3.Text)
            {
                MessageBox.Show("Mật khẩu mới không khớp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var isSuccess = await _authService.ChangePassword(curUser.Id, textBox3.Text);
            if (isSuccess == false)
            {
                MessageBox.Show("Đổi mật khẩu không thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show("Đổi mật khẩu thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            var main = new LoginForm();
            main.Show();
            this.Hide();
            return;
        }

        private void ChangePasswordForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            var main = new MainForm();
            main.Show();
            this.Hide();
        }
    }
}
