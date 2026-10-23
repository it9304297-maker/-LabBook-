using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPageBase.Models;

namespace RazorPageBase.Pages.InstrumentPages;

public class DetailsModel : PageModel
{
    private readonly RazorPageBaseContext _context;
    public DetailsModel(RazorPageBaseContext context)
    {
        _context = context;
    }

    public Instrument Instrument { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var instrument = await _context.Instrument.FirstOrDefaultAsync(m => m.Id == id);
        if (instrument is null)
        {
            return NotFound();
        }
        else
        {
            Instrument = instrument;
        }

        return Page();
    }
}
