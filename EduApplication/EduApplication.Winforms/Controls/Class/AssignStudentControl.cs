using System.Threading.Tasks;
using EduApplication.EduApplication.Services;
using EduApplication.EduApplication.Winforms.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace EduApplication.EduApplication.Winforms.Controls.Class
{
    public partial class AssignStudentControl : UserControl
    {
        private readonly int _classId;
        private readonly EnrollmentService _enrollmentService;
        public AssignStudentControl()
        {
            InitializeComponent();
            string connString = Properties.Settings.Default.DefaultConnection;
            var options = new DbContextOptionsBuilder<Data.AppDbContext>()
                .UseSqlServer(connString)
                .Options;
            var context = new Data.AppDbContext(options);
            _enrollmentService = new EnrollmentService(context);
        }

        public AssignStudentControl(int id) : this()
        {
            _classId = id;
        }
        protected override async void OnLoad(EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = true;
            var enrollments = await _enrollmentService.GetEnrollmentsByClassIdAsync(_classId);
            dataGridView1.DataSource = enrollments.Select((s) => new
            {
                Id = s.Id,
                s.Student.FullName,
                Email = s.Student.Email,
                Gender = s.Student.Gender,
            }).ToList();
            dataGridView1.Columns["Id"].HeaderText= "Mã HS";
            dataGridView1.Columns["FullName"].HeaderText = "Họ tên";
            dataGridView1.Columns["Email"].HeaderText = "Email";
            dataGridView1.Columns["Gender"].HeaderText = "Giới tính";
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var popup = new PopupAddUser(_classId);
            popup.ShowDialog();
        }
    }
}
