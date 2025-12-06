using EduApplication.EduApplication.Data;
using EduApplication.EduApplication.Services;
using Microsoft.EntityFrameworkCore;

namespace EduApplication.EduApplication.Winforms.Controls.Class
{
    public partial class PopupAddUser : Form
    {
        private readonly StudentService _studentService;
        private readonly int _classId;
        private readonly EnrollmentService _enrollmentService;
        private List<int> _selectedStudentIds = new List<int>();
        public PopupAddUser()
        {
            InitializeComponent();
            string connString = Properties.Settings.Default.DefaultConnection;
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connString)
                .Options;
            var context = new AppDbContext(options);
            _studentService = new StudentService(context, new AuthService(context));
            _enrollmentService = new EnrollmentService(context);
            this.Load += PopupAddUser_Load!;
        }

        public PopupAddUser(int id) : this()
        {
            _classId = id;
        }
        private async void PopupAddUser_Load(object? sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = true;
            var listStudents = await _studentService.SearchStudentsAsync("");
            dataGridView1.DataSource = listStudents.Select(s => new
            {
                Id = s.Id,
                s.FullName,
                s.Email,
                s.Gender
            }).ToList();
            DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
            chk.Name = "Select";
            chk.HeaderText = "Chọn";
            dataGridView1.Columns.Add(chk);

            dataGridView1.Columns["Id"].HeaderText = "Mã HS";
            dataGridView1.Columns["FullName"].HeaderText = "Họ tên";
            dataGridView1.Columns["Email"].HeaderText = "Email";
            dataGridView1.Columns["Gender"].HeaderText = "Giới tính";

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var listStudents = await _studentService.SearchStudentsAsync(textBox1.Text);
            dataGridView1.DataSource = listStudents.Select(s => new
            {
                Id = s.Id,
                s.FullName,
                s.Email,
                s.Gender
            }).ToList();
            DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
            chk.Name = "Select";
            chk.HeaderText = "Chọn";
            dataGridView1.Columns.Add(chk);

            dataGridView1.Columns["Id"].HeaderText = "Mã HS";
            dataGridView1.Columns["FullName"].HeaderText = "Họ tên";
            dataGridView1.Columns["Email"].HeaderText = "Email";
            dataGridView1.Columns["Gender"].HeaderText = "Giới tính";
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            _selectedStudentIds.Clear();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (Convert.ToBoolean(row.Cells["Select"].Value))
                {
                    int studentId = Convert.ToInt32(row.Cells["Id"].Value);
                    _selectedStudentIds.Add(studentId);
                }
            }

            await _enrollmentService.EnrollStudentToClassAsync(_selectedStudentIds, _classId);

            MessageBox.Show($"Đã chọn {_selectedStudentIds.Count} học viên", "Thông báo");
        }
    }
}
