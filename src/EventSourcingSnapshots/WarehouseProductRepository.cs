using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;

namespace EventSourcing.Demo
{
    public class WarehouseProductEventStoreStream
    {
        private const int SnapshotInterval = 4;
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

        private string GetSnapshotStreamName(string sku)
        {
            return $"WarehouseProduct-Snapshot-{sku}";
        }

        public async Task<WarehouseProduct> Get(string sku)
        {
            var streamName = GetStreamName(sku);

            var snapshot = await GetSnapshot(sku);
            var warehouseProduct = new WarehouseProduct(sku, snapshot.State);

            StreamEventsSlice currentSlice;
            var nextSliceStart = snapshot.Version + 1;
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
                }
            } while (!currentSlice.IsEndOfStream);

            return warehouseProduct;
        }

        private async Task<Snapshot> GetSnapshot(string sku)
        {
            var streamName = GetSnapshotStreamName(sku);
            var slice = await _connection.ReadStreamEventsBackwardAsync(streamName, (long)StreamPosition.End, 1, false);
            if (slice.Events.Any())
            {
                var evnt = slice.Events.First();
                var json = Encoding.UTF8.GetString(evnt.Event.Data);
                return JsonConvert.DeserializeObject<Snapshot>(json);
            }

            return new Snapshot();
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

        public async Task Save(WarehouseProduct warehouseProduct)
        {
            var streamName = GetStreamName(warehouseProduct.Sku);

            var newEvents = warehouseProduct.GetUncommittedEvents();
            long version = 0;
            foreach (var evnt in newEvents)
            {
                var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(evnt));
                var metadata = Encoding.UTF8.GetBytes("{}");
                var evt = new EventData(Guid.NewGuid(), evnt.EventType, true, data, metadata);
                var result = await _connection.AppendToStreamAsync(streamName, ExpectedVersion.Any, evt);
                version = result.NextExpectedVersion;
            }

            if ((version + 1) >= SnapshotInterval && (version + 1) % SnapshotInterval == 0)
            {
                await AppendSnapshot(warehouseProduct, version);
            }
        }

        private async Task AppendSnapshot(WarehouseProduct warehouseProduct, long version)
        {
            var streamName = GetSnapshotStreamName(warehouseProduct.Sku);
            var state = warehouseProduct.GetState();

            var snapshot = new Snapshot
            {
                State = state,
                Version = version
            };

            var metadata = Encoding.UTF8.GetBytes("{}");
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(snapshot));
            var evt = new EventData(Guid.NewGuid(), "snapshot", true, data, metadata);
            await _connection.AppendToStreamAsync(streamName, ExpectedVersion.Any, evt);
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }

    public class Snapshot
    {
        public long Version { get; set; } = -1;
        public WarehouseProductState State { get; set; } = new();
    }
}