using System;

namespace AggregateConsistencyBoundary
{
    public abstract class Stop
    {
        public int StopId { get; set; }
        public StopStatus Status { get; set; }
        public int Sequence { get; set; }

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
        }
    }

    public class PickupStop : Stop { }

    public class DeliveryStop : Stop { }

    public enum StopStatus
    {
        InTransit,
        Arrived,
        Departed
    }
}