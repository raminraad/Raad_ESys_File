using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ESys.Api.Profiles.Serializers;

public class DictionaryTKeyStringTValueStringSerializer : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
        {
            return false;
        }

        if (typeToConvert.GetGenericTypeDefinition() != typeof(Dictionary<,>))
        {
            return false;
        }

        return typeToConvert.GetGenericArguments()[0] == typeof(string) &&
               typeToConvert.GetGenericArguments()[1] == typeof(string);
    }

    public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
    {
        Type[] typeArguments = type.GetGenericArguments();
        Type keyType = typeArguments[0];
        Type valueType = typeArguments[1];

        JsonConverter converter = (JsonConverter)Activator.CreateInstance(
            typeof(DictionaryStringStringConverterInner<,>).MakeGenericType([keyType, valueType]),
            BindingFlags.Instance | BindingFlags.Public, binder: null, args: [options], culture: null)!;

        return converter;
    }

    private class DictionaryStringStringConverterInner<TKey, TValue> : JsonConverter<Dictionary<TKey, TValue>>
        where TKey : notnull
    {
        private readonly JsonConverter<TValue> _valueConverter;
        private readonly Type _keyType;
        private readonly Type _valueType;

        public DictionaryStringStringConverterInner(JsonSerializerOptions options)
        {
            // For performance, use the existing converter.
            _valueConverter = (JsonConverter<TValue>)options.GetConverter(typeof(TValue));

            // Cache the key and value types.
            _keyType = typeof(TKey);
            _valueType = typeof(TValue);
        }

        public override Dictionary<TKey, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var dictionary = new Dictionary<TKey, TValue>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return dictionary;
                }

                // Get the key.
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                string? propertyName = reader.GetString();

                // For performance, parse with ignoreCase:false first.
                // if (!Enum.TryParse(propertyName, ignoreCase: false, out TKey key) &&
                //     !Enum.TryParse(propertyName, ignoreCase: true, out key))
                // {
                //     throw new JsonException($"Unable to convert \"{propertyName}\" to Enum \"{_keyType}\".");
                // }
                TKey key = (TKey)Convert.ChangeType(propertyName, typeof(TKey));
 
                // Get the value.
                reader.Read();
                TValue value = _valueConverter.Read(ref reader, _valueType, options)!;

                // Add to dictionary.
                dictionary.Add(key, value);
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<TKey, TValue> dictionary,
            JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach ((TKey key, TValue value) in dictionary)
            {
                string propertyName = key.ToString();
                writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(propertyName) ?? propertyName);

                _valueConverter.Write(writer, value, options);
            }

            writer.WriteEndObject();
        }
    }
}