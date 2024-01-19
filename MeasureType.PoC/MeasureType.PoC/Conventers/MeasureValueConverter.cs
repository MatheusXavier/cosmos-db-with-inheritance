using MeasureType.PoC.Models;

using Newtonsoft.Json;

namespace MeasureType.PoC.Conventers;

public class MeasureValueConverter : JsonConverter<MeasureValueBase>
{
    public override MeasureValueBase? ReadJson(
        JsonReader reader,
        Type objectType,
        MeasureValueBase? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        serializer.TypeNameHandling = TypeNameHandling.Objects;

        return serializer.Deserialize(reader) as MeasureValueBase;
    }

    public override void WriteJson(JsonWriter writer, MeasureValueBase? value, JsonSerializer serializer)
    {
        serializer.TypeNameHandling = TypeNameHandling.Objects;

        serializer.Serialize(writer, value);
    }
}
