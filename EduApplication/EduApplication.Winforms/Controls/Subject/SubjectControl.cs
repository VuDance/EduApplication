using EduApplication.EduApplication.Data;
using EduApplication.EduApplication.Services;
using EduApplication.EduApplication.Winforms.Controls.Teacher;
using Microsoft.EntityFrameworkCore;

namespace EduApplication.EduApplication.Winforms.Controls.Subject
{
    public partial class SubjectControl : UserControl
    {
        private readonly SubjectService _subjectService;
        public SubjectControl()
        {
            InitializeComponent();
            string connString = Properties.Settings.Default.DefaultConnection;

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connString)
                .Options;
            var context = new AppDbContext(options);
            _subjectService = new SubjectService(context);
            this.Load += SubjectControl_Load!;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.FindForm() is MainForm mainForm)
            {
                mainForm.LoadContent(new SubjectActionControl());
            }
        }
        private async void SubjectControl_Load(object? sender, EventArgs e)
        {
            await LoadSubjectsAsync();
        }
        private async Task LoadSubjectsAsync()
        {
            try
            {
                var subjects = await _subjectService.GetAllSubjectsAsync();
                // Gán dữ liệu vào DataGridView
                dataGridView1.AutoGenerateColumns = true;
                dataGridView1.DataSource = subjects.Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Code,
                    s.Specialty,
                    s.Description
                }).ToList();
                dataGridView1.Columns["Id"].Visible = false;
                dataGridView1.Columns["Name"].HeaderText = "Tên Môn Học";
                dataGridView1.Columns["Code"].HeaderText = "Mã Môn Học";
                dataGridView1.Columns["Specialty"].HeaderText = "Khoa";
                dataGridView1.Columns["Description"].HeaderText = "Mô Tả";
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
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách môn học: " + ex.Message);
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
                contextMenu.Items.Add("✏️ Edit", null, (s, ev) => EditSubject(id));
                contextMenu.Items.Add("🗑️ Delete", null, async (s, ev) => await DeleteSubject(id));
                contextMenu.Items.Add("ℹ️ View Details", null, (s, ev) => ViewSubject(id));

                var cellRect = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                var location = new Point(cellRect.Left, cellRect.Bottom);
                contextMenu.Show(dataGridView1, location);
            }
        }

        private void EditSubject(int id)
        {
            if (this.FindForm() is MainForm mainForm)
            {
                mainForm.LoadContent(new SubjectActionControl(id));
            }
        }

        private async Task DeleteSubject(int id)
        {
            var confirm = MessageBox.Show("Bạn có chắc muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                await _subjectService.DeleteSubjectAsync(id);
                await LoadSubjectsAsync();
            }
        }

        private void ViewSubject(int id)
        {
            if (this.FindForm() is MainForm mainForm)
            {
                mainForm.LoadContent(new SubjectActionControl(id, true));
            }
        }
    }
}
