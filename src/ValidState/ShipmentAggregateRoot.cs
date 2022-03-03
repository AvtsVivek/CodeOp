using System;
using System.Collections.Generic;
using System.Linq;

namespace AggregateConsistencyBoundary
{
    public class ShipmentAggregateRoot
    {
        private SortedList<int, Stop> Stops { get; } = new();

        private ShipmentAggregateRoot(IReadOnlyList<Stop> stops)
        {
            for(var x = 0; x < stops.Count; x++)
            {
                Stops.Add(x, stops[x]);
            }
        }

        public static ShipmentAggregateRoot Factory(PickupStop pickup, DeliveryStop delivery)
        {
            return new ShipmentAggregateRoot(new Stop[] { pickup, delivery });
        }

        public static ShipmentAggregateRoot Factory(Stop[] stops)
        {
            if (stops.Length < 2)
            {
                throw new InvalidOperationException("Shipment requires at least 2 stops.");
            }

            if (stops.First() is not PickupStop)
            {
                throw new InvalidOperationException("First stop must be a Pickup");
            }

            if (stops.Last() is not DeliveryStop)
            {
                throw new InvalidOperationException("first stop must be a Pickup");
            }

            return new ShipmentAggregateRoot(stops);
        }

        public void Arrive(int stopId)
        {
            var currentStop = Stops.SingleOrDefault(x => x.Value.StopId == stopId);
            if (currentStop.Value == null)
            {
                throw new InvalidOperationException("Stop does not exist.");
            }

            var previousStopsAreNotDeparted = Stops.Any(x => x.Key < currentStop.Key && x.Value.Status != StopStatus.Departed);
            if (previousStopsAreNotDeparted)
            {
                throw new InvalidOperationException("Previous stops have not departed.");
            }

            currentStop.Value.Arrive();
        }

        public void Pickup(int stopId)
        {
            var currentStop = Stops.SingleOrDefault(x => x.Value.StopId == stopId);
            if (currentStop.Value == null)
            {
                throw new InvalidOperationException("Stop does not exist.");
            }

            if (currentStop.Value is not PickupStop)
            {
                throw new InvalidOperationException("Stop is not a pickup.");
            }

            currentStop.Value.Depart();
        }

        public void Deliver(int stopId)
        {
            var currentStop = Stops.SingleOrDefault(x => x.Value.StopId == stopId);
            if (currentStop.Value == null)
            {
                throw new InvalidOperationException("Stop does not exist.");
            }

            if (currentStop.Value is not DeliveryStop)
            {
                throw new InvalidOperationException("Stop is not a delivery.");
            }

            currentStop.Value.Depart();
        }

        public bool IsComplete()
        {
            return Stops.All(x => x.Value.Status == StopStatus.Departed);
        }
    }
}
