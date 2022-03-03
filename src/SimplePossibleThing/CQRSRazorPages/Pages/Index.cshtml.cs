using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleCQRS;

namespace CQRSRazorPages.Pages;

public class IndexModel : PageModel
{
    public IEnumerable<InventoryItemListDto> InventoryItems { get; set; }

    public void OnGet()
    {
        InventoryItems = new ReadModelFacade().GetInventoryItems();
    }
}
