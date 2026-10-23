using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPageBase.Models;

namespace RazorPageBase.Pages.ReservationPages;

public class DetailsModel : PageModel
{
    private readonly RazorPageBaseContext _context;
    public DetailsModel(RazorPageBaseContext context)
    {
        _context = context;
    }

    public Reservation Reservation { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var reservation = await _context.Reservation.FirstOrDefaultAsync(m => m.Id == id);
        if (reservation is null)
        {
            return NotFound();
        }
        else
        {
            Reservation = reservation;
        }

        return Page();
    }
}
