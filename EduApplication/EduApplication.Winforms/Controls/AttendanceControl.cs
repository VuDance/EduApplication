namespace EduApplication.EduApplication.Winforms.Controls
{
    public class AttendanceControl : UserControl
    {
        public AttendanceControl()
        {
            var label = new Label
            {
                Text = "Điểm danh",
                Dock = DockStyle.Top,
                Font = new System.Drawing.Font("Segoe UI", 14)
            };
            this.Controls.Add(label);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // AttendanceControl
            // 
            Name = "AttendanceControl";
            ResumeLayout(false);

        }
    }
}
