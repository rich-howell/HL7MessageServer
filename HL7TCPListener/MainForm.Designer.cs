namespace HL7TCPListener
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnStart = new Button();
            btnStop = new Button();
            btnClearLogs = new Button();
            label1 = new Label();
            label2 = new Label();
            lblStatus = new Label();
            numPort = new NumericUpDown();
            label3 = new Label();
            folderBrowserDialog = new FolderBrowserDialog();
            btnBrowse = new Button();
            txtfolderPath = new TextBox();
            label4 = new Label();
            txtLogs = new RichTextBox();
            ((System.ComponentModel.ISupportInitialize)numPort).BeginInit();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Location = new Point(16, 365);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(75, 23);
            btnStart.TabIndex = 0;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(97, 365);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(75, 23);
            btnStop.TabIndex = 1;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnClearLogs
            // 
            btnClearLogs.Location = new Point(178, 365);
            btnClearLogs.Name = "btnClearLogs";
            btnClearLogs.Size = new Size(75, 23);
            btnClearLogs.TabIndex = 3;
            btnClearLogs.Text = "Clear Logs";
            btnClearLogs.UseVisualStyleBackColor = true;
            btnClearLogs.Click += btnClearLogs_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(12, 10);
            label1.Name = "label1";
            label1.Size = new Size(32, 15);
            label1.TabIndex = 4;
            label1.Text = "Logs";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(325, 389);
            label2.Name = "label2";
            label2.Size = new Size(45, 15);
            label2.TabIndex = 5;
            label2.Text = "Status:";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(376, 389);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(75, 15);
            lblStatus.TabIndex = 6;
            lblStatus.Text = "Not Running";
            // 
            // numPort
            // 
            numPort.Location = new Point(16, 330);
            numPort.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
            numPort.Minimum = new decimal(new int[] { 4000, 0, 0, 0 });
            numPort.Name = "numPort";
            numPort.Size = new Size(120, 23);
            numPort.TabIndex = 7;
            numPort.Value = new decimal(new int[] { 4000, 0, 0, 0 });
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(16, 307);
            label3.Name = "label3";
            label3.Size = new Size(84, 15);
            label3.TabIndex = 8;
            label3.Text = "Listening Port";
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new Point(371, 330);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(75, 23);
            btnBrowse.TabIndex = 9;
            btnBrowse.Text = "Browse";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // txtfolderPath
            // 
            txtfolderPath.Location = new Point(153, 330);
            txtfolderPath.Name = "txtfolderPath";
            txtfolderPath.Size = new Size(212, 23);
            txtfolderPath.TabIndex = 10;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(153, 312);
            label4.Name = "label4";
            label4.Size = new Size(84, 15);
            label4.TabIndex = 11;
            label4.Text = "Save Location";
            // 
            // txtLogs
            // 
            txtLogs.Location = new Point(12, 28);
            txtLogs.Name = "txtLogs";
            txtLogs.Size = new Size(434, 261);
            txtLogs.TabIndex = 12;
            txtLogs.Text = "";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(463, 411);
            Controls.Add(txtLogs);
            Controls.Add(label4);
            Controls.Add(txtfolderPath);
            Controls.Add(btnBrowse);
            Controls.Add(label3);
            Controls.Add(numPort);
            Controls.Add(lblStatus);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnClearLogs);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Name = "MainForm";
            Text = "HL7 Server";
            ((System.ComponentModel.ISupportInitialize)numPort).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnStart;
        private Button btnStop;
        private Button btnClearLogs;
        private Label label1;
        private Label label2;
        private Label lblStatus;
        private NumericUpDown numPort;
        private Label label3;
        private FolderBrowserDialog folderBrowserDialog;
        private Button btnBrowse;
        private TextBox txtfolderPath;
        private Label label4;
        private RichTextBox txtLogs;
    }
}