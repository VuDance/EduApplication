namespace EduApplication.EduApplication.Winforms.Controls.Class
{
    partial class AssignStudentControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            button3 = new Button();
            label1 = new Label();
            button2 = new Button();
            panel2 = new Panel();
            button1 = new Button();
            panel3 = new Panel();
            dataGridView1 = new DataGridView();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(button3);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(button2);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(595, 60);
            panel1.TabIndex = 0;
            // 
            // button3
            // 
            button3.Location = new Point(478, 13);
            button3.Name = "button3";
            button3.Size = new Size(114, 30);
            button3.TabIndex = 1;
            button3.Text = "Thêm học viên";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.HotTrack;
            label1.Location = new Point(151, 11);
            label1.Name = "label1";
            label1.Size = new Size(272, 32);
            label1.TabIndex = 8;
            label1.Text = "Thêm học viên vào lớp";
            // 
            // button2
            // 
            button2.Image = Properties.Resources.icons8_arrow_left_301;
            button2.Location = new Point(3, 13);
            button2.Name = "button2";
            button2.Size = new Size(40, 30);
            button2.TabIndex = 7;
            button2.Text = "<<";
            button2.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            panel2.Controls.Add(button1);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 304);
            panel2.Name = "panel2";
            panel2.Size = new Size(595, 57);
            panel2.TabIndex = 1;
            // 
            // button1
            // 
            button1.Location = new Point(255, 6);
            button1.Name = "button1";
            button1.Size = new Size(75, 33);
            button1.TabIndex = 0;
            button1.Text = "Lưu";
            button1.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            panel3.Controls.Add(dataGridView1);
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(0, 60);
            panel3.Name = "panel3";
            panel3.Size = new Size(595, 244);
            panel3.TabIndex = 2;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(0, 0);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(595, 244);
            dataGridView1.TabIndex = 0;
            // 
            // AssignStudentControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Name = "AssignStudentControl";
            Size = new Size(595, 361);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private Button button2;
        private Label label1;
        private Button button1;
        private DataGridView dataGridView1;
        private Button button3;
    }
}
