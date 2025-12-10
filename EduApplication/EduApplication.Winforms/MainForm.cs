using EduApplication.EduApplication.Winforms.Shared;
using EduApplication.EduApplication.Winforms.Shared.Enums;

namespace EduApplication.EduApplication.Winforms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.Text = "Hệ thống quản lý lớp học";
            button1.Click += (s, e) => LoadContent(new Controls.StudentsControl());
            button2.Click += (s, e) => LoadContent(new Controls.ClassControl());
            button5.Click += (s, e) => LoadContent(new Controls.Subject.SubjectControl());
            button6.Click += (s, e) => LoadContent(new Controls.Teacher.TeacherControl());
            button3.Text = AppSession.CurrentUser.Username;
        }
        public void LoadContent(UserControl control)
        {
            panelContent.Controls.Clear();
            control.Dock = DockStyle.Fill;
            panelContent.Controls.Add(control);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var curUser = AppSession.CurrentUser;
            if (curUser == null)
            {
                MessageBox.Show("Vui lòng đăng nhập lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            if (curUser.Role == Role.Student || curUser.Role == Role.Teacher)
            {
                button1.Visible = false;
                button5.Visible = false;
                button6.Visible = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Đổi mật khẩu", null, async (s, ev) => ChangePassword());
            contextMenu.Items.Add("Đăng xuất", null, (s, ev) => Logout());
            contextMenu.Show(button3, new Point(0, button3.Height));
        }
        private void ChangePassword()
        {
            this.Hide();
            var changePasswordForm = new ChangePasswordForm();
            changePasswordForm.FormClosed += (s, args) => this.Close();
            changePasswordForm.Show();
        }
        private void Logout()
        {
            this.Hide();
            var loginForm = new LoginForm();
            loginForm.FormClosed += (s, args) => this.Close();
            loginForm.Show();
        }
    }
}
