using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPageBase.Extensions;
using RazorPageBase.Models;
using RazorPageBase.Services;

namespace RazorPageBase.Pages.Reservations
{
    public class IndexModel : PageModel
    {
        private readonly RazorPageBaseContext _context;

        public IndexModel(RazorPageBaseContext context)
        {
            _context = context;
        }

        public IList<Reservation> Reservation { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public ReservationStatus? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Scope { get; set; } = "upcoming"; // upcoming / history / all

        public bool IsAdmin => User.IsInRole("Admin");
        public Dictionary<ReservationStatus, int> StatusCounts { get; set; } = new();

        public async Task OnGetAsync()
        {
            // 管理員看全部；一般使用者只看自己的 (用 Identity 的 User ID 過濾)
            var userId = User.GetUserId();
            var baseQuery = _context.Reservation.Include(r => r.Instrument).AsNoTracking();
            if (!IsAdmin)
            {
                baseQuery = baseQuery.Where(r => r.UserId == userId);
            }

            StatusCounts = await baseQuery
                .GroupBy(r => r.Status)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.Count);

            var query = baseQuery;
            var now = DateTime.Now;
            query = Scope switch
            {
                "history" => query.Where(r => r.EndTime < now),
                "all" => query,
                _ => query.Where(r => r.EndTime >= now)
            };

            if (StatusFilter.HasValue)
            {
                query = query.Where(r => r.Status == StatusFilter.Value);
            }
            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                query = query.Where(r => r.ProjectTitle.Contains(SearchString)
                                      || r.UserEmail.Contains(SearchString)
                                      || r.Advisor.Contains(SearchString)
                                      || r.Instrument!.Name.Contains(SearchString));
            }

            query = Scope == "history"
                ? query.OrderByDescending(r => r.StartTime)
                : query.OrderBy(r => r.StartTime);

            Reservation = await query.ToListAsync();
        }

        // ===== 管理員審核 =====
        public async Task<IActionResult> OnPostApproveAsync(int id, string? note)
        {
            if (!IsAdmin) return Forbid();
            var r = await _context.Reservation.Include(x => x.Instrument).FirstOrDefaultAsync(x => x.Id == id);
            if (r is null) return NotFound();

            if (r.Status != ReservationStatus.Pending)
            {
                TempData["Error"] = "只有「待審核」的預約可以核准";
                return RedirectToPage(new { Scope, StatusFilter, SearchString });
            }

            // 核准前再檢查一次：不可與其他「已核准」預約衝突
            var conflict = await ReservationRules.FindConflictAsync(_context, r, approvedOnly: true);
            if (conflict is not null)
            {
                TempData["Error"] = $"無法核准：與 {conflict.UserEmail} 在 {conflict.StartTime:MM/dd HH:mm} 的已核准預約衝突";
                return RedirectToPage(new { Scope, StatusFilter, SearchString });
            }

            r.Status = ReservationStatus.Approved;
            r.AdminNote = string.IsNullOrWhiteSpace(note) ? r.AdminNote : note.Trim();
            await _context.SaveChangesAsync();
            TempData["Success"] = $"已核准 {r.UserEmail} 的「{r.Instrument?.Name}」預約";
            return RedirectToPage(new { Scope, StatusFilter, SearchString });
        }

        public async Task<IActionResult> OnPostRejectAsync(int id, string? note)
        {
            if (!IsAdmin) return Forbid();
            var r = await _context.Reservation.FindAsync(id);
            if (r is null) return NotFound();

            if (r.Status != ReservationStatus.Pending && r.Status != ReservationStatus.Approved)
            {
                TempData["Error"] = "此預約目前狀態無法退回";
                return RedirectToPage(new { Scope, StatusFilter, SearchString });
            }

            r.Status = ReservationStatus.Rejected;
            r.AdminNote = string.IsNullOrWhiteSpace(note) ? "未說明原因" : note.Trim();
            await _context.SaveChangesAsync();
            TempData["Success"] = "已退回該筆預約";
            return RedirectToPage(new { Scope, StatusFilter, SearchString });
        }

        public async Task<IActionResult> OnPostCompleteAsync(int id)
        {
            if (!IsAdmin) return Forbid();
            var r = await _context.Reservation.FindAsync(id);
            if (r is null) return NotFound();

            if (r.Status != ReservationStatus.Approved)
            {
                TempData["Error"] = "只有「已核准」的預約可以標記完成";
                return RedirectToPage(new { Scope, StatusFilter, SearchString });
            }

            r.Status = ReservationStatus.Completed;
            await _context.SaveChangesAsync();
            TempData["Success"] = "已標記為使用完成";
            return RedirectToPage(new { Scope, StatusFilter, SearchString });
        }
    }
}
