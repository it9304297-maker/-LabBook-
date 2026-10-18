using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace RazorPageBase.Data;
// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [MaxLength(100)]
    public string? FullName { get; set; }

    public DateTime CreatedAtUtc { get; set; } =
        DateTime.UtcNow;
}
