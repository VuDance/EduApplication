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
            button3.Click += (s, e) => LoadContent(new Controls.AttendanceControl());
            button5.Click += (s, e) => LoadContent(new Controls.Subject.SubjectControl());
            button6.Click += (s, e) => LoadContent(new Controls.Teacher.TeacherControl());
        }
        public void LoadContent(UserControl control)
        {
            panelContent.Controls.Clear();
            control.Dock = DockStyle.Fill;
            panelContent.Controls.Add(control);
        }
    }
}
