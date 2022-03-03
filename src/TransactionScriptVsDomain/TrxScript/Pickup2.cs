using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Demo;
using Demo.EventSourced;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using IRequest = MediatR.IRequest;

namespace TransactionScriptVsDomain.TrxScript.Pickup2
{
    public class Pickup : IRequest
    {
        public int ShipmentId { get; set; }
        public int StopId { get; set; }
        public DateTime Departed { get; set; }
    }

    public class PickupHandler : IRequestHandler<Pickup>
    {
        private readonly IShipmentRepository _shipmentRepository;

        public PickupHandler(IShipmentRepository shipmentRepository)
        {
            _shipmentRepository = shipmentRepository;
        }

        public async Task<Unit> Handle(Pickup request, CancellationToken cancellationToken)
        {
            var shipment = await _shipmentRepository.Get(request.ShipmentId);
            shipment.Pickup(request.StopId, request.Departed);

            await _shipmentRepository.Save(shipment);

            return Unit.Value;
        }
    }
}