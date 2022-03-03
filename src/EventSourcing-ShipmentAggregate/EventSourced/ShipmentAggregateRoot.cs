using System;
using System.Collections.Generic;
using System.Linq;

namespace AggregateConsistencyBoundary.EventSourced
{
    public class ShipmentAggregateRoot
    {
        private readonly ShipmentState _projection = new();

        public void Dispatch(int shipmentId, IEnumerable<ShipmentStop> stops)
        {
            var dispatched = new Dispatched(shipmentId, stops, DateTime.UtcNow);
            Apply(dispatched);
        }

        private void Apply(Dispatched dispatched)
        {
            _projection.Stops =  dispatched.Stops.OrderBy(x => x.Sequence).Select(x => new StopState(x.StopId, x.StopType, StopStatus.InTransit)).ToList();
            _projection.CurrentStopState = _projection.Stops.First();
        }

        public void Arrive(int stopId)
        {
            if (IsComplete())
            {
                throw new InvalidOperationException("Shipment is already complete.");
            }

            if (_projection.CurrentStopState.StopId != stopId)
            {
                throw new InvalidOperationException("Stop does not exist or is not the current stop.");
            }

            if (_projection.CurrentStopState.Status != StopStatus.InTransit)
            {
                throw new InvalidOperationException("Stop has already arrived.");
            }

            var arrived = new Arrived(stopId, DateTime.UtcNow);
            Apply(arrived);
        }

        private void Apply(Arrived arrived)
        {
            _projection.CurrentStopState.Status = StopStatus.Arrived;
        }

        public void Pickup(int stopId)
        {
            if (IsComplete())
            {
                throw new InvalidOperationException("Shipment is already complete.");
            }

            if (_projection.CurrentStopState.StopId != stopId)
            {
                throw new InvalidOperationException("Stop does not exist or is not the current stop.");
            }

            if (_projection.CurrentStopState.Type != StopType.Pickup)
            {
                throw new InvalidOperationException("Stop is not a pickup.");
            }

            if (_projection.CurrentStopState.Status == StopStatus.Departed)
            {
                throw new InvalidOperationException("Stop has already departed.");
            }

            if (_projection.CurrentStopState.Status == StopStatus.InTransit)
            {
                throw new InvalidOperationException("Stop hasn't arrived yet.");
            }

            var pickedUp = new PickedUp(stopId, DateTime.UtcNow);
            Apply(pickedUp);
        }

        private void Apply(PickedUp pickedUp)
        {
            _projection.CurrentStopState = _projection.Stops.SkipWhile(x => x.Status != StopStatus.InTransit).FirstOrDefault();
        }

        public void Deliver(int stopId)
        {
            if (IsComplete())
            {
                throw new InvalidOperationException("Shipment is already complete.");
            }

            if (_projection.CurrentStopState.StopId != stopId)
            {
                throw new InvalidOperationException("Stop does not exist or is not the current stop.");
            }

            if (_projection.CurrentStopState.Type != StopType.Delivery)
            {
                throw new InvalidOperationException("Stop is not a delivery.");
            }

            if (_projection.CurrentStopState.Status == StopStatus.Departed)
            {
                throw new InvalidOperationException("Stop has already departed.");
            }

            if (_projection.CurrentStopState.Status == StopStatus.InTransit)
            {
                throw new InvalidOperationException("Stop hasn't arrived yet.");
            }

            var delivered = new Delivered(stopId, DateTime.UtcNow);
            Apply(delivered);
        }

        private void Apply(Delivered delivered)
        {
            _projection.CurrentStopState = _projection.Stops.SkipWhile(x => x.Status != StopStatus.InTransit).FirstOrDefault();
        }

        public bool IsComplete()
        {
            return _projection.CurrentStopState == null;
        }
    }

    public record ShipmentStop(int StopId, StopType StopType, int Sequence);
}
