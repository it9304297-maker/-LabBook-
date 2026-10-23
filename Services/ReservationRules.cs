using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using RazorPageBase.Extensions;
using RazorPageBase.Models;

namespace RazorPageBase.Services;

/// <summary>
/// 預約業務規則（商業邏輯集中在這裡，新增、修改、審核都共用）
/// 1. 儀器必須是「開放預約」狀態
/// 2. 單次使用時數不可超過儀器上限
/// 3. 不可預約過去的時間
/// 4. 同一台儀器同一時段不可重複預約（與待審核/已核准的預約比對）
/// 5. 費用 = 每小時收費 × 使用時數
/// </summary>
public static class ReservationRules
{
    public static readonly ReservationStatus[] ActiveStatuses =
        [ReservationStatus.Pending, ReservationStatus.Approved];

    public static async Task<Instrument?> ValidateAsync(
        RazorPageBaseContext db,
        Reservation r,
        ModelStateDictionary modelState,
        string prefix = "Reservation",
        bool checkPastTime = true)
    {
        var instrument = await db.Instrument.AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == r.InstrumentId);

        if (instrument is null)
        {
            modelState.AddModelError($"{prefix}.InstrumentId", "找不到此儀器");
            return null;
        }

        if (instrument.Status != InstrumentStatus.Available)
        {
            modelState.AddModelError($"{prefix}.InstrumentId",
                $"「{instrument.Name}」目前為{GetStatusText(instrument.Status)}，暫不開放預約");
        }

        if (r.EndTime > r.StartTime && (r.EndTime - r.StartTime).TotalHours > instrument.MaxHoursPerBooking)
        {
            modelState.AddModelError($"{prefix}.EndTime",
                $"「{instrument.Name}」單次最多只能預約 {instrument.MaxHoursPerBooking} 小時");
        }

        if (checkPastTime && r.StartTime < DateTime.Now)
        {
            modelState.AddModelError($"{prefix}.StartTime", "不可預約已經過去的時間");
        }

        var conflict = await FindConflictAsync(db, r);
        if (conflict is not null)
        {
            modelState.AddModelError(string.Empty,
                $"時段衝突：此儀器在 {conflict.StartTime:MM/dd HH:mm}–{conflict.EndTime:HH:mm} 已被預約，請改選其他時段");
        }

        return instrument;
    }

    /// <summary>找出同一台儀器時間重疊的有效預約（排除自己）</summary>
    public static Task<Reservation?> FindConflictAsync(RazorPageBaseContext db, Reservation r,
        bool approvedOnly = false)
    {
        // 用 List 而非陣列：避免 C# 14 把陣列 Contains 解析成 Span 版本，EF 無法轉成 SQL
        List<ReservationStatus> statuses = approvedOnly
            ? [ReservationStatus.Approved]
            : [.. ActiveStatuses];
        return db.Reservation.AsNoTracking()
            .Where(x => x.InstrumentId == r.InstrumentId
                     && x.Id != r.Id
                     && statuses.Contains(x.Status)
                     && x.StartTime < r.EndTime
                     && r.StartTime < x.EndTime)
            .OrderBy(x => x.StartTime)
            .FirstOrDefaultAsync();
    }

    public static decimal CalculateFee(Instrument instrument, Reservation r)
    {
        var hours = (decimal)(r.EndTime - r.StartTime).TotalHours;
        return Math.Round(instrument.HourlyRate * hours, 0);
    }

    private static string GetStatusText(InstrumentStatus s) => s.GetDisplayName();
}
