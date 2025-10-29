using System.Diagnostics;
using System.Text.Json;

namespace HL7TCPListener
{
    public partial class MainForm : Form
    {
        private UILogger _uiLogger;
        private HL7Server? server;
        private NotifyIcon trayIcon;
        public HL7Schema? schema = null;
        public HL7Validator? validator = null;
        private readonly System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private bool _isExit = false;
        private int _totalMessages = 0;

        public MainForm()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.app_icon;

            _uiLogger = new UILogger();
            _uiLogger.OnLog += UILogger_OnLog;

            var config = LoadConfig();
            numPort.Value = config.Port;
            txtfolderPath.Text = config.FolderPath;

            btnCopyLastMessage.Enabled = false;
            btnCopyAckMessage.Enabled = false;

            LoadSchema();
            InitializeTrayIcon();
            StyleButtons();
        }

        private void StyleButtons()
        {
            btnStop.FlatStyle = FlatStyle.Flat;
            btnStop.UseVisualStyleBackColor = false;
            btnStop.BackColor = Color.Firebrick;
            btnStop.ForeColor = Color.White;
            btnStop.FlatAppearance.BorderSize = 0;

            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.UseVisualStyleBackColor = false;
            btnStart.BackColor = Color.MediumSeaGreen;
            btnStart.ForeColor = Color.White;
            btnStart.FlatAppearance.BorderSize = 0;
        }

        private void InitializeTrayIcon()
        {
            trayIcon = new NotifyIcon
            {
                Icon = Properties.Resources.hl7_helix_stopped1,
                Visible = true,
                Text = "HL7 Server - Stopped"
            };

            trayIcon.DoubleClick += (s, e) =>
            {
                Show();
                WindowState = FormWindowState.Normal;
                Activate();
            };

            var contextMenu = new ContextMenuStrip();
            var startItem = new ToolStripMenuItem("Start", null, (s, e) =>
            {
                if (InvokeRequired)
                    BeginInvoke(new Action(StartServer));
                else
                    StartServer();
            });

            var stopItem = new ToolStripMenuItem("Stop", null, (s, e) =>
            {
                if (InvokeRequired)
                    BeginInvoke(new Action(StopServer));
                else
                    StopServer();
            });

            var openItem = new ToolStripMenuItem("Open", null, (s, e) =>
            {
                Show();
                WindowState = FormWindowState.Normal;
                Activate();
            });
            var exitItem = new ToolStripMenuItem("Exit", null, (s, e) =>
            {
                _isExit = true;
                Close();
            });

            contextMenu.Items.Add(startItem);
            contextMenu.Items.Add(stopItem);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(openItem);
            contextMenu.Items.Add(exitItem);

            trayIcon.ContextMenuStrip = contextMenu;

            trayIcon.ContextMenuStrip.Opening += (s, e) =>
            {
                UpdateTrayMenuState(server?.IsRunning ?? false);
            };
        }

        private void UILogger_OnLog(string message)
        {
            Color color = Color.Black;
            if (message.Contains("Error", StringComparison.OrdinalIgnoreCase)) color = Color.IndianRed;
            else if (message.Contains("Warning", StringComparison.OrdinalIgnoreCase)) color = Color.Gold;
            else if (message.Contains("Information", StringComparison.OrdinalIgnoreCase)) color = Color.DarkGreen;
            else if (message.Contains("Debug", StringComparison.OrdinalIgnoreCase)) color = Color.DodgerBlue;

            AppendColoredText(message + Environment.NewLine, color);
        }

        private void AppendColoredText(string text, Color color)
        {
            if (txtLogs.InvokeRequired)
            {
                txtLogs.BeginInvoke(new Action(() => AppendColoredText(text, color)));
                return;
            }

            txtLogs.SelectionStart = txtLogs.TextLength;
            txtLogs.SelectionColor = color;
            txtLogs.AppendText(text);
            txtLogs.SelectionColor = txtLogs.ForeColor;
            txtLogs.ScrollToCaret();
        }

