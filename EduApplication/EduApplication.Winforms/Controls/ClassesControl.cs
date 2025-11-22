namespace EduApplication.EduApplication.Winforms.Controls
{
    public class ClassesControl : UserControl
    {
        public ClassesControl()
        {
            var label = new Label
            {
                Text = "Quản lý lớp học",
                Dock = DockStyle.Top,
                Font = new System.Drawing.Font("Segoe UI", 14)
            };
            this.Controls.Add(label);
        }
    }
}
