using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPageBase.Models;

namespace RazorPageBase.Pages.InstrumentPages;

public class IndexModel : PageModel
{
    private readonly RazorPageBaseContext _context;

    public IndexModel(RazorPageBaseContext context)
    {
        _context = context;
    }

    public IList<Instrument> Instrument { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Instrument = await _context.Instrument.ToListAsync();
    }
}
