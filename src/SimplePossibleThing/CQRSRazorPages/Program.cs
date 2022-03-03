using CQRSGui;
using SimpleCQRS;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

var messageDispatcher = new MessageDispatcher();

var storage = new EventStore(messageDispatcher);
var rep = new Repository<InventoryItem>(storage);
var commands = new InventoryCommandHandlers(rep);
messageDispatcher.RegisterHandler<CheckInItemsToInventory>(commands.Handle);
messageDispatcher.RegisterHandler<CreateInventoryItem>(commands.Handle);
messageDispatcher.RegisterHandler<DeactivateInventoryItem>(commands.Handle);
messageDispatcher.RegisterHandler<RemoveItemsFromInventory>(commands.Handle);
messageDispatcher.RegisterHandler<RenameInventoryItem>(commands.Handle);
var detail = new InventoryItemDetailView();
messageDispatcher.RegisterHandler<InventoryItemCreated>(detail.Handle);
messageDispatcher.RegisterHandler<InventoryItemDeactivated>(detail.Handle);
messageDispatcher.RegisterHandler<InventoryItemRenamed>(detail.Handle);
messageDispatcher.RegisterHandler<ItemsCheckedInToInventory>(detail.Handle);
messageDispatcher.RegisterHandler<ItemsRemovedFromInventory>(detail.Handle);
var list = new InventoryListView();
messageDispatcher.RegisterHandler<InventoryItemCreated>(list.Handle);
messageDispatcher.RegisterHandler<InventoryItemRenamed>(list.Handle);
messageDispatcher.RegisterHandler<InventoryItemDeactivated>(list.Handle);
ServiceLocator.MessageDispatcher = messageDispatcher;

app.Run();
