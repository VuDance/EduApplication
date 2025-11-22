using EduApplication.EduApplication.Services;
using EduApplication.EduApplication.Winforms.Dtos;
using EduApplication.EduApplication.Winforms.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace EduApplication.EduApplication.Winforms.Controls.Subject
{
    public partial class SubjectActionControl : UserControl
    {
        private ErrorProvider errorProvider = new ErrorProvider();
        private int? _editingSubjectId = null;
        private bool? _isView = null;
        private readonly SubjectService _subjectService;
        public SubjectActionControl()
        {
            InitializeComponent();
            string connString = Properties.Settings.Default.DefaultConnection;
            var options = new DbContextOptionsBuilder<Data.AppDbContext>()
                .UseSqlServer(connString)
                .Options;
            var context = new Data.AppDbContext(options);
            _subjectService = new SubjectService(context);
            this.Load += SubjectActionControl_Load!;
        }
        public SubjectActionControl(int id, bool isView = false) : this()
        {
            _editingSubjectId = id;
            _isView = isView;
        }
        private void SubjectActionControl_Load(object sender, EventArgs e)
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
            if (_isView == true && _editingSubjectId != null)
            {
                button1.Visible = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                comboBox1.Enabled = false;
                richTextBox1.Enabled = false;
                label1.Text = "Xem thông tin môn học";
                await LoadSubjectData(_editingSubjectId.Value);
            }
            else if (_editingSubjectId != null)
            {
                label1.Text = "Cập nhật thông tin môn học";
                textBox1.Enabled = false;
                await LoadSubjectData(_editingSubjectId.Value);
            }
            else
            {
                label1.Text = "Thêm mới môn học";
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInput())
                    return;
                var subjectDto = new SubjectDto
                {
                    Id = _editingSubjectId ?? 0,
                    Code = textBox1.Text.Trim(),
                    Name = textBox2.Text.Trim(),
                    Specialty = (Specialty)comboBox1.SelectedValue,
                    Description = richTextBox1.Text.Trim()
                };
                if (_editingSubjectId == null)
                {
                    // Create new subject
                    await _subjectService.CreateSubjectAsync(subjectDto);
                    MessageBox.Show("Thêm môn học thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Update existing subject
                    await _subjectService.UpdateSubjectAsync(subjectDto);
                    MessageBox.Show("Cập nhật môn học thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                if (this.FindForm() is MainForm mainForm)
                {
                    mainForm.LoadContent(new SubjectControl());
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

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                errorProvider.SetError(textBox1, "Mã môn học là bắt buộc");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                errorProvider.SetError(textBox2, "Tên môn học là bắt buộc");
                isValid = false;
            }


            return isValid;
        }

        private async Task LoadSubjectData(int id)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);
            if (subject == null) return;

            textBox1.Text = subject.Code;
            textBox2.Text = subject.Name;
            comboBox1.SelectedValue = subject.Specialty;
            richTextBox1.Text = subject.Description;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.FindForm() is MainForm mainForm)
            {
                mainForm.LoadContent(new SubjectControl());
            }
        }
    }
}
