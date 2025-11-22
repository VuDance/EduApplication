using EduApplication.EduApplication.Data;
using EduApplication.EduApplication.Services;
using Microsoft.EntityFrameworkCore;

namespace EduApplication.EduApplication.Winforms.Controls.Teacher
{
    public partial class TeacherControl : UserControl
    {
        private readonly TeacherService _teacherService;
        public TeacherControl()
        {
            InitializeComponent();
            string connString = Properties.Settings.Default.DefaultConnection;

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connString)
                .Options;
            var context = new AppDbContext(options);
            var authService = new AuthService(context);
            _teacherService = new TeacherService(context, authService);
            this.Load += TeacherControl_Load!;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.FindForm() is MainForm mainForm)
            {
                mainForm.LoadContent(new TeacherActionControl());
            }
        }
        private async void TeacherControl_Load(object? sender, EventArgs e)
        {
            await LoadTeachersAsync();
        }
        private async Task LoadTeachersAsync()
        {
            try
            {
                var teacher = await _teacherService.GetAllTeachersAsync();

                // Gán dữ liệu vào DataGridView
                dataGridView1.AutoGenerateColumns = true;

                dataGridView1.DataSource = teacher.Select(s => new
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    Email = s.Email,
                    PhoneNumber = s.PhoneNumber,
                    DateOfBirth = s.DateOfBirth.ToShortDateString(),
                    HireDate = s.HireDate.ToShortDateString(),
                    Specialty = s.Specialty,
                }).ToList();

                dataGridView1.Columns["Id"].Visible = false;
                dataGridView1.Columns["FullName"].HeaderText = "Họ Tên";
                dataGridView1.Columns["Email"].HeaderText = "Email";
                dataGridView1.Columns["PhoneNumber"].HeaderText = "Số Điện Thoại";
                dataGridView1.Columns["DateOfBirth"].HeaderText = "Ngày Sinh";
                dataGridView1.Columns["HireDate"].HeaderText = "Ngày Vào Làm";
                dataGridView1.Columns["Specialty"].HeaderText = "Chuyên Môn";

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

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dataGridView1.Columns[e.ColumnIndex].Name == "Actions")
            {
                int id = (int)dataGridView1.Rows[e.RowIndex].Cells["Id"].Value;

                var contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add("✏️ Edit", null, (s, ev) => EditTeacher(id));
                contextMenu.Items.Add("🗑️ Delete", null, async (s, ev) => await DeleteTeacher(id));
                contextMenu.Items.Add("ℹ️ View Details", null, (s, ev) => ViewTeacher(id));

                var cellRect = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                var location = new Point(cellRect.Left, cellRect.Bottom);
                contextMenu.Show(dataGridView1, location);
            }
        }
        private void EditTeacher(int id)
        {
            if (this.FindForm() is MainForm mainForm)
            {
                mainForm.LoadContent(new TeacherActionControl(id));
            }
        }

        private async Task DeleteTeacher(int id)
        {
            var confirm = MessageBox.Show("Bạn có chắc muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                await _teacherService.DeleteTeacherAsync(id);
                await LoadTeachersAsync();
            }
        }

        private void ViewTeacher(int id)
        {
            if (this.FindForm() is MainForm mainForm)
            {
                mainForm.LoadContent(new TeacherActionControl(id, true));
            }
        }
    }
}
