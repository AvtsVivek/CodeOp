using CQRSGui;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleCQRS;

namespace CQRSRazorPages.Pages;

public class Deactivate : PageModel
{
    public InventoryItemDetailsDto InventoryItem { get; set; }

    public void OnGet(string id)
    {
        InventoryItem = new ReadModelFacade().GetInventoryItemDetails(Guid.Parse(id));
    }

    public IActionResult OnPost(string id, int version)
    {
        var command = new DeactivateInventoryItem(Guid.Parse(id), version);
        ServiceLocator.MessageDispatcher.Send(command);

        return RedirectToPage("/Index");
    }
}