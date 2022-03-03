using System.Net;
using System.Text;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;

namespace EventSourcing.Demo
{
    public class WarehouseProductEventStoreStream : IDisposable
    {
        private readonly IEventStoreConnection _connection;

        public static async Task<WarehouseProductEventStoreStream> Factory()
        {
            var connectionSettings = ConnectionSettings.Create()
                .KeepReconnecting()
                .KeepRetrying()
                .SetHeartbeatTimeout(TimeSpan.FromMinutes(5))
                .SetHeartbeatInterval(TimeSpan.FromMinutes(1))
                .DisableTls()
                .DisableServerCertificateValidation()
                .SetDefaultUserCredentials(new UserCredentials("admin", "changeit"))
                .Build();

            var conn = EventStoreConnection.Create(connectionSettings, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1113));
            await conn.ConnectAsync();

            return new WarehouseProductEventStoreStream(conn);
        }

        private WarehouseProductEventStoreStream(IEventStoreConnection connection)
        {
            _connection = connection;
        }

        private string GetStreamName(string sku)
        {
            return $"WarehouseProduct-{sku}";
        }

        public async Task<EventStreamAggregate<WarehouseProduct>> Get(string sku)
        {
            var streamName = GetStreamName(sku);

            var warehouseProduct = new WarehouseProduct(sku, new WarehouseProductState());

            StreamEventsSlice currentSlice;
            long nextSliceStart = 0;
            long lastVersion = -1;
            do
            {
                currentSlice = await _connection.ReadStreamEventsForwardAsync(
                    streamName,
                    nextSliceStart,
                    200,
                    false
                );

                nextSliceStart = currentSlice.NextEventNumber;

                foreach (var evnt in currentSlice.Events)
                {
                    var eventObj = DeserializeEvent(evnt);
                    warehouseProduct.ApplyEvent(eventObj);
                    lastVersion = evnt.OriginalEventNumber;
                }
            } while (!currentSlice.IsEndOfStream);

            return new EventStreamAggregate<WarehouseProduct>(warehouseProduct, lastVersion);
        }

        private IEvent DeserializeEvent(ResolvedEvent evnt)
        {
            var json = Encoding.UTF8.GetString(evnt.Event.Data);
            return evnt.Event.EventType switch
            {
                "InventoryAdjusted" => JsonConvert.DeserializeObject<InventoryAdjusted>(json),
                "ProductShipped" => JsonConvert.DeserializeObject<ProductShipped>(json),
                "ProductReceived" => JsonConvert.DeserializeObject<ProductReceived>(json),
                _ => throw new InvalidOperationException($"Unknown Event: {evnt.Event.EventType}")
            };
        }

        public async Task Save(WarehouseProduct warehouseProduct, long expectedVersion)
        {
            var streamName = GetStreamName(warehouseProduct.Sku);

            var newEvents = warehouseProduct.GetUncommittedEvents();
            foreach (var evnt in newEvents)
            {
                var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(evnt));
                var metadata = Encoding.UTF8.GetBytes("{}");
                var evt = new EventData(Guid.NewGuid(), evnt.EventType, true, data, metadata);
                var result = await _connection.AppendToStreamAsync(streamName, expectedVersion, evt);
                expectedVersion = result.NextExpectedVersion;
            }
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }

    public class EventStreamAggregate<T>
    {
        public EventStreamAggregate(T aggregate, long version)
        {
            Aggregate = aggregate;
            Version = version;
        }

        public T Aggregate { get; private set; }
        public long Version { get; private set; }
    }
}