using NHapi.Base.Parser;
using NHapi.Base.Model;
using NHapi.Model.V25.Message;
using NHapi.Model.V25.Segment;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace HL7TCPListener
{
    public class HL7Server
    {
        private readonly UILogger uiLogger;
        private TcpListener? listener;
        private CancellationTokenSource? cts;
        private readonly int port;
        private readonly string? saveFolder;
        private HL7Validator? validator;

        public event Action<string, string>? MessageProcessed;
        public event Action<string>? AckSent;

        private const char VT = (char)0x0B;
        private const char FS = (char)0x1C;
        private const char CR = (char)0x0D;

        private readonly PipeParser parser = new PipeParser();

        public bool IsRunning { get; private set; }

        public HL7Server(UILogger logger, int port = 2575, string? saveFolder = null, HL7Validator? validator = null)
        {
            uiLogger = logger;
            this.port = port;
            this.saveFolder = saveFolder;
            this.validator = validator;
        }

        public async Task StartAsync()
        {
            if (IsRunning) return;
            cts = new CancellationTokenSource();
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            IsRunning = true;

            uiLogger.Info($"Listener started on port {port}");

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
                uiLogger.Error($"Listener error: {ex.Message}");
            }
        }

        public void Stop()
        {
            if (!IsRunning) return;
            uiLogger.Info("Stopping listener...");
            cts?.Cancel();
            listener?.Stop();
            IsRunning = false;
            uiLogger.Info("Listener stopped.");
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken token)
        {
            var endpoint = client.Client.RemoteEndPoint?.ToString();
            uiLogger.Info($"Connection opened: {endpoint}");

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
                        reader.Read();
                        inMessage = false;

                        string hl7 = sb.ToString();
                        uiLogger.Info($"Received message ({hl7.Length} bytes)");

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
                uiLogger.Error($"Client error ({endpoint}): {ex.Message}");
            }
            finally
            {
                client.Close();
                uiLogger.Info($"Connection closed: {endpoint}");
            }
        }

        private async Task ProcessMessageAsync(string hl7, StreamWriter writer)
        {
            uiLogger.Info("Parsing message with NHapi...");

            try
            {
                IMessage inbound = parser.Parse(hl7);
                string messageType = GetMessageType(inbound);
                string messageControlId = GetMessageControlId(inbound);
                
                uiLogger.Info($"Parsed {messageType} successfully");

                MessageProcessed?.Invoke(hl7, messageType);

                var (isValid, error) = validator?.Validate(inbound) ?? (true, "");

                if (!isValid)
                {
                    uiLogger.Error($"Validation failed: {error}");
                    await SendAckAsync(writer, inbound, "AE", error);
                    uiLogger.Error("AE ACK sent due to validation failure.");
                    return;
                }

                if (!string.IsNullOrWhiteSpace(saveFolder))
                {
                    try
                    {
                        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        string fileName = $"{timestamp}_{messageControlId}.hl7";
                        string fullPath = Path.Combine(saveFolder, fileName);

                        Directory.CreateDirectory(saveFolder);
                        await File.WriteAllTextAsync(fullPath, hl7);
                        uiLogger.Info($"Message saved to: {fileName}");
                    }
                    catch (Exception ex)
                    {
                        uiLogger.Error($"Could not save message: {ex.Message}");
                    }
                }

                await SendAckAsync(writer, inbound, "AA");
                uiLogger.Info("Structured ACK sent.");
            }
            catch (Exception ex)
            {
                uiLogger.Error($"NHapi parse error: {ex.Message}");

                try
                {
                    IMessage? inbound = null;
                    try { inbound = parser.Parse(hl7); } catch { /* ignore */ }

                    await SendAckAsync(writer, inbound, "AR", ex.Message);
                    uiLogger.Error("AR ACK sent due to parse error.");
                }
                catch
                {
                    uiLogger.Log("[Critical] Could not send AR ACK — fatal parse failure.");
                }
            }
        }

        private IMessage BuildAck(IMessage inbound, string ackCode = "AA", string? errorText = null, string? versionOverride = null)
        {
            string version = versionOverride ?? "2.5";

            try
            {
                if(inbound != null)
                {
                    var mshIn = (MSH)inbound.GetStructure("MSH");
                    version = mshIn.VersionID.VersionID.Value ?? version;
                }
            }
            catch { /* ignore */ }

            IMessage ack = version switch
            {
                "2.3" => new NHapi.Model.V23.Message.ACK(),
                "2.3.1" => new NHapi.Model.V231.Message.ACK(),
                "2.4" => new NHapi.Model.V24.Message.ACK(),
                "2.5" => new NHapi.Model.V25.Message.ACK(),
                "2.5.1" => new NHapi.Model.V251.Message.ACK(),
                "2.6" => new NHapi.Model.V26.Message.ACK(),
                _ => new NHapi.Model.V25.Message.ACK()
            };

            try
            {
                if (inbound != null)
                {
                    var mshIn = (MSH)inbound.GetStructure("MSH");
                    var mshOut = (MSH)ack.GetStructure("MSH");
                    var msaOut = (MSA)ack.GetStructure("MSA");

                    // MSH
                    mshOut.FieldSeparator.Value = "|";
                    mshOut.EncodingCharacters.Value = "^~\\&";
                    mshOut.SendingApplication.NamespaceID.Value = mshIn.ReceivingApplication.NamespaceID.Value;
                    mshOut.SendingFacility.NamespaceID.Value = mshIn.ReceivingFacility.NamespaceID.Value;
                    mshOut.ReceivingApplication.NamespaceID.Value = mshIn.SendingApplication.NamespaceID.Value;
                    mshOut.ReceivingFacility.NamespaceID.Value = mshIn.SendingFacility.NamespaceID.Value;
                    mshOut.DateTimeOfMessage.Time.Value = DateTime.Now.ToString("yyyyMMddHHmmss");
                    mshOut.MessageType.MessageCode.Value = "ACK";
                    mshOut.MessageType.TriggerEvent.Value = mshIn.MessageType.TriggerEvent.Value;
                    mshOut.MessageControlID.Value = mshIn.MessageControlID.Value;
                    mshOut.ProcessingID.ProcessingID.Value = "P";
                    mshOut.VersionID.VersionID.Value = version;

                    // MSA
                    msaOut.AcknowledgmentCode.Value = ackCode;
                    msaOut.MessageControlID.Value = mshIn.MessageControlID.Value;
                    if (!string.IsNullOrWhiteSpace(errorText))
                        msaOut.TextMessage.Value = errorText;
                }
                else
                {
                    var msaOut = (MSA)ack.GetStructure("MSA");
                    msaOut.AcknowledgmentCode.Value = ackCode;
                    msaOut.TextMessage.Value = errorText ?? "No inbound message available";
                }
            }
            catch (Exception ex)
            {
                var msaOut = (MSA)ack.GetStructure("MSA");
                msaOut.AcknowledgmentCode.Value = "AR";
                msaOut.TextMessage.Value = $"ACK generation error: {ex.Message}";
            }

            return ack;
        }

        public void UpdateValidator(HL7Validator newValidator)
        {
            validator = newValidator;
        }

        private async Task SendAckAsync(StreamWriter writer, IMessage? inbound, string ackCode, string? errorText = null)
        {
            try
            {
                IMessage ack = BuildAck(inbound, ackCode, errorText);
                string ackText = parser.Encode(ack);

                await writer.WriteAsync($"{VT}{ackText}{FS}{CR}");

                AckSent?.Invoke(ackText);

                if (!string.IsNullOrWhiteSpace(saveFolder))
                {
                    string controlId = "UNKNOWN";

                    try
                    {
                        if(inbound != null)
                        {
                            var msh = (MSH)inbound.GetStructure("MSH");
                            controlId = msh.MessageControlID.Value ?? "UNKNOWN";
                            uiLogger.Warn("Message Control ID not extracted from message");
                        }
                    }
                    catch { }

                    string ackPath = Path.Combine(saveFolder, $"{DateTime.Now:yyyyMMdd_HHmmss}_{controlId}_{ackCode}_ACK.hl7");

                    await File.WriteAllTextAsync(ackPath, ackText);
                    uiLogger.Info($"ACK saved to: {ackPath}");
                }

            }
            catch (Exception ex)
            {
                uiLogger.Error($"Failed to build or send ACK: {ex.Message}");
            }
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
