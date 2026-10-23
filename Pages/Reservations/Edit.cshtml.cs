using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorPageBase.Extensions;
using RazorPageBase.Models;
using RazorPageBase.Services;

namespace RazorPageBase.Pages.Reservations
{
    public class EditModel : PageModel
    {
        private readonly RazorPageBaseContext _context;

        public EditModel(RazorPageBaseContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Reservation Reservation { get; set; } = default!;

        public SelectList InstrumentList { get; set; } = default!;
        public List<Instrument> AvailableInstruments { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation.FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }
            if (!CanEdit(reservation))
            {
                TempData["Error"] = "只能修改自己尚未審核、且尚未開始的預約";
                return RedirectToPage("./Index");
            }

            Reservation = reservation;
            await LoadInstrumentsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var existing = await _context.Reservation.FirstOrDefaultAsync(r => r.Id == Reservation.Id);
            if (existing == null)
            {
                return NotFound();
            }
            if (!CanEdit(existing))
            {
                TempData["Error"] = "只能修改自己尚未審核、且尚未開始的預約";
                return RedirectToPage("./Index");
            }

            // 只更新使用者可以改的欄位 (防止 over-posting)
            existing.InstrumentId = Reservation.InstrumentId;
            existing.StartTime = Reservation.StartTime;
            existing.EndTime = Reservation.EndTime;
            existing.ProjectTitle = Reservation.ProjectTitle;
            existing.Advisor = Reservation.Advisor;
            existing.SampleCount = Reservation.SampleCount;
            existing.Purpose = Reservation.Purpose;

            var instrument = await ReservationRules.ValidateAsync(_context, existing, ModelState);
            if (!ModelState.IsValid || instrument is null)
            {
                await LoadInstrumentsAsync();
                return Page();
            }

            existing.TotalFee = ReservationRules.CalculateFee(instrument, existing);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"已更新預約，新的預估費用 NT$ {existing.TotalFee:N0}";
            return RedirectToPage("./Index");
        }

        private bool CanEdit(Reservation r) =>
            r.Status == ReservationStatus.Pending
            && r.StartTime > DateTime.Now
            && (r.UserId == User.GetUserId() || User.IsInRole("Admin"));

        private async Task LoadInstrumentsAsync()
        {
            AvailableInstruments = await _context.Instrument.AsNoTracking()
                .Where(i => i.Status == InstrumentStatus.Available)
                .OrderBy(i => i.Name)
                .ToListAsync();
            InstrumentList = new SelectList(
                AvailableInstruments.Select(i => new { i.Id, Text = $"{i.Name}（{i.AssetCode}）" }),
                "Id", "Text", Reservation.InstrumentId);
        }
    }
}
