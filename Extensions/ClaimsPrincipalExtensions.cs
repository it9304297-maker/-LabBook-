using System.Security.Claims;

namespace RazorPageBase.Extensions;

/// <summary>
/// 老師講義「方法三：包裝成擴充方法」：在任何有 User 的地方呼叫 User.GetUserId()
/// </summary>
public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
