using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPageBase.Models;

/// <summary>
/// 商務物件：儀器預約單（交易紀錄）
/// </summary>
public class Reservation : IValidatableObject
{
    public int Id { get; set; }

    [Display(Name = "儀器")]
    [Range(1, int.MaxValue, ErrorMessage = "請選擇{0}")]
    public int InstrumentId { get; set; }

    [ValidateNever]
    [Display(Name = "儀器")]
    public Instrument? Instrument { get; set; }

    // 由伺服器端從 Identity 取得 (User.GetUserId())，不接受表單送入，防止竄改
    [BindNever, ValidateNever]
    [StringLength(450)]
    public string UserId { get; set; } = string.Empty;

    [BindNever, ValidateNever]
    [Display(Name = "預約人")]
    [StringLength(256)]
    public string UserEmail { get; set; } = string.Empty;

    [Display(Name = "研究計畫名稱")]
    [Required(ErrorMessage = "請輸入{0}")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "{0}長度需介於 {2} 到 {1} 個字")]
    public string ProjectTitle { get; set; } = string.Empty;

    [Display(Name = "指導教授")]
    [Required(ErrorMessage = "請輸入{0}")]
    [StringLength(30)]
    public string Advisor { get; set; } = string.Empty;

    [Display(Name = "樣品數")]
    [Range(1, 200, ErrorMessage = "{0}需介於 {1} 到 {2}")]
    public int SampleCount { get; set; } = 1;

    [Display(Name = "開始時間")]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
    public DateTime StartTime { get; set; }

    [Display(Name = "結束時間")]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
    public DateTime EndTime { get; set; }

    [Display(Name = "實驗目的/備註")]
    [DataType(DataType.MultilineText)]
    [StringLength(500)]
    public string? Purpose { get; set; }

    [BindNever]
    [Display(Name = "狀態")]
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

    [BindNever]
    [Display(Name = "預估費用")]
    [DisplayFormat(DataFormatString = "{0:N0}")]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalFee { get; set; }

    [BindNever]
    [Display(Name = "審核意見")]
    [StringLength(200)]
    public string? AdminNote { get; set; }

    [BindNever]
    [Display(Name = "申請時間")]
    [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [NotMapped]
    [Display(Name = "使用時數")]
    public double Hours => Math.Round((EndTime - StartTime).TotalHours, 1);

    /// <summary>跨欄位驗證（業務規則）</summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndTime <= StartTime)
        {
            yield return new ValidationResult("結束時間必須晚於開始時間",
                [nameof(EndTime)]);
        }
        if (StartTime.Minute % 30 != 0 || EndTime.Minute % 30 != 0)
        {
            yield return new ValidationResult("時間請以 30 分鐘為單位（例如 09:00、09:30）",
                [nameof(StartTime)]);
        }
        if (StartTime.Hour < 8 || EndTime.Hour > 22 || (EndTime.Hour == 22 && EndTime.Minute > 0)
            || EndTime.Date != StartTime.Date)
        {
            yield return new ValidationResult("開放時段為當日 08:00 – 22:00，不可跨日",
                [nameof(StartTime)]);
        }
    }

    /// <summary>判斷兩段時間是否重疊</summary>
    public bool OverlapsWith(DateTime start, DateTime end) => StartTime < end && start < EndTime;
}

public enum ReservationStatus
{
    [Display(Name = "待審核")] Pending,
    [Display(Name = "已核准")] Approved,
    [Display(Name = "已退回")] Rejected,
    [Display(Name = "已取消")] Cancelled,
    [Display(Name = "已完成")] Completed
}
