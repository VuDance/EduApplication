using EduApplication.EduApplication.Services;
using EduApplication.EduApplication.Winforms.Dtos;
using EduApplication.EduApplication.Winforms.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace EduApplication.EduApplication.Winforms.Controls.Teacher
{
    public partial class TeacherActionControl : UserControl
    {
        private ErrorProvider errorProvider = new ErrorProvider();
        private int? _editingTeacherId = null;
        private bool? _isView = null;
        private readonly TeacherService _teacherService;
        public TeacherActionControl()
        {
            InitializeComponent();
            string connString = Properties.Settings.Default.DefaultConnection;
            var options = new DbContextOptionsBuilder<Data.AppDbContext>()
                .UseSqlServer(connString)
                .Options;
            var context = new Data.AppDbContext(options);
            var authService = new AuthService(context);
            _teacherService = new TeacherService(context, authService);
        }
        public TeacherActionControl(int id, bool isView = false) : this()
        {
            _editingTeacherId = id;
            _isView = isView;
        }

        private void TeacherActionControl_Load(object sender, EventArgs e)
        {
            var specialties = new Dictionary<Specialty, string>
            {
                { Specialty.ComputerScience, "Khoa học máy tính" },
                { Specialty.ForeignLanguages, "Ngoại ngữ" }
            };

            comboBox1.DataSource = specialties.ToList();
            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
        }


        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Dock = DockStyle.Fill;
            if (_isView == true && _editingTeacherId != null)
            {
                button1.Visible = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                dateTimePicker1.Enabled = false;
                dateTimePicker2.Enabled = false;
                comboBox1.Enabled = false;
                label1.Text = "Xem thông tin giảng viên";
                await LoadTeacherData(_editingTeacherId.Value);
            }
            else if (_editingTeacherId != null)
            {
                label1.Text = "Cập nhật giảng viên";
                await LoadTeacherData(_editingTeacherId.Value);
            }
        }

        private async Task LoadTeacherData(int id)
        {
            var teacher = await _teacherService.GetTeacherByIdAsync(id);
            if (teacher == null) return;

            textBox1.Text = teacher.FullName;
            dateTimePicker2.Value = teacher.DateOfBirth;
            dateTimePicker1.Value = teacher.HireDate;
            textBox3.Text = teacher.Email;
            textBox2.Text = teacher.PhoneNumber;

            textBox3.Enabled = false;

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInput())
                    return;

                var teacher = new TeacherDto
                {
                    Id = _editingTeacherId ?? 0,
                    FullName = textBox1.Text,
                    DateOfBirth = dateTimePicker2.Value,
                    HireDate = dateTimePicker1.Value,
                    Email = textBox3.Text,
                    PhoneNumber = textBox2.Text,
                    Specialty = (Specialty)comboBox1.SelectedValue,
                };

                if (_editingTeacherId == null)
                {
                    await _teacherService.CreateTeacherAsync(teacher);
                    MessageBox.Show("Thêm giảng viên thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    await _teacherService.UpdateTeacherAsync(teacher);
                    MessageBox.Show("Cập nhật giảng viên thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool ValidateInput()
        {
            bool isValid = true;
            errorProvider.Clear();

            // Name
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                errorProvider.SetError(textBox1, "Họ tên là bắt buộc");
                isValid = false;
            }

            // Email
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                errorProvider.SetError(textBox3, "Email là bắt buộc");
                isValid = false;
            }
            else if (!IsValidEmail(textBox3.Text))
            {
                errorProvider.SetError(textBox3, "Email không hợp lệ");
                isValid = false;
            }

            // Phone
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                errorProvider.SetError(textBox2, "Số điện thoại là bắt buộc");
                isValid = false;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(textBox2.Text, @"^\d{9,11}$"))
            {
                errorProvider.SetError(textBox2, "Số điện thoại phải có 9-11 chữ số");
                isValid = false;
            }

            // Ngày sinh
            if (dateTimePicker2.Value >= DateTime.Now)
            {
                errorProvider.SetError(dateTimePicker2, "Ngày sinh không hợp lệ");
                isValid = false;
            }
            if (dateTimePicker1.Value > DateTime.Now)
            {
                errorProvider.SetError(dateTimePicker1, "Ngày vào làm không hợp lệ");
                isValid = false;
            }

            return isValid;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.FindForm() is MainForm mainForm)
            {
                mainForm.LoadContent(new TeacherControl());
            }
        }
    }
}
