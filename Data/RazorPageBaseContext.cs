using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RazorPageBase.Models;

public class RazorPageBaseContext(DbContextOptions<RazorPageBaseContext> options) : IdentityDbContext<RazorPageBase.Data.ApplicationUser, IdentityRole, string>(options)
{
    // 老師講義：手動加上 Entity 的 DbSet
    public DbSet<Instrument> Instrument { get; set; } = default!;
    public DbSet<Reservation> Reservation { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // Identity 資料表需要，不可刪除

        builder.Entity<Instrument>()
            .HasIndex(i => i.AssetCode)
            .IsUnique();

        builder.Entity<Reservation>()
            .HasOne(r => r.Instrument)
            .WithMany(i => i.Reservations)
            .HasForeignKey(r => r.InstrumentId)
            .OnDelete(DeleteBehavior.Restrict); // 有預約紀錄的儀器不可直接刪除

        builder.Entity<Reservation>()
            .HasIndex(r => new { r.InstrumentId, r.StartTime, r.EndTime });

        builder.Entity<Reservation>()
            .HasIndex(r => r.UserId);

        // 種子資料：預設儀器
        builder.Entity<Instrument>().HasData(
            new Instrument { Id = 1, Name = "高效液相層析儀", AssetCode = "HPLC-001", Brand = "Agilent 1260 Infinity II", Category = InstrumentCategory.Chromatography, Location = "理學院 B205", HourlyRate = 800, MaxHoursPerBooking = 6, RequiresTraining = true, Status = InstrumentStatus.Available, Custodian = "陳助理", Extension = "2301", Description = "適用有機化合物分離與定量分析，需自備移動相。", PurchaseDate = new DateTime(2021, 3, 15) },
            new Instrument { Id = 2, Name = "紫外光-可見光分光光譜儀", AssetCode = "UV-002", Brand = "Shimadzu UV-1900i", Category = InstrumentCategory.Spectroscopy, Location = "理學院 B201", HourlyRate = 300, MaxHoursPerBooking = 4, RequiresTraining = false, Status = InstrumentStatus.Available, Custodian = "林助理", Extension = "2302", Description = "波長範圍 190–1100 nm，可做吸收光譜與動力學量測。", PurchaseDate = new DateTime(2022, 9, 1) },
            new Instrument { Id = 3, Name = "掃描式電子顯微鏡", AssetCode = "SEM-003", Brand = "JEOL JSM-IT510", Category = InstrumentCategory.Microscopy, Location = "貴儀中心 101", HourlyRate = 2500, MaxHoursPerBooking = 3, RequiresTraining = true, Status = InstrumentStatus.Available, Custodian = "張技正", Extension = "5101", Description = "表面形貌觀察，附 EDS 元素分析，樣品需先鍍金。", PurchaseDate = new DateTime(2020, 6, 20) },
            new Instrument { Id = 4, Name = "熱重分析儀", AssetCode = "TGA-004", Brand = "TA Instruments TGA 55", Category = InstrumentCategory.Thermal, Location = "理學院 B210", HourlyRate = 600, MaxHoursPerBooking = 8, RequiresTraining = false, Status = InstrumentStatus.Maintenance, Custodian = "陳助理", Extension = "2301", Description = "溫度範圍室溫至 1000°C，目前更換天平模組中。", PurchaseDate = new DateTime(2019, 11, 5) },
            new Instrument { Id = 5, Name = "即時定量 PCR", AssetCode = "QPCR-005", Brand = "Bio-Rad CFX Opus 96", Category = InstrumentCategory.MolecularBiology, Location = "生科館 3F 304", HourlyRate = 500, MaxHoursPerBooking = 4, RequiresTraining = true, Status = InstrumentStatus.Available, Custodian = "黃助理", Extension = "3304", Description = "96 孔，5 通道螢光偵測，適用基因表現分析。", PurchaseDate = new DateTime(2023, 2, 10) },
            new Instrument { Id = 6, Name = "傅立葉轉換紅外光譜儀", AssetCode = "FTIR-006", Brand = "Thermo Nicolet iS20", Category = InstrumentCategory.Spectroscopy, Location = "理學院 B201", HourlyRate = 400, MaxHoursPerBooking = 4, RequiresTraining = false, Status = InstrumentStatus.Available, Custodian = "林助理", Extension = "2302", Description = "附 ATR 配件，固體、液體樣品皆可直接量測。", PurchaseDate = new DateTime(2022, 4, 18) }
        );
    }
}
