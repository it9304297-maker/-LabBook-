using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorPageBase.Extensions;
using RazorPageBase.Models;
using RazorPageBase.Services;

namespace RazorPageBase.Pages.ReservationPages
{
    public class CreateModel : PageModel
    {
        private readonly RazorPageBaseContext _context;

        public CreateModel(RazorPageBaseContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Reservation Reservation { get; set; } = new();

        public SelectList InstrumentList { get; set; } = default!;

        // 給前端 JavaScript 即時試算費用用
        public List<Instrument> AvailableInstruments { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? instrumentId)
        {
            // 預設：明天 09:00–11:00
            var tomorrow = DateTime.Today.AddDays(1);
            Reservation.StartTime = tomorrow.AddHours(9);
            Reservation.EndTime = tomorrow.AddHours(11);
            if (instrumentId.HasValue) Reservation.InstrumentId = instrumentId.Value;

            await LoadInstrumentsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var instrument = await ReservationRules.ValidateAsync(_context, Reservation, ModelState);

            if (!ModelState.IsValid || instrument is null)
            {
                await LoadInstrumentsAsync();
                return Page();
            }

            // 由伺服器端填入，不信任前端 (老師講義：Identity 的 User ID)
            Reservation.UserId = User.GetUserId()!;
            Reservation.UserEmail = User.Identity?.Name ?? "";
            Reservation.Status = ReservationStatus.Pending;
            Reservation.TotalFee = ReservationRules.CalculateFee(instrument, Reservation);
            Reservation.CreatedAt = DateTime.Now;

            _context.Reservation.Add(Reservation);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"已送出「{instrument.Name}」預約申請，預估費用 NT$ {Reservation.TotalFee:N0}，請等待管理員審核";
            return RedirectToPage("./Index");
        }

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
