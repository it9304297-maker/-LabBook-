using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPageBase.Models;

namespace RazorPageBase.Pages.Instruments
{
    public class DeleteModel : PageModel
    {
        private readonly RazorPageBaseContext _context;

        public DeleteModel(RazorPageBaseContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Instrument Instrument { get; set; } = default!;

        public int ReservationCount { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instrument = await _context.Instrument.FirstOrDefaultAsync(m => m.Id == id);

            if (instrument is not null)
            {
                Instrument = instrument;
                ReservationCount = await _context.Reservation.CountAsync(r => r.InstrumentId == id);
                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instrument = await _context.Instrument.FindAsync(id);
            if (instrument == null)
            {
                return RedirectToPage("./Index");
            }

            // 業務規則：已有預約紀錄的儀器不可刪除，改為「停用」以保留歷史資料
            ReservationCount = await _context.Reservation.CountAsync(r => r.InstrumentId == id);
            if (ReservationCount > 0)
            {
                instrument.Status = InstrumentStatus.Retired;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"「{instrument.Name}」已有 {ReservationCount} 筆預約紀錄，已改為停用並保留歷史資料";
                return RedirectToPage("./Index");
            }

            _context.Instrument.Remove(instrument);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"已刪除儀器「{instrument.Name}」";

            return RedirectToPage("./Index");
        }
    }
}
