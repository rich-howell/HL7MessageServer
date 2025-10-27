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
            components = new System.ComponentModel.Container();
            btnStart = new Button();
            btnStop = new Button();
            btnClearLogs = new Button();
            lblStatus = new Label();
            numPort = new NumericUpDown();
            label3 = new Label();
            folderBrowserDialog = new FolderBrowserDialog();
            btnBrowse = new Button();
            txtfolderPath = new TextBox();
            label4 = new Label();
            txtLogs = new RichTextBox();
            btnCopyLogs = new Button();
            btnReloadSchema = new Button();
            pnlSchemaStatus = new Panel();
            lblSchemaStatus = new Label();
            toolTip1 = new ToolTip(components);
            pnlServerStatus = new Panel();
            toolTip2 = new ToolTip(components);
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            notifyIcon = new NotifyIcon(components);
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            messageStoreToolStripMenuItem = new ToolStripMenuItem();
            schemaConfigToolStripMenuItem = new ToolStripMenuItem();
            appSettingsToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem1 = new ToolStripMenuItem();
            groupBox3 = new GroupBox();
            btnClearLastMessage = new Button();
            btnCopyLastMessage = new Button();
            txtMessageReceived = new RichTextBox();
            groupBox4 = new GroupBox();
            btnClearAckMessage = new Button();
            btnCopyAckMessage = new Button();
            txtAckSent = new RichTextBox();
            label1 = new Label();
            label2 = new Label();
            label5 = new Label();
            lblTotalMessages = new Label();
            lblUptime = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)numPort).BeginInit();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            menuStrip1.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Location = new Point(7, 74);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(75, 23);
            btnStart.TabIndex = 0;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(88, 74);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(75, 23);
            btnStop.TabIndex = 1;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = false;
            btnStop.Click += btnStop_Click;
            // 
            // btnClearLogs
            // 
            btnClearLogs.Location = new Point(356, 74);
            btnClearLogs.Name = "btnClearLogs";
            btnClearLogs.Size = new Size(75, 23);
            btnClearLogs.TabIndex = 3;
            btnClearLogs.Text = "Clear Logs";
            btnClearLogs.UseVisualStyleBackColor = true;
            btnClearLogs.Click += btnClearLogs_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(41, 477);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(75, 15);
            lblStatus.TabIndex = 6;
            lblStatus.Text = "Not Running";
            // 
            // numPort
            // 
            numPort.Location = new Point(7, 45);
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
            label3.Location = new Point(9, 25);
            label3.Name = "label3";
            label3.Size = new Size(84, 15);
            label3.TabIndex = 8;
            label3.Text = "Listening Port";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new Point(356, 45);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(75, 23);
            btnBrowse.TabIndex = 9;
            btnBrowse.Text = "Browse";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // txtfolderPath
            // 
            txtfolderPath.Location = new Point(140, 45);
            txtfolderPath.Name = "txtfolderPath";
            txtfolderPath.Size = new Size(212, 23);
            txtfolderPath.TabIndex = 10;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(140, 25);
            label4.Name = "label4";
            label4.Size = new Size(84, 15);
            label4.TabIndex = 11;
            label4.Text = "Save Location";
            // 
            // txtLogs
            // 
            txtLogs.Location = new Point(6, 22);
            txtLogs.Name = "txtLogs";
            txtLogs.Size = new Size(423, 261);
            txtLogs.TabIndex = 12;
            txtLogs.Text = "";
            // 
            // btnCopyLogs
            // 
            btnCopyLogs.Location = new Point(275, 74);
            btnCopyLogs.Name = "btnCopyLogs";
            btnCopyLogs.Size = new Size(75, 23);
            btnCopyLogs.TabIndex = 13;
            btnCopyLogs.Text = "Copy Logs";
            btnCopyLogs.UseVisualStyleBackColor = true;
            btnCopyLogs.Click += btnCopyLogs_Click;
            // 
            // btnReloadSchema
            // 
            btnReloadSchema.Location = new Point(169, 74);
            btnReloadSchema.Name = "btnReloadSchema";
            btnReloadSchema.Size = new Size(100, 23);
            btnReloadSchema.TabIndex = 14;
            btnReloadSchema.Text = "Reload Schema";
            btnReloadSchema.UseVisualStyleBackColor = true;
            btnReloadSchema.Click += btnReloadSchema_Click;
            // 
            // pnlSchemaStatus
            // 
            pnlSchemaStatus.BackColor = Color.DarkRed;
            pnlSchemaStatus.BorderStyle = BorderStyle.FixedSingle;
            pnlSchemaStatus.Location = new Point(16, 448);
            pnlSchemaStatus.Name = "pnlSchemaStatus";
            pnlSchemaStatus.Size = new Size(20, 20);
            pnlSchemaStatus.TabIndex = 15;
            pnlSchemaStatus.Click += pnlSchemaStatus_Click;
            // 
            // lblSchemaStatus
            // 
            lblSchemaStatus.AutoSize = true;
            lblSchemaStatus.Location = new Point(41, 451);
            lblSchemaStatus.Name = "lblSchemaStatus";
            lblSchemaStatus.Size = new Size(106, 15);
            lblSchemaStatus.TabIndex = 16;
            lblSchemaStatus.Text = "No schema loaded";
            // 
            // pnlServerStatus
            // 
            pnlServerStatus.BackColor = Color.DarkRed;
            pnlServerStatus.BorderStyle = BorderStyle.FixedSingle;
            pnlServerStatus.Location = new Point(16, 475);
            pnlServerStatus.Name = "pnlServerStatus";
            pnlServerStatus.Size = new Size(20, 20);
            pnlServerStatus.TabIndex = 17;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(txtLogs);
            groupBox1.Location = new Point(16, 30);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(435, 294);
            groupBox1.TabIndex = 18;
            groupBox1.TabStop = false;
            groupBox1.Text = "Logs";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(btnStart);
            groupBox2.Controls.Add(btnStop);
            groupBox2.Controls.Add(btnReloadSchema);
            groupBox2.Controls.Add(btnCopyLogs);
            groupBox2.Controls.Add(btnClearLogs);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(numPort);
            groupBox2.Controls.Add(btnBrowse);
            groupBox2.Controls.Add(txtfolderPath);
            groupBox2.Location = new Point(16, 330);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(438, 109);
            groupBox2.TabIndex = 19;
            groupBox2.TabStop = false;
            groupBox2.Text = "Controls";
            // 
            // notifyIcon
            // 
            notifyIcon.Text = "notifyIcon";
            notifyIcon.Visible = true;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, viewToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(933, 24);
            menuStrip1.TabIndex = 20;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(92, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { messageStoreToolStripMenuItem, schemaConfigToolStripMenuItem, appSettingsToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(44, 20);
            viewToolStripMenuItem.Text = "View";
            // 
            // messageStoreToolStripMenuItem
            // 
            messageStoreToolStripMenuItem.Name = "messageStoreToolStripMenuItem";
            messageStoreToolStripMenuItem.Size = new Size(155, 22);
            messageStoreToolStripMenuItem.Text = "Message Store";
            messageStoreToolStripMenuItem.Click += messageStoreToolStripMenuItem_Click;
            // 
            // schemaConfigToolStripMenuItem
            // 
            schemaConfigToolStripMenuItem.Name = "schemaConfigToolStripMenuItem";
            schemaConfigToolStripMenuItem.Size = new Size(155, 22);
            schemaConfigToolStripMenuItem.Text = "Schema Config";
            schemaConfigToolStripMenuItem.Click += schemaConfigToolStripMenuItem_Click;
            // 
            // appSettingsToolStripMenuItem
            // 
            appSettingsToolStripMenuItem.Name = "appSettingsToolStripMenuItem";
            appSettingsToolStripMenuItem.Size = new Size(155, 22);
            appSettingsToolStripMenuItem.Text = "App Settings";
            appSettingsToolStripMenuItem.Click += appSettingsToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem, helpToolStripMenuItem1 });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(127, 22);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem1
            // 
            helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            helpToolStripMenuItem1.Size = new Size(127, 22);
            helpToolStripMenuItem1.Text = "View Help";
            helpToolStripMenuItem1.Click += helpToolStripMenuItem1_Click;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(btnClearLastMessage);
            groupBox3.Controls.Add(btnCopyLastMessage);
            groupBox3.Controls.Add(txtMessageReceived);
            groupBox3.Location = new Point(481, 30);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(435, 233);
            groupBox3.TabIndex = 21;
            groupBox3.TabStop = false;
            groupBox3.Text = "Last Message Received";
            // 
            // btnClearLastMessage
            // 
            btnClearLastMessage.Location = new Point(263, 204);
            btnClearLastMessage.Name = "btnClearLastMessage";
            btnClearLastMessage.Size = new Size(44, 23);
            btnClearLastMessage.TabIndex = 2;
            btnClearLastMessage.Text = "Clear";
            btnClearLastMessage.UseVisualStyleBackColor = true;
            btnClearLastMessage.Click += btnClearLastMessage_Click;
            // 
            // btnCopyLastMessage
            // 
            btnCopyLastMessage.Location = new Point(313, 204);
            btnCopyLastMessage.Name = "btnCopyLastMessage";
            btnCopyLastMessage.Size = new Size(116, 23);
            btnCopyLastMessage.TabIndex = 1;
            btnCopyLastMessage.Text = "Copy To Clipboard";
            btnCopyLastMessage.UseVisualStyleBackColor = true;
            btnCopyLastMessage.Click += btnCopyLastMessage_Click;
            // 
            // txtMessageReceived
            // 
            txtMessageReceived.Location = new Point(6, 22);
            txtMessageReceived.Name = "txtMessageReceived";
            txtMessageReceived.Size = new Size(423, 176);
            txtMessageReceived.TabIndex = 0;
            txtMessageReceived.Text = "";
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(btnClearAckMessage);
            groupBox4.Controls.Add(btnCopyAckMessage);
            groupBox4.Controls.Add(txtAckSent);
            groupBox4.Location = new Point(481, 269);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(435, 223);
            groupBox4.TabIndex = 22;
            groupBox4.TabStop = false;
            groupBox4.Text = "Last Ack Sent Back";
            // 
            // btnClearAckMessage
            // 
            btnClearAckMessage.Location = new Point(263, 196);
            btnClearAckMessage.Name = "btnClearAckMessage";
            btnClearAckMessage.Size = new Size(44, 23);
            btnClearAckMessage.TabIndex = 3;
            btnClearAckMessage.Text = "Clear";
            btnClearAckMessage.UseVisualStyleBackColor = true;
            btnClearAckMessage.Click += btnClearAckMessage_Click;
            // 
            // btnCopyAckMessage
            // 
            btnCopyAckMessage.Location = new Point(313, 196);
            btnCopyAckMessage.Name = "btnCopyAckMessage";
            btnCopyAckMessage.Size = new Size(116, 23);
            btnCopyAckMessage.TabIndex = 2;
            btnCopyAckMessage.Text = "Copy To Clipboard";
            btnCopyAckMessage.UseVisualStyleBackColor = true;
            btnCopyAckMessage.Click += btnCopyAckMessage_Click;
            // 
            // txtAckSent
            // 
            txtAckSent.Location = new Point(6, 18);
            txtAckSent.Name = "txtAckSent";
            txtAckSent.Size = new Size(423, 176);
            txtAckSent.TabIndex = 0;
            txtAckSent.Text = "";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(295, 448);
            label1.Name = "label1";
            label1.Size = new Size(76, 15);
            label1.TabIndex = 23;
            label1.Text = "Server Stats";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(295, 465);
            label2.Name = "label2";
            label2.Size = new Size(111, 15);
            label2.TabIndex = 24;
            label2.Text = "Messages Received:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(295, 483);
            label5.Name = "label5";
            label5.Size = new Size(78, 15);
            label5.TabIndex = 25;
            label5.Text = "Total Uptime:";
            // 
            // lblTotalMessages
            // 
            lblTotalMessages.AutoSize = true;
            lblTotalMessages.Location = new Point(406, 465);
            lblTotalMessages.Name = "lblTotalMessages";
            lblTotalMessages.Size = new Size(13, 15);
            lblTotalMessages.TabIndex = 26;
            lblTotalMessages.Text = "0";
            // 
            // lblUptime
            // 
            lblUptime.AutoSize = true;
            lblUptime.Location = new Point(372, 483);
            lblUptime.Name = "lblUptime";
            lblUptime.Size = new Size(38, 15);
            lblUptime.TabIndex = 27;
            lblUptime.Text = "label7";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(933, 502);
            Controls.Add(lblUptime);
            Controls.Add(lblTotalMessages);
            Controls.Add(label5);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(pnlServerStatus);
            Controls.Add(lblSchemaStatus);
            Controls.Add(pnlSchemaStatus);
            Controls.Add(lblStatus);
            Controls.Add(menuStrip1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Text = "HL7 Message Server";
            ((System.ComponentModel.ISupportInitialize)numPort).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox4.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnStart;
        private Button btnStop;
        private Button btnClearLogs;
        private Label lblStatus;
        private NumericUpDown numPort;
        private Label label3;
        private FolderBrowserDialog folderBrowserDialog;
        private Button btnBrowse;
        private TextBox txtfolderPath;
        private Label label4;
        private RichTextBox txtLogs;
        private Button btnCopyLogs;
        private Button btnReloadSchema;
        private Panel pnlSchemaStatus;
        private Label lblSchemaStatus;
        private ToolTip toolTip1;
        private Panel pnlServerStatus;
        private ToolTip toolTip2;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private NotifyIcon notifyIcon;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem1;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private RichTextBox txtMessageReceived;
        private RichTextBox txtAckSent;
        private Button btnCopyLastMessage;
        private Button btnCopyAckMessage;
        private Button btnClearLastMessage;
        private Button btnClearAckMessage;
        private Label label1;
        private Label label2;
        private Label label5;
        private Label lblTotalMessages;
        private Label lblUptime;
        private System.Windows.Forms.Timer timer1;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem messageStoreToolStripMenuItem;
        private ToolStripMenuItem schemaConfigToolStripMenuItem;
        private ToolStripMenuItem appSettingsToolStripMenuItem;
    }
}