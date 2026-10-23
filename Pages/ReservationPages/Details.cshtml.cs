using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPageBase.Extensions;
using RazorPageBase.Models;

namespace RazorPageBase.Pages.ReservationPages
{
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
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation.Include(r => r.Instrument)
                .AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (reservation is null)
            {
                return NotFound();
            }
            // 只有本人或管理員可以看
            if (reservation.UserId != User.GetUserId() && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            Reservation = reservation;
            return Page();
        }
    }
}
