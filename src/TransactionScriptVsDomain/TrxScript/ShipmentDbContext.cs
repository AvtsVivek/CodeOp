using System;
using System.Threading.Tasks;
using Demo;
using Microsoft.EntityFrameworkCore;

namespace TransactionScriptVsDomain.TrxScript;

public class ShipmentDbContext : DbContext
{
    public DbSet<StopDataModel> Stops { get; set; }
}

public class StopDataModel
{
    public int ShipmentId { get; set; }
    public int StopId { get; set; }
    public StopStatus Status { get; set; }
    public DateTime Scheduled { get; set;}
    public DateTime Arrived { get; set; }
    public DateTime? Departed { get; set; }
}

public interface IShipmentRepository
{
    public Task<ShipmentAggregateRoot> Get(int shipmentId);
    public Task Save(ShipmentAggregateRoot shipmentAggregateRoot);
}