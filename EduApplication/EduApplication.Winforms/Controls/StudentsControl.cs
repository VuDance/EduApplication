using System.Threading.Tasks;
using EduApplication.EduApplication.Data;
using EduApplication.EduApplication.Services;
using Microsoft.EntityFrameworkCore;

namespace EduApplication.EduApplication.Winforms.Controls
{
    public partial class StudentsControl : UserControl
    {
        private readonly StudentService _studentService;
        public StudentsControl()
        {
            InitializeComponent();
            string connString = Properties.Settings.Default.DefaultConnection;

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connString)
                .Options;
            var context = new AppDbContext(options);
            var authService = new AuthService(context);
            _studentService = new StudentService(context, authService);
            this.Load += StudentControl_Load!;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.FindForm() is MainForm mainForm)
            {
                mainForm.LoadContent(new StudentCreateControl());
            }
        }
        private async void StudentControl_Load(object? sender, EventArgs e)
        {
            await LoadStudentsAsync();
        }
        private async Task LoadStudentsAsync()
        {
            try
            {
                var students = await _studentService.GetAllStudentsAsync();

                // Gán dữ liệu vào DataGridView
                dataGridView1.AutoGenerateColumns = true; // để nó tự sinh cột

                dataGridView1.DataSource = students.Select(s => new
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    Email = s.Email,
                    PhoneNumber = s.PhoneNumber,
                    Address = s.Address,
                    Gender = s.Gender.ToString(),
                    DateOfBirth = s.DateOfBirth.ToShortDateString(),
                    RegisterDate = s.RegisterDate.ToShortDateString()
                }).ToList();

                dataGridView1.Columns["Id"].Visible = false;
                dataGridView1.Columns["FullName"].HeaderText = "Họ Tên";
                dataGridView1.Columns["PhoneNumber"].HeaderText = "Số Điện Thoại";
                dataGridView1.Columns["Address"].HeaderText = "Địa Chỉ";
                dataGridView1.Columns["Gender"].HeaderText = "Giới Tính";
                dataGridView1.Columns["DateOfBirth"].HeaderText = "Ngày Sinh";
                dataGridView1.Columns["RegisterDate"].HeaderText = "Ngày Đăng Ký";

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
                int studentId = (int)dataGridView1.Rows[e.RowIndex].Cells["Id"].Value;

                var contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add("✏️ Edit", null, (s, ev) => EditStudent(studentId));
                contextMenu.Items.Add("🗑️ Delete", null, (s, ev) => DeleteStudent(studentId));
                contextMenu.Items.Add("ℹ️ View Details", null, (s, ev) => ViewStudent(studentId));

                var cellRect = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                var location = new Point(cellRect.Left, cellRect.Bottom);
                contextMenu.Show(dataGridView1, location);
            }
        }
        private void EditStudent(int id)
        {
            if (this.FindForm() is MainForm mainForm)
            {
                mainForm.LoadContent(new StudentCreateControl(id));
            }
        }

        private async Task DeleteStudent(int id)
        {
            var confirm = MessageBox.Show("Bạn có chắc muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                await _studentService.DeleteStudentAsync(id);
                await LoadStudentsAsync();
            }
        }

        private void ViewStudent(int id)
        {
            if (this.FindForm() is MainForm mainForm)
            {
                mainForm.LoadContent(new StudentCreateControl(id, true));
            }
        }

    }
}
