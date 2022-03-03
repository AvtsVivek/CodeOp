using System;
using System.Collections.Generic;
using System.Linq;

namespace AggregateConsistencyBoundary
{
    public class ShipmentAggregateRoot
    {
        public int ShipmentId { get; set; }
        public Address Customer { get; set; }
        public DateTime DateTime { get; set; }
        public List<Stop> Stops { get; set; } = new();

        public void Arrive(int stopId)
        {
            var currentStop = Stops.SingleOrDefault(x => x.StopId == stopId);
            if (currentStop == null)
            {
                throw new InvalidOperationException("Stop does not exist.");
            }

            var previousStopsAreNotDeparted = Stops.Any(x => x.Sequence < currentStop.Sequence && x.Status != StopStatus.Departed);
            if (previousStopsAreNotDeparted)
            {
                throw new InvalidOperationException("Previous stops have not departed.");
            }

            currentStop.Arrive();
        }

        public void Pickup(int stopId)
        {
            var currentStop = Stops.SingleOrDefault(x => x.StopId == stopId);
            if (currentStop == null)
            {
                throw new InvalidOperationException("Stop does not exist.");
            }

            if (currentStop.Type != StopType.Pickup)
            {
                throw new InvalidOperationException("Stop is not a pickup.");
            }

            currentStop.Depart();
        }

        public void Deliver(int stopId)
        {
            var currentStop = Stops.SingleOrDefault(x => x.StopId == stopId);
            if (currentStop == null)
            {
                throw new InvalidOperationException("Stop does not exist.");
            }

            if (currentStop.Type != StopType.Delivery)
            {
                throw new InvalidOperationException("Stop is not a delivery.");
            }

            currentStop.Depart();
        }

        public bool IsComplete()
        {
            return Stops.All(x => x.Status == StopStatus.Departed);
        }
    }

    public record Address(string Street, string City, string Postal, string Country);
}
