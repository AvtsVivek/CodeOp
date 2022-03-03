using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Demo;
using Demo.EventSourced;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace TransactionScriptVsDomain.TrxScript.Arrive3
{
    public class Arrive : IRequest
    {
        public int ShipmentId { get; set; }
        public int StopId { get; set; }
        public DateTime Arrived { get; set; }
    }

    public class ArriveHandler : IRequestHandler<Arrive>
    {
        private readonly ShipmentDbContext _dbContext;
        private readonly IBus _bus;

        public ArriveHandler(ShipmentDbContext dbContext, IBus bus)
        {
            _dbContext = dbContext;
            _bus = bus;
        }

        public async Task<Unit> Handle(Arrive request, CancellationToken cancellationToken)
        {
            var allStops = await _dbContext.Stops.Where(x => x.ShipmentId == request.ShipmentId).ToArrayAsync();

            var stop = allStops.SingleOrDefault(x => x.StopId == request.StopId);
            if (stop == null)
            {
                throw new InvalidOperationException("Stop does not exist.");
            }

            if (stop.Status != StopStatus.InTransit)
            {
                throw new InvalidOperationException("Stop has already arrived.");
            }

            var previousStopsAreNotDeparted = allStops
                .Where(x => x.Scheduled < stop.Scheduled)
                .Any(x => x.Status != StopStatus.Departed);

            if (previousStopsAreNotDeparted)
            {
                throw new InvalidOperationException("Previous stops have not departed.");
            }

            stop.Status = StopStatus.Arrived;
            stop.Arrived = request.Arrived;

            await _dbContext.SaveChangesAsync();

            await _bus.Publish(new Arrived(stop.StopId, stop.Arrived));

            return Unit.Value;
        }
    }
}