using System.Data;
using EduApplication.EduApplication.Core.Entities;
using EduApplication.EduApplication.Data;
using EduApplication.EduApplication.Services;
using EduApplication.EduApplication.Winforms.Controls.Class;
using EduApplication.EduApplication.Winforms.Shared;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EduApplication.EduApplication.Winforms.Controls
{
    public partial class ClassControl : UserControl
    {
        private readonly ClassService _classService;
        public ClassControl()
        {
            InitializeComponent();
            string connString = Properties.Settings.Default.DefaultConnection;

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connString)
                .Options;
            var context = new AppDbContext(options);
            _classService = new ClassService(context);
            this.Load += ClassControl_Load!;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.FindForm() is MainForm mainForm)
            {
                mainForm.LoadContent(new ClassCreateControl());
            }
        }

        private async void ClassControl_Load(object? sender, EventArgs e)
        {
            await LoadClassesAsync();
        }
        private async Task LoadClassesAsync()
        {
            try
            {
                var cls = await _classService.GetAllClassesAsync();

                // Gán dữ liệu vào DataGridView
                dataGridView1.AutoGenerateColumns = true;
                dataGridView1.ReadOnly = true;
                if (AppSession.CurrentUser.Role == Shared.Enums.Role.Teacher)
                {
                    using (var conn = new SqlConnection(Properties.Settings.Default.DefaultConnection))
                    using (var cmd = new SqlCommand("GetClassesByTeacherCursor", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TeacherId", AppSession.CurrentUser.OrderId);

                        var dt = new DataTable();
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }

                        var data = dt.AsEnumerable()
                         .Select(row => new
                         {
                             Id = row.Field<string>("Id"),
                             ClassName = row.Field<string>("ClassName"),
                             SubjectName = row.Field<string>("SubjectName"),
                             TeacherName = row.Field<string>("TeacherName"),
                             MaxStudent = row.Field<int>("MaxStudent"),
                             StartDate = row.Field<DateTime>("StartDate").ToShortDateString(),
                             EndDate = row.Field<DateTime>("EndDate").ToShortDateString()
                         })
                         .ToList();

                        // Gán vào DataGridView
                        dataGridView1.DataSource = data;


                        // Thiết lập hiển thị cột
                        dataGridView1.Columns["Id"].Visible = false;
                        dataGridView1.Columns["ClassName"].HeaderText = "Tên lớp";
                        dataGridView1.Columns["SubjectName"].HeaderText = "Môn học";
                        dataGridView1.Columns["TeacherName"].HeaderText = "Giáo viên";
                        dataGridView1.Columns["MaxStudent"].HeaderText = "Sĩ số";
                        dataGridView1.Columns["StartDate"].HeaderText = "Ngày bắt đầu";
                        dataGridView1.Columns["EndDate"].HeaderText = "Ngày kết thúc";
                    }
                }
                else
                {
                    dataGridView1.DataSource = cls.Select(s => new
                    {
                        Id = s.ClassId,
                        ClassName = s.ClassName,
                        Subject = s.Subject.Name,
                        Teacher = s.Teacher.FullName,
                        s.MaxStudent,
                        Start = s.StartDate.ToShortDateString(),
                        End = s.EndDate.ToShortDateString()
                    }).ToList();

                    dataGridView1.Columns["Id"].Visible = false;
                    dataGridView1.Columns["ClassName"].HeaderText = "Tên lớp";
                    dataGridView1.Columns["Subject"].HeaderText = "Môn học";
                    dataGridView1.Columns["Teacher"].HeaderText = "Giáo viên";
                    dataGridView1.Columns["MaxStudent"].HeaderText = "Sĩ số";
                    dataGridView1.Columns["Start"].HeaderText = "Ngày bắt đầu";
                    dataGridView1.Columns["End"].HeaderText = "Ngày kết thúc";
                }
                if (!dataGridView1.Columns.Contains("Actions"))
                {
                    var buttonCol = new DataGridViewButtonColumn
                    {
                        Name = "Actions",
                        HeaderText = "Hành động",
                        Text = "Chọn",
                        UseColumnTextForButtonValue = true
                    };
                    dataGridView1.Columns.Add(buttonCol);
                }

                // Cho các cột tự fit width
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dataGridView1.Columns[e.ColumnIndex].Name == "Actions")
            {
                int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Id"].Value);

                var contextMenu = new ContextMenuStrip();
                var itemEdit = contextMenu.Items.Add("✏️ Sửa", null, (s, ev) => EditClass(id));
                itemEdit.Name = "Edit";

                var itemDelete = contextMenu.Items.Add("🗑️ Xóa", null, async (s, ev) => await DeleteClass(id));
                itemDelete.Name = "Delete";

                var itemInfo = contextMenu.Items.Add("ℹ️ Xem chi tiết", null, (s, ev) => ViewClass(id));
                itemInfo.Name = "Info";

                var itemAddStudent = contextMenu.Items.Add("+ Thêm học sinh", null, (s, ev) => AddStudent(id));
                itemAddStudent.Name = "AddStudent";

                var itemAttendance = contextMenu.Items.Add("+ Điểm danh", null, (s, ev) => Attendance(id));
                itemAttendance.Name = "Attendance";

                if (AppSession.CurrentUser.Role != Shared.Enums.Role.Admin)
                {
                    contextMenu.Items["Edit"].Visible = false;
                    contextMenu.Items["Delete"].Visible = false;
                    contextMenu.Items["AddStudent"].Visible = false;
                }

                if(AppSession.CurrentUser.Role == Shared.Enums.Role.Student)
                {
                    var itemRes = contextMenu.Items.Add("Đăng ký", null, (s, ev) => RegisterClass(id));
                    itemRes.Name = "Register";
                    var cls = await _classService.GetAllClassesAsync();
                    var isRegistered = cls.Any(c => c.ClassId == id && (c.Enrollments?.Any(st => st.StudentId == AppSession.CurrentUser.OrderId) ?? false));
                    contextMenu.Items["Register"].Enabled = !isRegistered;
                    contextMenu.Items["Attendance"].Visible = false;
                }

                var cellRect = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                var location = new Point(cellRect.Left, cellRect.Bottom);
                contextMenu.Show(dataGridView1, location);
            }
        }
        private void EditClass(int id)
        {
            if (this.FindForm() is MainForm mainForm)
            {
                mainForm.LoadContent(new ClassCreateControl(id));
            }
        }
        private async void RegisterClass(int id)
        {
            try
            {
                using (var conn = new SqlConnection(Properties.Settings.Default.DefaultConnection))
                using (var cmd = new SqlCommand("sp_EnrollStudentsToClass", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@ClassId", id));

                    cmd.Parameters.Add(new SqlParameter("@StudentId", AppSession.CurrentUser.OrderId));

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    MessageBox.Show("Đăng ký lớp học thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đăng ký lớp học: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                await LoadClassesAsync();
            }
        }
        private void Attendance(int id)
        {
            if (this.FindForm() is MainForm mainForm)
            {
                mainForm.LoadContent(new AttendanceControl(id));
            }
        }

        private async Task DeleteClass(int id)
        {
            var confirm = MessageBox.Show("Bạn có chắc muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                await _classService.DeleteClassAsync(id);
                await LoadClassesAsync();
            }
        }

        private void AddStudent(int id)
        {
            if (this.FindForm() is MainForm mainForm)
            {
                mainForm.LoadContent(new AssignStudentControl(id));
            }
        }

        private void ViewClass(int id)
        {
            if (this.FindForm() is MainForm mainForm)
            {
                mainForm.LoadContent(new ClassCreateControl(id, true));
            }
        }
    }
}
