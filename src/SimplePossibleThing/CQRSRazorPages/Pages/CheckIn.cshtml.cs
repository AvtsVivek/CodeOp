using CQRSGui;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleCQRS;

namespace CQRSRazorPages.Pages;

public class CheckIn : PageModel
{
    public InventoryItemDetailsDto InventoryItem { get; set; }

    public void OnGet(string id)
    {
        InventoryItem = new ReadModelFacade().GetInventoryItemDetails(Guid.Parse(id));
    }

    public IActionResult OnPost(string id, int number, int version)
    {
        var command = new CheckInItemsToInventory(Guid.Parse(id), number, version);
        ServiceLocator.MessageDispatcher.Send(command);

        return RedirectToPage("/Details", new { id = id});
    }
}