using Microsoft.Extensions.Logging;
using Orleans;

public interface IInvoiceNumberGeneratorGrain : IGrainWithGuidKey
{
    public Task<int> ReserveInvoiceNumber();
}

public class InvoiceNumberGeneratorGrain : Grain, IInvoiceNumberGeneratorGrain
{
    private readonly ILogger<InvoiceNumberGeneratorGrain> _logger;
    private int _currentInvoiceNumber = 0;

    public InvoiceNumberGeneratorGrain(ILogger<InvoiceNumberGeneratorGrain> logger)
    {
        _logger = logger;
    }

    public override Task OnActivateAsync()
    {
        // Fetch Data from database or use persistence
        return base.OnActivateAsync();
    }

    public Task<int> ReserveInvoiceNumber()
    {
        _logger.LogInformation($"Executing in Grain Tenant: {this.GetPrimaryKey()}, Invoice #: {_currentInvoiceNumber}");

        _currentInvoiceNumber++;
        return Task.FromResult(_currentInvoiceNumber);
    }
}