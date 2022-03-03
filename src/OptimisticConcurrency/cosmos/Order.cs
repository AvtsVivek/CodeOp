using Newtonsoft.Json;

public class Order
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "customerId")]
    public string CustomerId { get; set; }

    [JsonProperty(PropertyName = "status")]
    public OrderStatus Status { get; set; } = OrderStatus.Placed;
}

public enum OrderStatus
{
    Placed,
    Processing,
    Cancelled,
    Shipped,
}