using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPageBase.Extensions;
using RazorPageBase.Models;

namespace RazorPageBase.Pages.ReservationPages
{
    /// <summary>
    /// 使用者：取消預約（狀態改為已取消，保留紀錄）
    /// 管理員：可永久刪除紀錄
    /// </summary>
    public class DeleteModel : PageModel
    {
        private readonly RazorPageBaseContext _context;

        public DeleteModel(RazorPageBaseContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Reservation Reservation { get; set; } = default!;

        public bool IsAdmin => User.IsInRole("Admin");
        public bool CanCancel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation.Include(r => r.Instrument)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation is null)
            {
                return NotFound();
            }
            if (reservation.UserId != User.GetUserId() && !IsAdmin)
            {
                return Forbid();
            }

            Reservation = reservation;
            CanCancel = IsCancellable(reservation);
            if (!CanCancel && !IsAdmin)
            {
                TempData["Error"] = "此預約已開始或已結案，無法取消";
                return RedirectToPage("./Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostCancelAsync(int? id)
        {
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null) return NotFound();
            if (reservation.UserId != User.GetUserId() && !IsAdmin) return Forbid();

            if (!IsCancellable(reservation))
            {
                TempData["Error"] = "此預約已開始或已結案，無法取消";
                return RedirectToPage("./Index");
            }

            reservation.Status = ReservationStatus.Cancelled;
            await _context.SaveChangesAsync();
            TempData["Success"] = "已取消預約，該時段已釋出給其他人";
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int? id)
        {
            if (!IsAdmin) return Forbid();
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservation.Remove(reservation);
                await _context.SaveChangesAsync();
                TempData["Success"] = "已永久刪除該筆預約紀錄";
            }
            return RedirectToPage("./Index");
        }

        private static bool IsCancellable(Reservation r) =>
            (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Approved)
            && r.StartTime > DateTime.Now;
    }
}
