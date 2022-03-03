using System;

namespace AggregateConsistencyBoundary
{
    public class PickupStop : Stop
    {
        public PickupStop(int stopId, Address address, DateTime scheduled)
            : base(stopId, address, scheduled)
        {
            StopId = stopId;
            Address = address;
        }
    }

    public class DeliveryStop : Stop
    {
        public DeliveryStop(int stopId, Address address, DateTime scheduled)
            : base(stopId, address, scheduled)
        {
            StopId = stopId;
            Address = address;
        }
    }

    public abstract class Stop
    {
        public int StopId { get; protected set; }
        public StopStatus Status { get; private set; } = StopStatus.InTransit;
        public Address Address { get; protected set;}
        public DateTime Scheduled { get; }
        public DateTime? Departed { get;  protected set; }

        public Stop(int stopId, Address address, DateTime scheduled)
        {
            StopId = stopId;
            Address = address;
            Scheduled = scheduled;
        }

        public void Arrive()
        {
            if (Status != StopStatus.InTransit)
            {
                throw new InvalidOperationException("Stop has already arrived.");
            }

            Status = StopStatus.Arrived;
        }

        public void Depart()
        {
            if (Status == StopStatus.Departed)
            {
                throw new InvalidOperationException("Stop has already departed.");
            }

            if (Status == StopStatus.InTransit)
            {
                throw new InvalidOperationException("Stop hasn't arrived yet.");
            }

            Status = StopStatus.Departed;
            Departed = DateTime.UtcNow;
        }
    }

    public enum StopStatus
    {
        InTransit,
        Arrived,
        Departed
    }
}