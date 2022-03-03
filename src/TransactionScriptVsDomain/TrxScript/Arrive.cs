using System;
using System.Threading;
using System.Threading.Tasks;
using Demo;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace TransactionScriptVsDomain.TrxScript
{
    public class Arrive : IRequest
    {
        public int StopId { get; set; }
        public DateTime Arrived { get; set; }
    }

    public class ArriveHandler : IRequestHandler<Arrive>
    {
        private readonly ShipmentDbContext _dbContext;

        public ArriveHandler(ShipmentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(Arrive request, CancellationToken cancellationToken)
        {
            var stop = await _dbContext.Stops.SingleOrDefaultAsync(x => x.StopId == request.StopId);
            if (stop == null)
            {
                throw new InvalidOperationException("Stop does not exist.");
            }

            if (stop.Status != StopStatus.InTransit)
            {
                throw new InvalidOperationException("Stop has already arrived.");
            }

            stop.Status = StopStatus.Arrived;
            stop.Arrived = request.Arrived;

            await _dbContext.SaveChangesAsync();

            return Unit.Value;
        }
    }
}