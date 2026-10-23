using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPageBase.Models;

namespace RazorPageBase.Pages.ReservationPages;

public class IndexModel : PageModel
{
    private readonly RazorPageBaseContext _context;

    public IndexModel(RazorPageBaseContext context)
    {
        _context = context;
    }

    public IList<Reservation> Reservation { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Reservation = await _context.Reservation.ToListAsync();
    }
}
