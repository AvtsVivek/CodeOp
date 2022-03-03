var tenantIds = Enumerable.Range(1, 1000).Select(x => Guid.NewGuid()).ToArray();
var httpClient = new HttpClient();

while (true)
{
    Console.Write("Tenant: ");
    if (int.TryParse(Console.ReadLine(), out var tenantNumber))
    {
        var tenant = tenantIds.Skip(tenantNumber - 1).First();

        await Parallel.ForEachAsync(Enumerable.Repeat(true, 10), async (_, _) =>
        {
            var response = await httpClient.PostAsync(new Uri($"http://localhost:6000/invoice/{tenant}"), null);
            var invoiceNumber = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Tenant: {tenant}, Invoice #: {invoiceNumber}");

        });
    }

}
