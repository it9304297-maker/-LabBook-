using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPageBase.Models;

namespace RazorPageBase.Pages.Admin;

/// <summary>管理後台：營運儀表板</summary>
public class IndexModel : PageModel
{
    private readonly RazorPageBaseContext _context;

    public IndexModel(RazorPageBaseContext context)
    {
        _context = context;
    }

    public int InstrumentCount { get; set; }
    public int AvailableCount { get; set; }
    public int PendingCount { get; set; }
    public int UserCount { get; set; }
    public decimal MonthRevenue { get; set; }
    public IList<Reservation> PendingList { get; set; } = new List<Reservation>();
    public List<UsageRow> Usage { get; set; } = new();

    public record UsageRow(string Name, int Count, double Hours, decimal Revenue, double Utilization);

    public async Task OnGetAsync()
    {
        InstrumentCount = await _context.Instrument.CountAsync();
        AvailableCount = await _context.Instrument.CountAsync(i => i.Status == InstrumentStatus.Available);
        PendingCount = await _context.Reservation.CountAsync(r => r.Status == ReservationStatus.Pending);
        UserCount = await _context.Users.CountAsync();

        var monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var monthEnd = monthStart.AddMonths(1);
        MonthRevenue = await _context.Reservation
            .Where(r => (r.Status == ReservationStatus.Approved || r.Status == ReservationStatus.Completed)
                     && r.StartTime >= monthStart && r.StartTime < monthEnd)
            .SumAsync(r => r.TotalFee);

        PendingList = await _context.Reservation.Include(r => r.Instrument).AsNoTracking()
            .Where(r => r.Status == ReservationStatus.Pending)
            .OrderBy(r => r.StartTime)
            .Take(5)
            .ToListAsync();

        // 近 30 天各儀器使用率（已核准 + 已完成）；可用時數 = 30 天 × 14 小時 (08–22)
        var from = DateTime.Today.AddDays(-30);
        var to = DateTime.Today.AddDays(1);
        var instruments = await _context.Instrument.AsNoTracking().OrderBy(i => i.Name).ToListAsync();
        var bookings = await _context.Reservation.AsNoTracking()
            .Where(r => (r.Status == ReservationStatus.Approved || r.Status == ReservationStatus.Completed)
                     && r.StartTime >= from && r.StartTime < to)
            .Select(r => new { r.InstrumentId, r.StartTime, r.EndTime, r.TotalFee })
            .ToListAsync();

        const double capacityHours = 30 * 14;
        Usage = instruments.Select(i =>
        {
            var mine = bookings.Where(b => b.InstrumentId == i.Id).ToList();
            var hours = mine.Sum(b => (b.EndTime - b.StartTime).TotalHours);
            return new UsageRow(i.Name, mine.Count, hours, mine.Sum(b => b.TotalFee),
                Math.Round(hours / capacityHours * 100, 1));
        })
        .OrderByDescending(u => u.Hours)
        .ToList();
    }
}
