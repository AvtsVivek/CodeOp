using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleCQRS;

namespace CQRSRazorPages.Pages;

public class Details : PageModel
{
    public InventoryItemDetailsDto InventoryItem { get; set; }

    public void OnGet(string id)
    {
        InventoryItem = new ReadModelFacade().GetInventoryItemDetails(Guid.Parse(id));
    }


}