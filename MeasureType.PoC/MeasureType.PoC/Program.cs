using MeasureType.PoC.Models;

using Microsoft.Azure.Cosmos;

const string EndpointUri = "https://localhost:8081";
const string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
const string DatabaseId = "MeasureType.PoC";
const string ContainerId = "measures";

CosmosClient cosmosClient = new(EndpointUri, PrimaryKey, new CosmosClientOptions
{
    SerializerOptions = new CosmosSerializationOptions
    {
        IgnoreNullValues = true,
        Indented = true,
        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
    },
});
DatabaseResponse database = await cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseId);
ContainerResponse container = await database.Database.CreateContainerIfNotExistsAsync(ContainerId, "/type");

SensorMeasure temperatureMeasure = new(
    Guid.NewGuid(),
    MeasureType.PoC.Models.MeasureType.Temperature,
    DateTime.UtcNow,
    new TemperatureMeasureValue { Temperature = new Random().NextDouble() });

SensorMeasure airQualityMeasure = new(
    Guid.NewGuid(),
    MeasureType.PoC.Models.MeasureType.AirQuality,
    DateTime.UtcNow,
    new AirQualityMeasureValue { AirQualityIndex = new Random().Next() });

SensorMeasure luminosityMeasure = new(
    Guid.NewGuid(),
    MeasureType.PoC.Models.MeasureType.Luminosity,
    DateTime.UtcNow,
    new LuminosityMeasureValue { LuminosityValue = new Random().Next() });

await SaveSensorMeasureAsync(container, temperatureMeasure);
await SaveSensorMeasureAsync(container, airQualityMeasure);
await SaveSensorMeasureAsync(container, luminosityMeasure);

List<SensorMeasure> sensorMeasures = await QuerySensorMeasuresAsync(container);

foreach (SensorMeasure sensorMeasure in sensorMeasures)
{
    Console.WriteLine($"Id: {sensorMeasure.Id}, Type: {sensorMeasure.Type}, Date: {sensorMeasure.Date}, Value: {sensorMeasure.Value}");
}

static async Task SaveSensorMeasureAsync(Container container, SensorMeasure sensorMeasure)
{
    ItemResponse<SensorMeasure> itemResponse = await container.CreateItemAsync(
        sensorMeasure,
        new PartitionKey((int)sensorMeasure.Type));

    Console.WriteLine($"Saved sensor measure with id {itemResponse.Resource.Id}");
}

static async Task<List<SensorMeasure>> QuerySensorMeasuresAsync(Container container)
{
    string sqlQueryText = "SELECT * FROM c";
    QueryDefinition queryDefinition = new(sqlQueryText);

    FeedIterator<SensorMeasure> iterator = container.GetItemQueryIterator<SensorMeasure>(queryDefinition);

    List<SensorMeasure> sensorMeasures = [];

    while (iterator.HasMoreResults)
    {
        FeedResponse<SensorMeasure> response = await iterator.ReadNextAsync();

        sensorMeasures.AddRange(response.ToList());
    }

    return sensorMeasures;
}