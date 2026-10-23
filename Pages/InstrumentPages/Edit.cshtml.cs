using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPageBase.Models;

namespace RazorPageBase.Pages.InstrumentPages;

public class EditModel : PageModel
{
    private readonly RazorPageBaseContext _context;

    public EditModel(RazorPageBaseContext context)
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
        Instrument = instrument;
        return Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Instrument).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!InstrumentExists(Instrument.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("./Index");
    }

    private bool InstrumentExists(int id)
    {
        return _context.Instrument.Any(e => e.Id == id);
    }
}
