using System;

namespace AggregateConsistencyBoundary
{
    public class Stop
    {
        public int StopId { get; set; }
        public StopType Type { get; set; }
        public StopStatus Status { get; set; }
        public int Sequence { get; set; }
        public Address Address { get; set; }
        public DateTime Scheduled { get; set; }
        public DateTime? Departed { get; set; }

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
}