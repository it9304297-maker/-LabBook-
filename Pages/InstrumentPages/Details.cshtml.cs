using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPageBase.Models;

namespace RazorPageBase.Pages.InstrumentPages
{
    public class DetailsModel : PageModel
    {
        private readonly RazorPageBaseContext _context;

        public DetailsModel(RazorPageBaseContext context)
        {
            _context = context;
        }

        public Instrument Instrument { get; set; } = default!;

        // 未來 14 天已被預約（待審核/已核准）的時段，讓使用者避開
        public IList<Reservation> UpcomingBookings { get; set; } = new List<Reservation>();

        public int TotalBookings { get; set; }
        public double TotalHours { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instrument = await _context.Instrument.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (instrument is null)
            {
                return NotFound();
            }
            Instrument = instrument;

            var now = DateTime.Now;
            var until = DateTime.Today.AddDays(14);
            UpcomingBookings = await _context.Reservation.AsNoTracking()
                .Where(r => r.InstrumentId == id
                         && r.EndTime >= now && r.StartTime <= until
                         && (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Approved))
                .OrderBy(r => r.StartTime)
                .ToListAsync();

            var used = await _context.Reservation.AsNoTracking()
                .Where(r => r.InstrumentId == id
                         && (r.Status == ReservationStatus.Approved || r.Status == ReservationStatus.Completed))
                .Select(r => new { r.StartTime, r.EndTime })
                .ToListAsync();
            TotalBookings = used.Count;
            TotalHours = used.Sum(r => (r.EndTime - r.StartTime).TotalHours);

            return Page();
        }
    }
}
