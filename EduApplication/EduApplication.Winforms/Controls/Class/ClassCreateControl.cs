using EduApplication.EduApplication.Data;
using EduApplication.EduApplication.Services;
using EduApplication.EduApplication.Winforms.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EduApplication.EduApplication.Winforms.Controls.Class
{
    public partial class ClassCreateControl : UserControl
    {
        private readonly TeacherService _teacherService;
        private readonly SubjectService _subjectService;
        private readonly ClassService _classService;
        private ErrorProvider errorProvider = new ErrorProvider();
        private int? _editingClassId = null;
        private bool? _isView = null;
        public ClassCreateControl()
        {
            InitializeComponent();
            string connString = Properties.Settings.Default.DefaultConnection;

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connString)
                .Options;
            var context = new AppDbContext(options);
            _subjectService = new SubjectService(context);
            _teacherService = new TeacherService(context, new AuthService(context));
            _classService = new ClassService(context);
        }

        public ClassCreateControl(int id, bool isView = false) : this()
        {
            _editingClassId = id;
            _isView = isView;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            var cls = new ClassDto
            {
                Id = _editingClassId ?? 0,
                ClassName = textBox1.Text.Trim(),
                SubjectId = (int)comboBox1.SelectedValue,
                TeacherId = (int)comboBox2.SelectedValue,
                StartDate = dateTimePicker1.Value,
                EndDate = dateTimePicker2.Value,
                MaxStudent = (int)numericUpDown1.Value,
                Schedule = textBox2.Text.Trim(),
            };

            if (_editingClassId == null)
            {
                await _classService.CreateClassAsync(cls);
                MessageBox.Show("Thêm lớp thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                await _classService.UpdateClassAsync(cls);
                MessageBox.Show("Cập nhật lớp thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (this.FindForm() is MainForm mainForm)
            {
                mainForm.LoadContent(new StudentsControl());
            }
        }

        private bool ValidateInput()
        {
            bool isValid = true;
            errorProvider.Clear();

            // Name
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                errorProvider.SetError(textBox1, "Tên lớp là bắt buộc");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                errorProvider.SetError(textBox2, "Lịch học là bắt buộc");
                isValid = false;
            }

            if (dateTimePicker1.Value < DateTime.Now)
            {
                errorProvider.SetError(dateTimePicker1, "Ngày bắt đầu không hợp lệ");
                isValid = false;
            }

            if (dateTimePicker2.Value < dateTimePicker1.Value)
            {
                errorProvider.SetError(dateTimePicker2, "Ngày kết thúc phải sau ngày bắt đầu");
                isValid = false;
            }

            if (comboBox1.SelectedIndex == -1)
            {
                errorProvider.SetError(comboBox1, "Vui lòng chọn lớp");
                isValid = false;
            }
            if (comboBox2.SelectedIndex == -1)
            {
                errorProvider.SetError(comboBox2, "Vui lòng chọn môn học");
                isValid = false;
            }

            return isValid;
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Dock = DockStyle.Fill;
            await LoadTeacherData();
            await LoadSubjectData();
            if (_isView == true && _editingClassId != null)
            {
                button1.Visible = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                dateTimePicker1.Enabled = false;
                dateTimePicker2.Enabled = false;
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                numericUpDown1.Enabled = false;
                label1.Text = "Xem thông tin lớp học";
                await LoadClassData(_editingClassId.Value);
            }
            else if (_editingClassId != null)
            {
                label1.Text = "Cập nhật lớp học";
                await LoadClassData(_editingClassId.Value);
            }
        }

        private async Task LoadClassData(int id)
        {
            var cls = await _classService.GetClassByIdAsync(id);
            if (cls == null) return;

            textBox1.Text = cls.ClassName;
            dateTimePicker1.Value = cls.StartDate;
            dateTimePicker2.Value = cls.EndDate;
            textBox2.Text = cls.Schedule;
            numericUpDown1.Value = cls.MaxStudent ?? 0;
            comboBox2.SelectedValue = cls.TeacherId;
            comboBox1.SelectedValue = cls.SubjectId;
        }

        private async Task LoadTeacherData()
        {
            var teachers = await _teacherService.GetAllTeachersAsync();
            comboBox2.Enabled = false;
            comboBox2.DataSource = new[] { "Đang tải..." };
            if (teachers != null && teachers.Any())
            {
                comboBox2.DataSource = teachers;
                comboBox2.DisplayMember = "FullName";
                comboBox2.ValueMember = "Id";
                comboBox2.SelectedIndex = -1;
            }
            comboBox2.Enabled = true;
        }

        private async Task LoadSubjectData()
        {
            var subjects = await _subjectService.GetAllSubjectsAsync();
            comboBox1.Enabled = false;
            comboBox1.DataSource = new[] { "Đang tải..." };
            if (subjects != null && subjects.Any())
            {
                comboBox1.DataSource = subjects;
                comboBox1.DisplayMember = "Name";
                comboBox1.ValueMember = "Id";
                comboBox1.SelectedIndex = -1;
            }
            comboBox1.Enabled = true;
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
