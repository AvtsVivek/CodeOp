using System;
using System.Collections.Generic;
using System.Linq;

namespace AggregateConsistencyBoundary
{
    public class Shipment
    {
        private readonly IList<Stop> _stops;

        public Shipment(IList<Stop> stops)
        {
            _stops = stops;
        }

        public bool IsComplete()
        {
            return _stops.All(x => x.Status == StopStatus.Departed);
        }

        public void Arrive(int stopId)
        {
            var currentStop = _stops.SingleOrDefault(x => x.StopId == stopId);
            if (currentStop == null)
            {
                throw new InvalidOperationException("Stop does not exist.");
            }

            var previousStopsAreNotDeparted = _stops.Any(x => x.Sequence < currentStop.Sequence && x.Status != StopStatus.Departed);
            if (previousStopsAreNotDeparted)
            {
                throw new InvalidOperationException("Previous stops have not departed.");
            }

            currentStop.Arrive();
        }

        public void Pickup(int stopId)
        {
            var currentStop = _stops.SingleOrDefault(x => x.StopId == stopId);
            if (currentStop == null)
            {
                throw new InvalidOperationException("Stop does not exist.");
            }

            if (currentStop is PickupStop == false)
            {
                throw new InvalidOperationException("Stop is not a pickup.");
            }

            currentStop.Depart();
        }

        public void Deliver(int stopId)
        {
            var currentStop = _stops.SingleOrDefault(x => x.StopId == stopId);
            if (currentStop == null)
            {
                throw new InvalidOperationException("Stop does not exist.");
            }

            if (currentStop is DeliveryStop == false)
            {
                throw new InvalidOperationException("Stop is not a delivery.");
            }

            currentStop.Depart();
        }
    }
}
