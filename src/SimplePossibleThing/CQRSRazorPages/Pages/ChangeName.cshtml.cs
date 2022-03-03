using CQRSGui;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleCQRS;

namespace CQRSRazorPages.Pages;

public class ChangeName : PageModel
{
    public InventoryItemDetailsDto InventoryItem { get; set; }

    public void OnGet(string id)
    {
        InventoryItem = new ReadModelFacade().GetInventoryItemDetails(Guid.Parse(id));
    }

    public IActionResult OnPost(string id, string name, int version)
    {
        var command = new RenameInventoryItem(Guid.Parse(id), name, version);
        ServiceLocator.MessageDispatcher.Send(command);

        return RedirectToPage("/Details", new { id = id});
    }
}