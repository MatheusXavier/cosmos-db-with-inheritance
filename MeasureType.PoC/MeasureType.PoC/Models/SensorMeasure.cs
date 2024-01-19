using MeasureType.PoC.Conventers;

using Newtonsoft.Json;

namespace MeasureType.PoC.Models;

public class SensorMeasure(Guid id, MeasureType type, DateTime date, MeasureValueBase value)
{
    public Guid Id { get; set; } = id;

    public MeasureType Type { get; set; } = type;

    public DateTime Date { get; set; } = date;

    [JsonConverter(typeof(MeasureValueConverter))]
    public MeasureValueBase Value { get; set; } = value;
}