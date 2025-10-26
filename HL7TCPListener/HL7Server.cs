using NHapi.Base.Parser;
using NHapi.Base.Model;
using NHapi.Model.V25.Message;
using NHapi.Model.V25.Segment;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HL7TCPListener
{
    public class HL7Server
    {
        private readonly UILogger uiLogger;
        private TcpListener? listener;
        private CancellationTokenSource? cts;
        private readonly int port;
        private readonly string? saveFolder;

        private const char VT = (char)0x0B;
        private const char FS = (char)0x1C;
        private const char CR = (char)0x0D;

        private readonly PipeParser parser = new PipeParser();

        public bool IsRunning { get; private set; }

        public HL7Server(UILogger logger, int port = 2575, string? saveFolder = null)
        {
            uiLogger = logger;
            this.port = port;
            this.saveFolder = saveFolder;
        }

        public async Task StartAsync()
        {
            if (IsRunning) return;
            cts = new CancellationTokenSource();
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            IsRunning = true;

            uiLogger.Log($"[Information] Listener started on port {port}");

            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    var client = await listener.AcceptTcpClientAsync(cts.Token);
                    _ = HandleClientAsync(client, cts.Token);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                uiLogger.Log($"[Error] Listener error: {ex.Message}");
            }
        }

        public void Stop()
        {
            if (!IsRunning) return;
            uiLogger.Log("[Information] Stopping listener...");
            cts?.Cancel();
            listener?.Stop();
            IsRunning = false;
            uiLogger.Log("[Information] Listener stopped.");
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken token)
        {
            var endpoint = client.Client.RemoteEndPoint?.ToString();
            uiLogger.Log($"[Information] Connection opened: {endpoint}");

            try
            {
                using var stream = client.GetStream();
                using var reader = new StreamReader(stream, Encoding.ASCII);
                using var writer = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true };

                var sb = new StringBuilder();
                int nextChar;
                bool inMessage = false;

                while (!token.IsCancellationRequested && (nextChar = reader.Read()) != -1)
                {
                    char c = (char)nextChar;

                    if (c == VT)
                    {
                        sb.Clear();
                        inMessage = true;
                        continue;
                    }

                    if (c == FS)
                    {
                        reader.Read(); // consume CR
                        inMessage = false;

                        string hl7 = sb.ToString();
                        uiLogger.Log($"[Information] Received message ({hl7.Length} bytes)");

                        await ProcessMessageAsync(hl7, writer);
                        sb.Clear();
                        continue;
                    }

                    if (inMessage)
                        sb.Append(c);
                }
            }
            catch (Exception ex)
            {
                uiLogger.Log($"[Error] Client error ({endpoint}): {ex.Message}");
            }
            finally
            {
                client.Close();
                uiLogger.Log($"[Information] Connection closed: {endpoint}");
            }
        }

        private async Task ProcessMessageAsync(string hl7, StreamWriter writer)
        {
            uiLogger.Log("[Information] Parsing message with NHapi...");

            try
            {
                IMessage inbound = parser.Parse(hl7);
                string messageType = GetMessageType(inbound);
                string messageControlId = GetMessageControlId(inbound);

                uiLogger.Log($"[Information] Parsed {messageType} successfully");

                if (!string.IsNullOrWhiteSpace(saveFolder))
                {
                    try
                    {
                        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        string fileName = $"{timestamp}_{messageControlId}.hl7";
                        string fullPath = Path.Combine(saveFolder, fileName);

                        Directory.CreateDirectory(saveFolder); // just in case
                        await File.WriteAllTextAsync(fullPath, hl7);
                        uiLogger.Log($"[Information] Message saved to: {fileName}");
                    }
                    catch (Exception ex)
                    {
                        uiLogger.Log($"[Error] Could not save message: {ex.Message}");
                    }
                }

                IMessage ack = BuildAck(inbound);
                string ackText = parser.Encode(ack);

                await writer.WriteAsync($"{VT}{ackText}{FS}{CR}");
                uiLogger.Log("[Information] Structured ACK sent.");
            }
            catch (Exception ex)
            {
                uiLogger.Log($"[Error] NHapi parse error: {ex.Message}");
            }
        }

        private IMessage BuildAck(IMessage inbound)
        {
            // Use NHapi’s built-in ACK model
            var ack = new ACK();

            // Get fields from original message
            var mshIn = (MSH)inbound.GetStructure("MSH");
            var mshOut = ack.MSH;
            var msaOut = ack.MSA;

            // Populate MSH
            mshOut.FieldSeparator.Value = "|";
            mshOut.EncodingCharacters.Value = "^~\\&";
            mshOut.SendingApplication.NamespaceID.Value = mshIn.ReceivingApplication.NamespaceID.Value;
            mshOut.SendingFacility.NamespaceID.Value = mshIn.ReceivingApplication.NamespaceID.Value;           
            mshOut.ReceivingApplication.NamespaceID.Value = mshIn.SendingApplication.NamespaceID.Value;
            mshOut.ReceivingFacility.NamespaceID.Value = mshIn.SendingFacility.NamespaceID.Value;
            mshOut.DateTimeOfMessage.Time.Value = DateTime.Now.ToString("yyyyMMddHHmmss");
            mshOut.MessageType.MessageCode.Value = "ACK";
            mshOut.MessageType.TriggerEvent.Value = mshIn.MessageType.TriggerEvent.Value;
            mshOut.MessageControlID.Value = mshIn.MessageControlID.Value;
            mshOut.ProcessingID.ProcessingID.Value = "P";
            mshOut.VersionID.VersionID.Value = mshIn.VersionID.VersionID.Value;

            // Populate MSA
            msaOut.AcknowledgmentCode.Value = "AA";
            msaOut.MessageControlID.Value = mshIn.MessageControlID.Value;

            return ack;
        }

        private static string GetMessageControlId(IMessage msg)
        {
            try
            {
                var msh = (MSH)msg.GetStructure("MSH");
                return $"{msh.SequenceNumber.Value}";
            }
            catch
            {
                return "MSGUNKNOWN";
            }
        }

        private static string GetMessageType(IMessage msg)
        {
            try
            {
                var msh = (MSH)msg.GetStructure("MSH");
                return $"{msh.MessageType.MessageCode.Value}^{msh.MessageType.TriggerEvent.Value}";
            }
            catch
            {
                return "UNKNOWN";
            }
        }
    }
}
