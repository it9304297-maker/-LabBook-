using RazorPageBase.Models;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace RazorPageBase.Extensions;

public static class EnumExtensions
{
    /// <summary>取得列舉上 [Display(Name=...)] 的中文名稱</summary>
    public static string GetDisplayName(this Enum value)
    {
        var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        var attr = member?.GetCustomAttribute<DisplayAttribute>();
        return attr?.GetName() ?? value.ToString();
    }

    /// <summary>預約狀態對應的 Bootstrap 徽章顏色</summary>
    public static string BadgeClass(this ReservationStatus status) => status switch
    {
        ReservationStatus.Pending => "text-bg-warning",
        ReservationStatus.Approved => "text-bg-success",
        ReservationStatus.Rejected => "text-bg-danger",
        ReservationStatus.Cancelled => "text-bg-secondary",
        ReservationStatus.Completed => "text-bg-primary",
        _ => "text-bg-light"
    };

    /// <summary>儀器狀態對應的 Bootstrap 徽章顏色</summary>
    public static string BadgeClass(this InstrumentStatus status) => status switch
    {
        InstrumentStatus.Available => "text-bg-success",
        InstrumentStatus.Maintenance => "text-bg-warning",
        InstrumentStatus.Retired => "text-bg-secondary",
        _ => "text-bg-light"
    };

    /// <summary>儀器類別對應的 Bootstrap Icon</summary>
    public static string Icon(this InstrumentCategory category) => category switch
    {
        InstrumentCategory.Spectroscopy => "bi-rainbow",
        InstrumentCategory.Chromatography => "bi-funnel",
        InstrumentCategory.Microscopy => "bi-search",
        InstrumentCategory.Thermal => "bi-thermometer-half",
        InstrumentCategory.MolecularBiology => "bi-virus",
        _ => "bi-cpu"
    };
}
