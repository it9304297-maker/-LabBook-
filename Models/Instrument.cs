using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPageBase.Models;

/// <summary>
/// 商務物件：實驗室精密儀器
/// </summary>
public class Instrument
{
    public int Id { get; set; }

    [Display(Name = "儀器名稱")]
    [Required(ErrorMessage = "請輸入{0}")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "{0}長度需介於 {2} 到 {1} 個字")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "財產編號")]
    [Required(ErrorMessage = "請輸入{0}")]
    [RegularExpression(@"^[A-Za-z]{2,5}-\d{3,5}$", ErrorMessage = "{0}格式例：HPLC-001（英文-數字）")]
    [StringLength(20)]
    public string AssetCode { get; set; } = string.Empty;

    [Display(Name = "廠牌型號")]
    [StringLength(80)]
    public string? Brand { get; set; }

    [Display(Name = "儀器類別")]
    public InstrumentCategory Category { get; set; }

    [Display(Name = "放置地點")]
    [Required(ErrorMessage = "請輸入{0}")]
    [StringLength(60)]
    public string Location { get; set; } = string.Empty;

    [Display(Name = "每小時收費")]
    [Range(0, 100000, ErrorMessage = "{0}需介於 {1} 到 {2} 元")]
    [DataType(DataType.Currency)]
    [DisplayFormat(DataFormatString = "{0:N0}")]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal HourlyRate { get; set; }

    [Display(Name = "單次最長使用(小時)")]
    [Range(1, 24, ErrorMessage = "{0}需介於 {1} 到 {2} 小時")]
    public int MaxHoursPerBooking { get; set; } = 4;

    [Display(Name = "需受訓認證")]
    public bool RequiresTraining { get; set; }

    [Display(Name = "儀器狀態")]
    public InstrumentStatus Status { get; set; } = InstrumentStatus.Available;

    [Display(Name = "管理人")]
    [StringLength(30)]
    public string? Custodian { get; set; }

    // ch01 教學第 7 步「新增欄位」：新增後執行 Add-Migration AddExtension → Update-Database
    [Display(Name = "管理人分機")]
    [RegularExpression(@"^\d{3,5}$", ErrorMessage = "{0}請輸入 3–5 位數字")]
    [StringLength(5)]
    public string? Extension { get; set; }

    [Display(Name = "說明")]
    [DataType(DataType.MultilineText)]
    [StringLength(500)]
    public string? Description { get; set; }

    [Display(Name = "購置日期")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = false)]
    public DateTime PurchaseDate { get; set; } = DateTime.Today;

    // 導覽屬性：一台儀器有多筆預約 (1 對多)
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}

public enum InstrumentCategory
{
    [Display(Name = "光譜分析")] Spectroscopy,
    [Display(Name = "層析分離")] Chromatography,
    [Display(Name = "顯微影像")] Microscopy,
    [Display(Name = "熱分析")] Thermal,
    [Display(Name = "分子生物")] MolecularBiology,
    [Display(Name = "其他")] Other
}

public enum InstrumentStatus
{
    [Display(Name = "開放預約")] Available,
    [Display(Name = "維修保養中")] Maintenance,
    [Display(Name = "停用")] Retired
}