        private void LoadSchema()
        {
            var schemaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hl7_schema.json");

            if (File.Exists(schemaPath))
            {
                try
                {
                    schema = HL7Schema.Load(schemaPath);
                    validator = new HL7Validator(schema, _uiLogger);
                    _uiLogger.Debug($"Loaded HL7 schema from {schemaPath}");
                    UpdateSchemaStatus(true, $"Loaded from {schemaPath}");
                }
                catch (Exception ex)
                {
                    _uiLogger.Warn($"Failed to load HL7 schema: {ex.Message}");
                    UpdateSchemaStatus(false, $"Schema load failed");
                }
            }
            else
            {
                _uiLogger.Error("No HL7 schema file found - see help.");
                MessageBox.Show("No HL7 schema file found", "Missing File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateSchemaStatus(false, $"Schema load failed");
                return;
            }
        }

        #region Server Control

        private void StartServer()
        {
            if (string.IsNullOrWhiteSpace(txtfolderPath.Text.Trim()))
            {
                try
                {
                    var outDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    outDir = Path.Combine(outDir, "HL7Inbox");
                    txtfolderPath.Text = outDir;
                    _uiLogger.Info("Output Directory Not Specificed, defaulting to fallack");
                }
                catch (Exception ex)
                {
                    _uiLogger.Error($"Error setting output directory {ex}");
                    return;
                }
            }

            btnStart.Enabled = false;
            btnStop.Enabled = true;
            UpdateTrayMenuState(true);

            int port = (int)numPort.Value;
            string folder = txtfolderPath.Text;

            try
            {
                server = new HL7Server(_uiLogger, port, folder, validator);

                server.MessageProcessed += OnMessageProcessed;
                server.AckSent += OnAckSent;

                _uiLogger.Info($"Starting listener on selected port {port}...");
                _ = Task.Run(() => server.StartAsync());
                trayIcon.Icon = Properties.Resources.hl7_helix_running1;
                trayIcon.Text = $"HL7 Server – Running (Port {port})";
                _uiLogger.Info($"Listening on port {port}...");

                _timer.Interval = 100;
                _timer.Tick += Timer1_Tick;
                _stopwatch.Start();
                UpdateUptime();
                _timer.Start();

                UpdateServerStatus(true, "Server Running");
            }
            catch (Exception ex)
            {
                _uiLogger.Error($"Failed to start listener: {ex.Message}");
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            }
        }

        private void StopServer()
        {
            btnStop.Enabled = false;
            UpdateTrayMenuState(false);

            try
            {
                server?.Stop();
                trayIcon.Icon = Properties.Resources.hl7_helix_stopped1;
                trayIcon.Text = "HL7 Server – Stopped";
                UpdateServerStatus(false, "Server Stopped");
                _uiLogger.Info("Listener stopped by user.");
                _timer.Stop();
                _stopwatch.Reset();
                lblUptime.Text = "00:00:00";
            }
            catch (Exception ex)
            {
                _uiLogger.Error($"Stopping listener: {ex.Message}");
            }

            btnStart.Enabled = true;
        }

        private void UpdateServerStatus(bool loaded, string? message = null)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateServerStatus(loaded, message)));
                return;
            }

            if (loaded)
            {
                pnlServerStatus.BackColor = Color.MediumSeaGreen;
                lblStatus.Text = "Server Running";
                toolTip2.SetToolTip(pnlServerStatus, message ?? "Running.");
            }
            else
            {
                pnlServerStatus.BackColor = Color.DarkRed;
                lblStatus.Text = "Server Stopped";
                toolTip2.SetToolTip(pnlServerStatus, message ?? "Not Running.");
            }
        }

        private void OnMessageProcessed(string message, string messageType)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnMessageProcessed(message, messageType)));
                return;
            }

            txtMessageReceived.Text = message;
            HighlightHL7Delimiters(txtMessageReceived);
            _totalMessages++;
            lblTotalMessages.Text = _totalMessages.ToString();
            btnCopyLastMessage.Enabled = true;

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            UpdateUptime();
        }

        private void UpdateUptime()
        {
            lblUptime.Text = $"{_stopwatch.Elapsed:hh\\:mm\\:ss}";
        }

        private void OnAckSent(string ack)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnAckSent(ack)));
                return;
            }

            txtAckSent.Text = ack;
            HighlightHL7Delimiters(txtAckSent);
            btnCopyAckMessage.Enabled = true;
        }

        private void UpdateTrayMenuState(bool isRunning)
        {
            foreach (ToolStripItem item in trayIcon.ContextMenuStrip.Items)
            {
                if (item is ToolStripMenuItem menuItem)
                {
                    if (menuItem.Text == "Start")
                        menuItem.Enabled = !isRunning;
                    else if (menuItem.Text == "Stop")
                        menuItem.Enabled = isRunning;
                }
            }
        }

        #endregion

        private void btnStart_Click(object sender, EventArgs e) => StartServer();

        private void btnStop_Click(object sender, EventArgs e) => StopServer();

        private void btnClearLogs_Click(object sender, EventArgs e)
        {
            txtLogs.Clear();
            MessageBox.Show("Logs Cleared", "Log Cleardown", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                txtfolderPath.Text = folderBrowser.SelectedPath;
                _uiLogger.Info($"Save folder set to: {folderBrowser.SelectedPath}");
            }
        }

        private string GetConfigPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
        }

        private AppConfig LoadConfig()
        {
            try
            {
                string path = GetConfigPath();
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    _uiLogger.Debug($"Application Config Loaded from {path}");
                    return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
                }
            }
            catch (Exception ex)
            {
                _uiLogger.Error($"Failed to load config: {ex.Message}");
            }
            return new AppConfig();
        }

        private void SaveConfig(AppConfig config)
        {
            try
            {
                string path = GetConfigPath();
                string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json);
                _uiLogger.Info("Configuration saved.");
            }
            catch (Exception ex)
            {
                _uiLogger.Error($"Failed to save config: {ex.Message}");
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!_isExit)
            {
                e.Cancel = true;
                Hide();

                // Save config
                var config = new AppConfig
                {
                    Port = (int)numPort.Value,
                    FolderPath = txtfolderPath.Text.Trim()
                };
                SaveConfig(config);

                return;
            }

            // Stop background stuff
            try
            {
                timer1?.Stop();
                timer1?.Dispose();

                server?.Stop();
                trayIcon.Visible = false;
                trayIcon.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cleanup error: {ex.Message}");
            }

            base.OnFormClosing(e);
        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                trayIcon.BalloonTipTitle = "HL7 Server";
                trayIcon.BalloonTipText = "The listener is still running in the background.";
                trayIcon.ShowBalloonTip(1500);
            }
        }

        private void btnCopyLogs_Click(object sender, EventArgs e)
        {
            if (txtLogs.Text.Length > 0)
            {
                Clipboard.SetText(txtLogs.Text);
                MessageBox.Show("Logs copied to clipboard", "Copy logs", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No logs to copy", "Copy logs", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnReloadSchema_Click(object sender, EventArgs e)
        {
            var schemaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hl7_schema.json");

            if (!File.Exists(schemaPath))
            {
                _uiLogger.Warn("Schema file not found.");
                return;
            }

            try
            {
                var newSchema = HL7Schema.Load(schemaPath);
                var newValidator = new HL7Validator(newSchema, _uiLogger);

                schema = newSchema;
                validator = newValidator;

                if (server != null)
                {
                    server.UpdateValidator(newValidator);
                }

                _uiLogger.Info("Schema reloaded successfully.");
                UpdateSchemaStatus(true, "Schema reloaded successfully.");
            }
            catch (Exception ex)
            {
                _uiLogger.Error($"Failed to reload schema: {ex.Message}");
                UpdateSchemaStatus(false, "Schema reload failed");
            }
        }

        private void UpdateSchemaStatus(bool loaded, string? message = null)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateSchemaStatus(loaded, message)));
                return;
            }

            if (loaded)
            {
                pnlSchemaStatus.BackColor = Color.MediumSeaGreen;
                lblSchemaStatus.Text = "Schema loaded";
                toolTip1.SetToolTip(pnlSchemaStatus, message ?? "HL7 validation is active.");
            }
            else
            {
                pnlSchemaStatus.BackColor = Color.DarkRed;
                lblSchemaStatus.Text = "No schema loaded";
                toolTip1.SetToolTip(pnlSchemaStatus, message ?? "Messages will not be validated.");
            }
        }

        private void pnlSchemaStatus_Click(object sender, EventArgs e)
        {
            if (File.Exists("hl7_schema.json"))
            {
                System.Diagnostics.Process.Start("explorer.exe", "/select,\"hl7_schema.json\"");
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Application.MessageLoop)
            {
                Application.Exit();
            }
            else
            {
                System.Environment.Exit(1);
            }
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = "https://github.com/rich-howell/HL7Server",
                UseShellExecute = true
            };

            Process.Start(info);
        }

        private void HighlightHL7Delimiters(RichTextBox box)
        {
            if (string.IsNullOrWhiteSpace(box.Text))
                return;

            int originalStart = box.SelectionStart;
            int originalLength = box.SelectionLength;

            box.SuspendLayout();
            box.SelectAll();
            box.SelectionColor = Color.Black;

            foreach (var i in Enumerable.Range(0, box.TextLength))
            {
                char c = box.Text[i];
                Color? color = c switch
                {
                    '|' => Color.FromArgb(0, 160, 120),
                    '^' => Color.FromArgb(180, 60, 60),
                    '~' => Color.FromArgb(100, 100, 200),
                    '\\' => Color.FromArgb(90, 130, 180),
                    _ => null
                };

                if (color.HasValue)
                {
                    box.Select(i, 1);
                    box.SelectionColor = color.Value;
                }
            }

            box.SelectionStart = originalStart;
            box.SelectionLength = originalLength;
            box.ResumeLayout();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = "https://github.com/rich-howell/HL7Server",
                UseShellExecute = true
            };

            Process.Start(info);
        }

        private void btnCopyLastMessage_Click(object sender, EventArgs e)
        {
            if (txtMessageReceived.Text.Length > 0)
            {
                Clipboard.SetText(txtMessageReceived.Text);
                MessageBox.Show("Message copied to clipboard", "Copy Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No Message to copy", "Copy Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCopyAckMessage_Click(object sender, EventArgs e)
        {
            if (txtAckSent.Text.Length > 0)
            {
                Clipboard.SetText(txtAckSent.Text);
                MessageBox.Show("ACK copied to clipboard", "Copy ACK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No ACK to copy", "Copy MessACKage", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnClearLastMessage_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMessageReceived.Text))
            {
                txtMessageReceived.Clear();
            }
        }

        private void btnClearAckMessage_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtAckSent.Text))
            {
                txtAckSent.Clear();
            }
        }

        private void messageStoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtfolderPath.Text.Trim()))
            {
                Process.Start("explorer.exe", txtfolderPath.Text.Trim());
            }
        }

        private void schemaConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists("hl7_schema.json"))
            {
                Process.Start("explorer.exe", "/select,\"hl7_schema.json\"");
            }
        }

        private void appSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists("appsettings.json"))
            {
                Process.Start("explorer.exe", "/select,\"appsettings.json\"");
            }
        }
    }
}
