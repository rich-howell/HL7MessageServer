using NHapi.Base.Model;
using NHapi.Base.Util;
using System.Text;

namespace HL7TCPListener
{
    public class HL7Validator
    {
        private readonly HL7Schema _schema;
        private readonly UILogger _logger;

        public HL7Validator(HL7Schema schema, UILogger logger)
        {
            _schema = schema;
            _logger = logger;
        }

        public (bool IsValid, string Error) Validate(IMessage message)
        {
            if (message == null)
                return (false, "Message is null and cannot be validated.");

            var terser = new Terser(message);
            string version = terser.Get("/MSH-12") ?? "2.5";
            string messageCode = terser.Get("/MSH-9-1");
            string triggerEvent = terser.Get("/MSH-9-2");
            string msgType = $"{messageCode}^{triggerEvent}";

            var msgSchema = _schema.GetMessageSchema(version, msgType);
            if (msgSchema == null)
                return (false, $"Schema not found for {msgType} (version {version})");

            foreach (var (segName, segSchema) in msgSchema.Segments)
            {
                var segmentCount = message.GetAll(segName)?.Length ?? 0;
                if (segmentCount == 0)
                {
                    if (segSchema.Required)
                        return (false, $"Missing required segment: {segName}");
                    continue;
                }

                foreach (var (fieldName, fieldSchema) in segSchema.Fields)
                {
                    string path = $"{segName}-{fieldSchema.Position}";
                    var value = terser.Get(path);

                    if (fieldSchema.Required && string.IsNullOrWhiteSpace(value))
                        return (false, $"Missing required field {segName}-{fieldSchema.Position} ({fieldName})");
                }
            }

            return (true, "");
        }
    }
}

