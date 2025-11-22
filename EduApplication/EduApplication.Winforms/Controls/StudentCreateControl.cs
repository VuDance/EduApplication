using System.Threading.Tasks;
using EduApplication.EduApplication.Data;
using EduApplication.EduApplication.Services;
using EduApplication.EduApplication.Winforms.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EduApplication.EduApplication.Winforms.Controls
{
    public partial class StudentCreateControl : UserControl
    {
        private ErrorProvider errorProvider = new ErrorProvider();
        private readonly StudentService _studentService;
        private int? _editingStudentId = null;
        private bool? _isView = null;
        public StudentCreateControl()
        {
            InitializeComponent();
            string connString = Properties.Settings.Default.DefaultConnection;

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connString)
                .Options;
            var context = new AppDbContext(options);
            var authService = new AuthService(context);
            _studentService = new StudentService(context, authService);
        }
        public StudentCreateControl(int id, bool isView=false) : this()
        {
            _editingStudentId = id;
            _isView = isView;
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Dock = DockStyle.Fill;
            if (_isView == true && _editingStudentId != null)
            {
                button1.Visible = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                textBox4.Enabled = false;
                dateTimePicker1.Enabled = false;
                dateTimePicker2.Enabled = false;
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                label1.Text = "Xem thông tin sinh viên";
                await LoadStudentData(_editingStudentId.Value);
            }
            else if (_editingStudentId != null)
            {
                label1.Text = "Cập nhật sinh viên";
                await LoadStudentData(_editingStudentId.Value);
            }
        }

        private async Task LoadStudentData(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null) return;

            textBox1.Text = student.FullName;
            dateTimePicker1.Value = student.DateOfBirth;
            textBox3.Text = student.Email;
            textBox4.Text = student.PhoneNumber;
            textBox2.Text = student.Address;
            dateTimePicker2.Value = student.RegisterDate;

            textBox3.Enabled = false;
            if (student.Gender == Shared.Enums.Gender.Male)
                radioButton2.Checked = true;
            else
                radioButton1.Checked = true;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInput())
                    return;

                var student = new StudentDto
                {
                    Id = _editingStudentId ?? 0,
                    FullName = textBox1.Text,
                    DateOfBirth = dateTimePicker1.Value,
                    Email = textBox3.Text,
                    PhoneNumber = textBox4.Text,
                    Address = textBox2.Text,
                    RegisterDate = dateTimePicker2.Value,
                    Gender = (Shared.Enums.Gender)GetSelectedGender()
                };

                if (_editingStudentId == null)
                {
                    await _studentService.CreateStudentAsync(student);
                    MessageBox.Show("Thêm sinh viên thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    await _studentService.UpdateStudentAsync(student);
                    MessageBox.Show("Cập nhật sinh viên thành công!", "Thông báo",
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
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                errorProvider.SetError(textBox4, "Số điện thoại là bắt buộc");
                isValid = false;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(textBox4.Text, @"^\d{9,11}$"))
            {
                errorProvider.SetError(textBox4, "Số điện thoại phải có 9-11 chữ số");
                isValid = false;
            }

            // Address
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                errorProvider.SetError(textBox2, "Địa chỉ là bắt buộc");
                isValid = false;
            }

            // Ngày sinh
            if (dateTimePicker1.Value > DateTime.Now)
            {
                errorProvider.SetError(dateTimePicker1, "Ngày sinh không hợp lệ");
                isValid = false;
            }

            // Ngày đăng ký
            if (dateTimePicker2.Value < dateTimePicker1.Value)
            {
                errorProvider.SetError(dateTimePicker2, "Ngày đăng ký phải sau ngày sinh");
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

        private int GetSelectedGender()
        {
            if (radioButton2.Checked)
                return 1; // Nam
            else
                return 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.FindForm() is MainForm mainForm)
            {
                mainForm.LoadContent(new StudentsControl());
            }
        }
    }
}
