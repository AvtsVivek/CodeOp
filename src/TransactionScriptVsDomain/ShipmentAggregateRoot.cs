using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.EventSourced;

namespace Demo
{
    public class ShipmentAggregateRoot
    {
        private readonly SortedList<int, Stop> _stops = new();
        private readonly List<IEvent> _events = new();

        private ShipmentAggregateRoot(IReadOnlyList<Stop> stops)
        {
            for(var x = 0; x < stops.Count; x++)
            {
                _stops.Add(x, stops[x]);
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

        public List<IEvent> GetUncommittedEvents()
        {
            var evnts = new List<IEvent>(_events);
            _events.Clear();
            return evnts;
        }

        public void Arrive(int stopId, DateTime arrived)
        {
            var currentStop = _stops.SingleOrDefault(x => x.Value.StopId == stopId);
            if (currentStop.Value == null)
            {
                throw new InvalidOperationException("Stop does not exist.");
            }

            var previousStopsAreNotDeparted = _stops.Any(x => x.Key < currentStop.Key && x.Value.Status != StopStatus.Departed);
            if (previousStopsAreNotDeparted)
            {
                throw new InvalidOperationException("Previous stops have not departed.");
            }

            currentStop.Value.Arrive(arrived);

            _events.Add(new Arrived(stopId, arrived));

            var arrivedToScheduled = arrived - currentStop.Value.Scheduled;
            if (arrivedToScheduled.TotalHours > 0)
            {
                _events.Add(new ArrivedLate(stopId, arrivedToScheduled));
            }
        }

        public void Pickup(int stopId, DateTime departed)
        {
            var currentStop = _stops.SingleOrDefault(x => x.Value.StopId == stopId);
            if (currentStop.Value == null)
            {
                throw new InvalidOperationException("Stop does not exist.");
            }

            if (currentStop.Value is not PickupStop)
            {
                throw new InvalidOperationException("Stop is not a pickup.");
            }

            if (currentStop.Value.Status != StopStatus.Arrived)
            {
                Arrive(stopId, departed);
            }

            currentStop.Value.Depart(departed);
            _events.Add(new PickedUp(stopId, departed));
        }

        public void Deliver(int stopId, DateTime departed)
        {
            var currentStop = _stops.SingleOrDefault(x => x.Value.StopId == stopId);
            if (currentStop.Value == null)
            {
                throw new InvalidOperationException("Stop does not exist.");
            }

            if (currentStop.Value is not DeliveryStop)
            {
                throw new InvalidOperationException("Stop is not a delivery.");
            }

            if (currentStop.Value.Status != StopStatus.Arrived)
            {
                Arrive(stopId, departed);
            }

            currentStop.Value.Depart(departed);
            _events.Add(new Delivered(stopId, departed));
        }

        public bool IsComplete()
        {
            return _stops.All(x => x.Value.Status == StopStatus.Departed);
        }
    }

    public class PickupStop : Stop
    {
        public PickupStop(int stopId)
        {
            StopId = stopId;
        }
    }

    public class DeliveryStop : Stop
    {
        public DeliveryStop(int stopId)
        {
            StopId = stopId;
        }
    }

    public record ShipmentStop(int StopId, StopType StopType, int Sequence);

    public abstract class Stop
    {
        public int StopId { get; protected set; }
        public StopStatus Status { get; private set; } = StopStatus.InTransit;
        public Address Address { get; protected set;}
        public DateTime Scheduled { get; protected set;}
        public DateTime Arrived { get; protected set; }
        public DateTime? Departed { get;  protected set; }

        public void Arrive(DateTime arrived)
        {
            if (Status != StopStatus.InTransit)
            {
                throw new InvalidOperationException("Stop has already arrived.");
            }

            Status = StopStatus.Arrived;
            Arrived = arrived;
        }

        public void Depart(DateTime departed)
        {
            if (Status == StopStatus.Departed)
            {
                throw new InvalidOperationException("Stop has already departed.");
            }

            if (departed < Arrived)
            {
                throw new InvalidOperationException("Departed Date/Time cannot be before Arrived Date/Time.");
            }

            Status = StopStatus.Departed;
            Departed = departed;
        }
    }

    public enum StopType
    {
        Pickup,
        Delivery
    }

    public enum StopStatus
    {
        InTransit,
        Arrived,
        Departed
    }

    public record Address(string Street, string City, string Postal, string Country);

    public interface IBus
    {
        Task Publish(IEvent evnt);
    }
}
