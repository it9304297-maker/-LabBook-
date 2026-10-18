using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPageBase.Data;
 
using System.ComponentModel.DataAnnotations;

namespace RazorPageBase.Areas.Identity.Pages.Account.Manage.UI;

[Authorize(Roles = "Admin")]
public class RolesModel : PageModel
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RolesModel(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    [BindProperty]
    public CreateRoleInput Input { get; set; } = new();

    public List<RoleRow> Roles { get; private set; } = [];

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        var result = await _roleManager.CreateAsync(new IdentityRole(Input.Name.Trim()));
        StatusMessage = result.Succeeded
            ? "角色已建立。"
            : string.Join("；", result.Errors.Select(error => error.Description));

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role is null)
        {
            return NotFound();
        }

        if (string.Equals(role.Name, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            StatusMessage = "內建 Admin 角色不可刪除。";
            return RedirectToPage();
        }

        var members = await _userManager.GetUsersInRoleAsync(role.Name!);
        if (members.Count > 0)
        {
            StatusMessage = "此角色仍有使用者，請先移除所有成員。";
            return RedirectToPage();
        }

        var result = await _roleManager.DeleteAsync(role);
        StatusMessage = result.Succeeded
            ? $"角色 {role.Name} 已刪除。"
            : string.Join("；", result.Errors.Select(error => error.Description));

        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        Roles = await _roleManager.Roles
            .OrderBy(role => role.Name)
            .Select(role => new RoleRow
            {
                Id = role.Id,
                Name = role.Name ?? ""
            })
            .ToListAsync();
    }

    public sealed class CreateRoleInput
    {
        [Required, StringLength(256)]
        public string Name { get; set; } = "";
    }

    public sealed class RoleRow
    {
        public string Id { get; init; } = "";
        public string Name { get; init; } = "";
    }
}
