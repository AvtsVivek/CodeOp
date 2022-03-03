using System.Collections.Generic;

namespace AggregateConsistencyBoundary.EventSourced
{
    public class ShipmentState
    {
        public List<StopState> Stops { get; set; }
        public StopState CurrentStopState { get; set; }
    }

    public class StopState
    {
        public StopState(int stopId, StopType type, StopStatus status)
        {
            StopId = stopId;
            Type = type;
            Status = status;
        }

        public int StopId { get; set; }
        public StopType Type { get; set; }
        public StopStatus Status { get; set; }
    }
}