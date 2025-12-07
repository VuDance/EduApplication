using System;
using System.Data;
using System.Threading.Tasks;
using EduApplication.EduApplication.Services;
using EduApplication.EduApplication.Winforms.Dtos;
using EduApplication.EduApplication.Winforms.Shared.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

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

        private void button1_Click(object sender, EventArgs e)
        {
            var dt = ExecuteStoredProcedure(
                "proc_GetAttendence",
                new SqlParameter("@ClassId", _classId),
                new SqlParameter("@Date", DateOnly.FromDateTime(DateTime.Now.Date))
            );

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu.");
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel file|*.xlsx";
                sfd.FileName = "BaoCao.xlsx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ExportToExcel(dt, sfd.FileName);
                    MessageBox.Show("Xuất Excel thành công!");
                }
            }
        }
        public static DataTable ExecuteStoredProcedure(string procName, params SqlParameter[] parameters)
        {
            var dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DefaultConnection))
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                conn.Open();
                da.Fill(dt);
            }

            return dt;
        }
        public static void ExportToExcel(DataTable dt, string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Data");

                // ---- HEADER ----
                ws.Cells[1, 1].Value = "Họ và tên";
                ws.Cells[1, 2].Value = "Trạng thái";
                ws.Cells[1, 3].Value = "Email";

                // ---- DATA ----
                int row = 2;

                foreach (DataRow dr in dt.Rows)
                {
                    ws.Cells[row, 1].Value = dr["FullName"];
                    ws.Cells[row, 2].Value = (AttendanceStatus)dr["Status"] == AttendanceStatus.Absent ? "Vắng" : "Có mặt";
                    ws.Cells[row, 3].Value = dr["Email"];
                    row++;
                }

                ws.Cells.AutoFitColumns();
                package.SaveAs(new FileInfo(filePath));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.FindForm() is MainForm mainForm)
            {
                mainForm.LoadContent(new ClassControl());
            }
        }
    }
}
