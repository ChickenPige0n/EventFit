namespace EventFitter
{
    partial class MainForm
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
            this.FitButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.allEventCountLabel = new System.Windows.Forms.Label();
            this.baseIndexLabel = new System.Windows.Forms.Label();
            this.windowLengthLabel = new System.Windows.Forms.Label();
            this.eventTypeLabel = new System.Windows.Forms.Label();
            this.lineIndexLabel = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // FitButton
            // 
            this.FitButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FitButton.Location = new System.Drawing.Point(125, 338);
            this.FitButton.Name = "FitButton";
            this.FitButton.Size = new System.Drawing.Size(137, 52);
            this.FitButton.TabIndex = 0;
            this.FitButton.Text = "Fit";
            this.FitButton.UseVisualStyleBackColor = true;
            this.FitButton.Click += new System.EventHandler(this.FitButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.allEventCountLabel);
            this.groupBox1.Controls.Add(this.baseIndexLabel);
            this.groupBox1.Controls.Add(this.windowLengthLabel);
            this.groupBox1.Controls.Add(this.eventTypeLabel);
            this.groupBox1.Controls.Add(this.lineIndexLabel);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.groupBox1.Location = new System.Drawing.Point(400, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(372, 191);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Informations";
            // 
            // allEventCountLabel
            // 
            this.allEventCountLabel.AutoSize = true;
            this.allEventCountLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.allEventCountLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.allEventCountLabel.Location = new System.Drawing.Point(3, 148);
            this.allEventCountLabel.Name = "allEventCountLabel";
            this.allEventCountLabel.Size = new System.Drawing.Size(194, 31);
            this.allEventCountLabel.TabIndex = 6;
            this.allEventCountLabel.Text = "AllEventsCount:";
            // 
            // baseIndexLabel
            // 
            this.baseIndexLabel.AutoSize = true;
            this.baseIndexLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.baseIndexLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.baseIndexLabel.Location = new System.Drawing.Point(3, 117);
            this.baseIndexLabel.Name = "baseIndexLabel";
            this.baseIndexLabel.Size = new System.Drawing.Size(224, 31);
            this.baseIndexLabel.TabIndex = 5;
            this.baseIndexLabel.Text = "CurrentStartIndex:";
            this.baseIndexLabel.Click += new System.EventHandler(this.baseIndexLabel_Click);
            // 
            // windowLengthLabel
            // 
            this.windowLengthLabel.AutoSize = true;
            this.windowLengthLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.windowLengthLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.windowLengthLabel.Location = new System.Drawing.Point(3, 86);
            this.windowLengthLabel.Name = "windowLengthLabel";
            this.windowLengthLabel.Size = new System.Drawing.Size(213, 31);
            this.windowLengthLabel.TabIndex = 4;
            this.windowLengthLabel.Text = "EventsHandeling:";
            // 
            // eventTypeLabel
            // 
            this.eventTypeLabel.AutoSize = true;
            this.eventTypeLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.eventTypeLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.eventTypeLabel.Location = new System.Drawing.Point(3, 55);
            this.eventTypeLabel.Name = "eventTypeLabel";
            this.eventTypeLabel.Size = new System.Drawing.Size(118, 31);
            this.eventTypeLabel.TabIndex = 1;
            this.eventTypeLabel.Text = "EventIdx:";
            this.eventTypeLabel.Click += new System.EventHandler(this.eventTypeLabel_Click);
            // 
            // lineIndexLabel
            // 
            this.lineIndexLabel.AutoSize = true;
            this.lineIndexLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.lineIndexLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lineIndexLabel.Location = new System.Drawing.Point(3, 24);
            this.lineIndexLabel.Name = "lineIndexLabel";
            this.lineIndexLabel.Size = new System.Drawing.Size(130, 31);
            this.lineIndexLabel.TabIndex = 0;
            this.lineIndexLabel.Text = "LineIndex:";
            this.lineIndexLabel.Click += new System.EventHandler(this.lineIndexLabel_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(41, 22);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(300, 300);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(784, 411);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.FitButton);
            this.Name = "MainForm";
            this.Text = "EventFitter";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Main_Paint);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Button FitButton;
        private Panel DrawPanel;
        private GroupBox groupBox1;
        private Label acutralLabel;
        private Label calcedLabel;
        private Label timeLabel;
        private Label easingLabel;
        private Label windowLengthLabel;
        private Label baseIndexLabel;
        private Label eventC;
        private Label Label;
        private Label LineNumLabel;
        private Label eventTypeLabel;
        private Label lineIndexLabel;
        private Label allEventCountLabel;
        private PictureBox pictureBox1;
    }
}