using System.Text.Json;
using System.Text.Json.Serialization;

namespace PRN231_2_EventFlowerExchange_FE.Service
{
    public class JsonStringEnumConverterWithDefault<T> : JsonConverter<T> where T : struct, Enum
    {
        private readonly T _defaultValue;

        public JsonStringEnumConverterWithDefault(T defaultValue)
        {
            _defaultValue = defaultValue;
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string enumString = reader.GetString();
                if (Enum.TryParse(enumString, ignoreCase: true, out T value))
                {
                    return value;
                }
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                int enumValue = reader.GetInt32();
                if (Enum.IsDefined(typeof(T), enumValue))
                {
                    return (T)Enum.ToObject(typeof(T), enumValue);
                }
            }
            return _defaultValue; // Return default value if conversion fails
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}