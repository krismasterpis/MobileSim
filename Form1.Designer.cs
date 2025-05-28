namespace MobileSim
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            groupBox3 = new GroupBox();
            trackBar1 = new TrackBar();
            button1 = new Button();
            groupBox2 = new GroupBox();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            groupBox1 = new GroupBox();
            comboBoxMode = new ComboBox();
            label1 = new Label();
            pictureBoxMap = new PictureBox();
            mapPanel = new Panel();
            panel3 = new Panel();
            loggerTextBox = new TextBox();
            groupBox4 = new GroupBox();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            label10 = new Label();
            label11 = new Label();
            label12 = new Label();
            label13 = new Label();
            panel1.SuspendLayout();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxMap).BeginInit();
            mapPanel.SuspendLayout();
            panel3.SuspendLayout();
            groupBox4.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ControlDark;
            panel1.Controls.Add(groupBox4);
            panel1.Controls.Add(groupBox3);
            panel1.Controls.Add(button1);
            panel1.Controls.Add(groupBox2);
            panel1.Controls.Add(groupBox1);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(3);
            panel1.Size = new Size(181, 681);
            panel1.TabIndex = 0;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(trackBar1);
            groupBox3.Dock = DockStyle.Top;
            groupBox3.Location = new Point(3, 268);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(175, 183);
            groupBox3.TabIndex = 4;
            groupBox3.TabStop = false;
            groupBox3.Text = "Base station parameters";
            // 
            // trackBar1
            // 
            trackBar1.Dock = DockStyle.Top;
            trackBar1.Location = new Point(3, 19);
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(169, 45);
            trackBar1.TabIndex = 0;
            trackBar1.TickStyle = TickStyle.None;
            // 
            // button1
            // 
            button1.Dock = DockStyle.Top;
            button1.Location = new Point(3, 245);
            button1.Name = "button1";
            button1.RightToLeft = RightToLeft.No;
            button1.Size = new Size(175, 23);
            button1.TabIndex = 2;
            button1.Text = "Symuluj połączenie";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label5);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(label2);
            groupBox2.Dock = DockStyle.Top;
            groupBox2.Location = new Point(3, 145);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(175, 100);
            groupBox2.TabIndex = 3;
            groupBox2.TabStop = false;
            groupBox2.Text = "Nodes";
            // 
            // label5
            // 
            label5.Dock = DockStyle.Top;
            label5.Location = new Point(3, 64);
            label5.Name = "label5";
            label5.Size = new Size(169, 15);
            label5.TabIndex = 5;
            label5.Text = "Receivers (0/1)";
            // 
            // label4
            // 
            label4.Dock = DockStyle.Top;
            label4.Location = new Point(3, 49);
            label4.Name = "label4";
            label4.Size = new Size(169, 15);
            label4.TabIndex = 4;
            label4.Text = "Senders (0/1)";
            // 
            // label3
            // 
            label3.Dock = DockStyle.Top;
            label3.Location = new Point(3, 34);
            label3.Name = "label3";
            label3.Size = new Size(169, 15);
            label3.TabIndex = 3;
            label3.Text = "Obstacles (0/50)";
            // 
            // label2
            // 
            label2.Dock = DockStyle.Top;
            label2.Location = new Point(3, 19);
            label2.Name = "label2";
            label2.Size = new Size(169, 15);
            label2.TabIndex = 2;
            label2.Text = "Base stations (0/10)";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(comboBoxMode);
            groupBox1.Controls.Add(label1);
            groupBox1.Dock = DockStyle.Top;
            groupBox1.Location = new Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(175, 142);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Tools";
            // 
            // comboBoxMode
            // 
            comboBoxMode.Dock = DockStyle.Top;
            comboBoxMode.FormattingEnabled = true;
            comboBoxMode.Items.AddRange(new object[] { "BaseStation", "Obstacle", "Sender", "Receiver" });
            comboBoxMode.Location = new Point(3, 34);
            comboBoxMode.Name = "comboBoxMode";
            comboBoxMode.Size = new Size(169, 23);
            comboBoxMode.TabIndex = 0;
            // 
            // label1
            // 
            label1.Dock = DockStyle.Top;
            label1.Location = new Point(3, 19);
            label1.Name = "label1";
            label1.Size = new Size(169, 15);
            label1.TabIndex = 1;
            label1.Text = "Zoom: 100%";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pictureBoxMap
            // 
            pictureBoxMap.BackColor = SystemColors.Control;
            pictureBoxMap.Location = new Point(0, 3);
            pictureBoxMap.Name = "pictureBoxMap";
            pictureBoxMap.Size = new Size(1000, 1033);
            pictureBoxMap.TabIndex = 1;
            pictureBoxMap.TabStop = false;
            // 
            // mapPanel
            // 
            mapPanel.AutoScroll = true;
            mapPanel.BackColor = SystemColors.ControlDark;
            mapPanel.BorderStyle = BorderStyle.FixedSingle;
            mapPanel.Controls.Add(pictureBoxMap);
            mapPanel.Dock = DockStyle.Left;
            mapPanel.Location = new Point(181, 0);
            mapPanel.Name = "mapPanel";
            mapPanel.Size = new Size(1002, 681);
            mapPanel.TabIndex = 2;
            // 
            // panel3
            // 
            panel3.BackColor = SystemColors.ControlDark;
            panel3.Controls.Add(loggerTextBox);
            panel3.Dock = DockStyle.Right;
            panel3.Location = new Point(1183, 0);
            panel3.Name = "panel3";
            panel3.Padding = new Padding(3);
            panel3.Size = new Size(721, 681);
            panel3.TabIndex = 1;
            // 
            // loggerTextBox
            // 
            loggerTextBox.BackColor = SystemColors.WindowText;
            loggerTextBox.Dock = DockStyle.Fill;
            loggerTextBox.Enabled = false;
            loggerTextBox.ForeColor = SystemColors.Window;
            loggerTextBox.Location = new Point(3, 3);
            loggerTextBox.Multiline = true;
            loggerTextBox.Name = "loggerTextBox";
            loggerTextBox.Size = new Size(715, 675);
            loggerTextBox.TabIndex = 0;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(label11);
            groupBox4.Controls.Add(label12);
            groupBox4.Controls.Add(label10);
            groupBox4.Controls.Add(label13);
            groupBox4.Controls.Add(label9);
            groupBox4.Controls.Add(label8);
            groupBox4.Controls.Add(label7);
            groupBox4.Controls.Add(label6);
            groupBox4.Dock = DockStyle.Top;
            groupBox4.Location = new Point(3, 451);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(175, 156);
            groupBox4.TabIndex = 5;
            groupBox4.TabStop = false;
            groupBox4.Text = "Cell parameters";
            // 
            // label6
            // 
            label6.Dock = DockStyle.Top;
            label6.Location = new Point(3, 19);
            label6.Name = "label6";
            label6.Size = new Size(169, 15);
            label6.TabIndex = 3;
            label6.Text = "X:";
            // 
            // label7
            // 
            label7.Dock = DockStyle.Top;
            label7.Location = new Point(3, 34);
            label7.Name = "label7";
            label7.Size = new Size(169, 15);
            label7.TabIndex = 4;
            label7.Text = "Y:";
            // 
            // label8
            // 
            label8.Dock = DockStyle.Top;
            label8.Location = new Point(3, 49);
            label8.Name = "label8";
            label8.Size = new Size(169, 15);
            label8.TabIndex = 5;
            label8.Text = "Cell type:";
            // 
            // label9
            // 
            label9.Dock = DockStyle.Top;
            label9.Location = new Point(3, 64);
            label9.Name = "label9";
            label9.Size = new Size(169, 15);
            label9.TabIndex = 6;
            label9.Text = "Signal strength:";
            // 
            // label10
            // 
            label10.Dock = DockStyle.Top;
            label10.Location = new Point(3, 94);
            label10.Name = "label10";
            label10.Size = new Size(169, 15);
            label10.TabIndex = 7;
            label10.Text = "Best station";
            label10.TextAlign = ContentAlignment.TopCenter;
            // 
            // label11
            // 
            label11.Dock = DockStyle.Top;
            label11.Location = new Point(3, 124);
            label11.Name = "label11";
            label11.Size = new Size(169, 15);
            label11.TabIndex = 9;
            label11.Text = "Y:";
            // 
            // label12
            // 
            label12.Dock = DockStyle.Top;
            label12.Location = new Point(3, 109);
            label12.Name = "label12";
            label12.Size = new Size(169, 15);
            label12.TabIndex = 8;
            label12.Text = "X:";
            // 
            // label13
            // 
            label13.Dock = DockStyle.Top;
            label13.Location = new Point(3, 79);
            label13.Name = "label13";
            label13.Size = new Size(169, 15);
            label13.TabIndex = 10;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(1904, 681);
            Controls.Add(panel3);
            Controls.Add(mapPanel);
            Controls.Add(panel1);
            MinimumSize = new Size(600, 400);
            Name = "Form1";
            Text = "Form1";
            WindowState = FormWindowState.Maximized;
            panel1.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxMap).EndInit();
            mapPanel.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            groupBox4.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private ComboBox comboBoxMode;
        private PictureBox pictureBoxMap;
        private Panel mapPanel;
        private Panel panel3;
        private Label label1;
        private GroupBox groupBox1;
        private Button button1;
        private GroupBox groupBox2;
        private Label label2;
        private Label label5;
        private Label label4;
        private Label label3;
        private GroupBox groupBox3;
        private TrackBar trackBar1;
        private TextBox loggerTextBox;
        private GroupBox groupBox4;
        private Label label13;
        private Label label11;
        private Label label12;
        private Label label10;
        private Label label9;
        private Label label8;
        private Label label7;
        private Label label6;
    }
}
