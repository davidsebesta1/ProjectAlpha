using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace ProjectAlpha.Yaml
{
    /// <summary>
    /// Custom yaml parser for <see cref="DateTime"/>.
    /// </summary>
    public class CustomDateTimeConverter : IYamlTypeConverter
    {
        public static string DateFormat => "dd/MM/yyyy HH:mm";

        /// <inheritdoc/>
        public bool Accepts(Type type)
        {
            return type == typeof(DateTime) || type == typeof(DateTime?);
        }

        /// <inheritdoc/>
        public object ReadYaml(IParser parser, Type type, ObjectDeserializer deserializer)
        {
            Scalar scalar = parser.Consume<Scalar>();
            if (DateTime.TryParseExact(scalar.Value, DateFormat, null, System.Globalization.DateTimeStyles.None, out var dateTime))
                return dateTime;

            throw new FormatException($"Invalid date format: {scalar.Value}");
        }

        /// <inheritdoc/>
        public void WriteYaml(IEmitter emitter, object value, Type type, ObjectSerializer serializer)
        {
            DateTime dateTime = (DateTime)value;
            emitter.Emit(new Scalar(dateTime.ToString(DateFormat)));
        }
    }
}
