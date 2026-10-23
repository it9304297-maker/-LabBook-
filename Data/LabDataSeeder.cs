using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RazorPageBase.Models;

namespace RazorPageBase.Data;

/// <summary>
/// 建立示範用預約資料（只在資料庫沒有任何預約時執行），方便錄影展示
/// </summary>
public static class LabDataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RazorPageBaseContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (await db.Reservation.AnyAsync()) return;

        var user = await userManager.FindByEmailAsync("user1@pccu.com");
        if (user is null) return;

        var instruments = await db.Instrument.ToDictionaryAsync(i => i.Id);
        if (instruments.Count == 0) return;

        var today = DateTime.Today;
        Reservation Make(int instrumentId, int dayOffset, int startHour, int hours,
                         string project, ReservationStatus status, string? note = null)
        {
            var start = today.AddDays(dayOffset).AddHours(startHour);
            var end = start.AddHours(hours);
            return new Reservation
            {
                InstrumentId = instrumentId,
                UserId = user.Id,
                UserEmail = user.Email ?? "",
                ProjectTitle = project,
                Advisor = "王教授",
                SampleCount = 6,
                StartTime = start,
                EndTime = end,
                Purpose = "示範資料",
                Status = status,
                AdminNote = note,
                TotalFee = instruments[instrumentId].HourlyRate * hours,
                CreatedAt = start.AddDays(-3)
            };
        }

        db.Reservation.AddRange(
            Make(1, -6, 9, 4, "天然物萃取成分定量", ReservationStatus.Completed),
            Make(3, -3, 13, 2, "奈米觸媒表面形貌鑑定", ReservationStatus.Completed),
            Make(2, 1, 10, 2, "染料降解動力學", ReservationStatus.Approved, "請準時到場"),
            Make(5, 2, 14, 3, "抗氧化基因表現量分析", ReservationStatus.Pending),
            Make(1, 3, 9, 5, "天然物萃取成分定量", ReservationStatus.Pending),
            Make(6, -1, 15, 1, "高分子薄膜官能基鑑定", ReservationStatus.Rejected, "樣品未附安全資料表 (SDS)")
        );
        await db.SaveChangesAsync();
    }
}
