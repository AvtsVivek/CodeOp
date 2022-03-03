using CQRSGui;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleCQRS;

namespace CQRSRazorPages.Pages;

public class Add : PageModel
{
    public IActionResult OnPost(string name)
    {
        var id = Guid.NewGuid();
        ServiceLocator.MessageDispatcher.Send(new CreateInventoryItem(id, name));
        return RedirectToPage("/Details", new { id = id});
    }
}