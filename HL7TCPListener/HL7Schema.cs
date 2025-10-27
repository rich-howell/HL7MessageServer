using System.Text.Json;

namespace HL7TCPListener
{
    public class HL7Schema
    {
        public Dictionary<string, HL7Version> Versions { get; set; } = new();

        public static HL7Schema Load(string path)
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<HL7Schema>(json)!;
        }

        public HL7MessageSchema? GetMessageSchema(string version, string messageType)
        {
            if (Versions.TryGetValue(version, out var v))
            {
                if (v.Messages.TryGetValue(messageType, out var msg))
                    return msg;
            }
            return null;
        }
    }

    public class HL7Version
    {
        public Dictionary<string, HL7MessageSchema> Messages { get; set; } = new();
    }

    public class HL7MessageSchema
    {
        public string Description { get; set; } = "";
        public Dictionary<string, HL7SegmentSchema> Segments { get; set; } = new();
    }

    public class HL7SegmentSchema
    {
        public bool Required { get; set; }
        public Dictionary<string, HL7FieldSchema> Fields { get; set; } = new();
    }

    public class HL7FieldSchema
    {
        public int Position { get; set; }
        public bool Required { get; set; }
        public string? DataType { get; set; }
    }
}

