using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPageBase.Models;

namespace RazorPageBase.Pages.InstrumentPages;

public class DeleteModel : PageModel
{
    private readonly RazorPageBaseContext _context;

    public DeleteModel(RazorPageBaseContext context)
    {
        _context = context;
    }

    [BindProperty]
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

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var instrument = await _context.Instrument.FindAsync(id);
        if (instrument != null)
        {
            Instrument = instrument;
            _context.Instrument.Remove(Instrument);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
