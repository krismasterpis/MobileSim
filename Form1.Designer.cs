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
            panel1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxMap).BeginInit();
            mapPanel.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ControlDark;
            panel1.Controls.Add(button1);
            panel1.Controls.Add(groupBox2);
            panel1.Controls.Add(groupBox1);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(3);
            panel1.Size = new Size(181, 1041);
            panel1.TabIndex = 0;
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
            label5.AutoSize = true;
            label5.Dock = DockStyle.Top;
            label5.Location = new Point(3, 64);
            label5.Name = "label5";
            label5.Size = new Size(84, 15);
            label5.TabIndex = 5;
            label5.Text = "Receivers (0/1)";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = DockStyle.Top;
            label4.Location = new Point(3, 49);
            label4.Name = "label4";
            label4.Size = new Size(76, 15);
            label4.TabIndex = 4;
            label4.Text = "Senders (0/1)";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Top;
            label3.Location = new Point(3, 34);
            label3.Name = "label3";
            label3.Size = new Size(92, 15);
            label3.TabIndex = 3;
            label3.Text = "Obstacles (0/50)";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Top;
            label2.Location = new Point(3, 19);
            label2.Name = "label2";
            label2.Size = new Size(109, 15);
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
            mapPanel.Size = new Size(1002, 1041);
            mapPanel.TabIndex = 2;
            // 
            // panel3
            // 
            panel3.BackColor = SystemColors.ControlDark;
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(1183, 0);
            panel3.Name = "panel3";
            panel3.Padding = new Padding(3);
            panel3.Size = new Size(721, 1041);
            panel3.TabIndex = 1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1904, 1041);
            Controls.Add(panel3);
            Controls.Add(mapPanel);
            Controls.Add(panel1);
            Name = "Form1";
            Text = "Form1";
            WindowState = FormWindowState.Maximized;
            panel1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxMap).EndInit();
            mapPanel.ResumeLayout(false);
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
    }
}
