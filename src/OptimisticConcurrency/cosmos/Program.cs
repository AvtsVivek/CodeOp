using Microsoft.Azure.Cosmos;

using var client = new CosmosClient("AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
var container = client.GetContainer("demo", "orders");
var newOrder = new Order
{
    Id = Guid.NewGuid().ToString(),
    CustomerId = "CodeOpinion"
};

await container.CreateItemAsync(newOrder, new PartitionKey(newOrder.Id));

var readOrder = await container.ReadItemAsync<Order>(newOrder.Id, new PartitionKey(newOrder.Id));
readOrder.Resource.Status = OrderStatus.Processing;

// Works because ETag is correct
await container.UpsertItemAsync(readOrder.Resource, new PartitionKey(readOrder.Resource.Id), new ItemRequestOptions
{
    IfMatchEtag = readOrder.ETag,
});

// Fails because ETag was changed when Upsert occured above.
await container.UpsertItemAsync(readOrder.Resource, new PartitionKey(readOrder.Resource.Id), new ItemRequestOptions
{
    IfMatchEtag = readOrder.ETag,
});

