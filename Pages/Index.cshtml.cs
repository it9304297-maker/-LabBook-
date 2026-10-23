using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPageBase.Models;

namespace RazorPageBase.Pages
{
    public class IndexModel : PageModel
    {
        private readonly RazorPageBaseContext _context;

        public IndexModel(RazorPageBaseContext context)
        {
            _context = context;
        }

        public IList<Instrument> Featured { get; set; } = new List<Instrument>();
        public int InstrumentCount { get; set; }
        public int TodayBookings { get; set; }
        public int CategoryCount { get; set; }

        public async Task OnGetAsync()
        {
            Featured = await _context.Instrument.AsNoTracking()
                .Where(i => i.Status == InstrumentStatus.Available)
                .OrderBy(i => i.AssetCode)
                .Take(3)
                .ToListAsync();
            InstrumentCount = await _context.Instrument.CountAsync(i => i.Status != InstrumentStatus.Retired);
            CategoryCount = await _context.Instrument.Select(i => i.Category).Distinct().CountAsync();

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            TodayBookings = await _context.Reservation.CountAsync(r =>
                r.Status == ReservationStatus.Approved && r.StartTime >= today && r.StartTime < tomorrow);
        }
    }
}
