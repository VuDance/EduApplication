using System.Data;
using System.Threading.Tasks;
using EduApplication.EduApplication.Services;
using EduApplication.EduApplication.Winforms.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EduApplication.EduApplication.Winforms.Controls.Class
{
    public partial class AttendanceControl : UserControl
    {
        private readonly int _classId;
        private readonly AttendanceService _attendanceService;
        public AttendanceControl()
        {
            InitializeComponent();
            string connString = Properties.Settings.Default.DefaultConnection;
            var options = new DbContextOptionsBuilder<Data.AppDbContext>()
                .UseSqlServer(connString)
                .Options;
            var context = new Data.AppDbContext(options);
            _attendanceService = new AttendanceService(context);
        }
        public AttendanceControl(int id) : this()
        {
            _classId = id;
        }
        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await LoadAttendanceData();
        }
        private async Task LoadAttendanceData()
        {
            await _attendanceService.GenerateAttendance(DateOnly.FromDateTime(DateTime.Now.Date), _classId);

            var attendanceRecords = await _attendanceService.GetAttendanceByClassAndDateAsync(
                _classId, DateOnly.FromDateTime(DateTime.Now.Date));

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn
            {
                Name = "Status",
                HeaderText = "Điểm danh",
                DataPropertyName = "Status",
                ReadOnly = false
            };
            dataGridView1.Columns.Add(chk);

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "Mã HS",
                DataPropertyName = "Id"
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "StudentName",
                HeaderText = "Họ tên",
                DataPropertyName = "StudentName"
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Date",
                HeaderText = "Ngày điểm danh",
                DataPropertyName = "Date"
            });

            dataGridView1.DataSource = attendanceRecords.Select(s => new AttendanceValueDto
            {
                Id = s.Id,
                StudentName = s.Student.FullName,
                Date = s.Date.ToShortDateString(),
                Status = s.Status == Shared.Enums.AttendanceStatus.Present
            }).ToList();

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }


        private async void button3_Click(object sender, EventArgs e)
        {
            var listAttendance = new List<AttendanceDto>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Id"].Value == null) continue;
                var attendance = new AttendanceDto
                {
                    Id = (int)row.Cells["Id"].Value,
                    Status = Convert.ToBoolean(row.Cells["Status"].Value) ? Shared.Enums.AttendanceStatus.Present : Shared.Enums.AttendanceStatus.Absent
                };
                listAttendance.Add(attendance);
            }
            await _attendanceService.UpdateAttendanceAsync(listAttendance);
            MessageBox.Show("Cập nhật điểm danh thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
