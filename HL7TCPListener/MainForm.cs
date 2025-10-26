namespace HL7TCPListener
{
    public partial class MainForm : Form
    {
        private UILogger _uiLogger;
        private HL7Server? server;

        public MainForm()
        {
            InitializeComponent();

            _uiLogger = new UILogger();
            _uiLogger.OnLog += UILogger_OnLog;

            btnStop.Enabled = false;
        }

        private void UILogger_OnLog(string message)
        {
            // detect log level
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

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;

            int port = (int)numPort.Value;
            string folder = txtfolderPath.Text;

            try
            {
                server = new HL7Server(_uiLogger, port, folder);

                _uiLogger.Log($"[Information] Starting listener on selected port {port}...");
                _ = Task.Run(() => server.StartAsync());
                lblStatus.Text = "Server Running";
            }
            catch (Exception ex)
            {
                _uiLogger.Log($"[Error] Failed to start listener: {ex.Message}");
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStop.Enabled = false;

            try
            {
                server?.Stop();
                lblStatus.Text = "Server Stopped";
                _uiLogger.Log("[Information] Listener stopped by user.");
            }
            catch (Exception ex)
            {
                _uiLogger.Log($"[Error] Stopping listener: {ex.Message}");
            }

            btnStart.Enabled = true;
        }

        private void btnClearLogs_Click(object sender, EventArgs e)
        {
            txtLogs.Clear();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                txtfolderPath.Text = folderBrowser.SelectedPath;
                _uiLogger.Log($"[Information] Save folder set to: {folderBrowser.SelectedPath}");
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try { server?.Stop(); } catch { }
            base.OnFormClosing(e);
        }
    }
}
