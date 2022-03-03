using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Demo;
using Demo.EventSourced;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace TransactionScriptVsDomain.TrxScript
{
    public class Pickup : IRequest
    {
        public int ShipmentId { get; set; }
        public int StopId { get; set; }
        public DateTime Departed { get; set; }
    }

    public class PickupHandler : IRequestHandler<Pickup>
    {
        private readonly ShipmentDbContext _dbContext;
        private readonly IBus _bus;

        public PickupHandler(ShipmentDbContext dbContext, IBus bus)
        {
            _dbContext = dbContext;
            _bus = bus;
        }

        public async Task<Unit> Handle(Pickup request, CancellationToken cancellationToken)
        {
            var stop = await _dbContext.Stops.SingleOrDefaultAsync(x => x.StopId == request.StopId);

            if (stop == null)
            {
                throw new InvalidOperationException("Stop does not exist.");
            }

            if (stop.Status == StopStatus.Departed)
            {
                throw new InvalidOperationException("Stop has already departed.");
            }

            if (stop.Status == StopStatus.InTransit)
            {
                throw new InvalidOperationException("Stop hasn't arrived yet.");
            }

            if (request.Departed < stop.Arrived)
            {
                throw new InvalidOperationException("Departed Date/Time cannot be before Arrived Date/Time.");
            }

            stop.Status = StopStatus.Departed;
            stop.Departed = request.Departed;

            await _dbContext.SaveChangesAsync();

            await _bus.Publish(new PickedUp(request.StopId, request.Departed));
            
            return Unit.Value;
        }
    }
}