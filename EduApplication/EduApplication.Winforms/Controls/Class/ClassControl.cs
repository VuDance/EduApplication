using System.Data;
using EduApplication.EduApplication.Data;
using EduApplication.EduApplication.Services;
using EduApplication.EduApplication.Winforms.Controls.Class;
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
                contextMenu.Items.Add("✏️ Edit", null, (s, ev) => EditClass(id));
                contextMenu.Items.Add("🗑️ Delete", null, async (s, ev) => await DeleteClass(id));
                contextMenu.Items.Add("ℹ️ View Details", null, (s, ev) => ViewClass(id));
                contextMenu.Items.Add("+ Add Student", null, (s, ev) => AddStudent(id));
                contextMenu.Items.Add("+ Attendance", null, (s, ev) => Attendance(id));

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
