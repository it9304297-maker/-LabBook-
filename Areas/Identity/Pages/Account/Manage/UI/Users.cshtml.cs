using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPageBase.Data;

namespace RazorPageBase.Areas.Identity.Pages.Account.Manage.UI;

[Authorize(Roles = "Admin")]
public class UsersModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersModel(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [BindProperty]
    public CreateUserInput Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; } = 10;
    public int TotalPages { get; private set; } = 1;
    public int TotalUsers { get; private set; }

    public List<UserRow> Users { get; private set; } = [];
    public List<string> Roles { get; private set; } = [];

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

        var user = new ApplicationUser
        {
            UserName = Input.Email,
            Email = Input.Email,
            FullName = Input.FullName
        };

        var result = await _userManager.CreateAsync(user, Input.Password);
        if (!result.Succeeded)
        {
            AddErrors(result);
            await LoadAsync();
            return Page();
        }

        if (!string.IsNullOrWhiteSpace(Input.Role))
        {
            var roleResult = await _userManager.AddToRoleAsync(user, Input.Role);
            if (!roleResult.Succeeded)
            {
                AddErrors(roleResult);
                await LoadAsync();
                return Page();
            }
        }

        StatusMessage = "使用者已建立。";
        return RedirectToUsersPage();
    }

    public async Task<IActionResult> OnPostAddToRoleAsync(
        string userId,
        string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null || !await _roleManager.RoleExistsAsync(roleName))
        {
            return NotFound();
        }

        if (!await _userManager.IsInRoleAsync(user, roleName))
        {
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                StatusMessage = string.Join(
                    "；",
                    result.Errors.Select(error => error.Description));

                return RedirectToUsersPage();
            }
        }

        StatusMessage = $"已將 {user.Email} 加入角色 {roleName}。";
        return RedirectToUsersPage();
    }

    public async Task<IActionResult> OnPostRemoveFromRoleAsync(
        string userId,
        string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return NotFound();
        }

        // 避免管理員移除自己的 Admin 角色，造成自己失去管理權限。
        if (user.Id == _userManager.GetUserId(User) &&
            string.Equals(roleName, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            StatusMessage = "不能移除自己的 Admin 角色。";
            return RedirectToUsersPage();
        }

        if (await _userManager.IsInRoleAsync(user, roleName))
        {
            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                StatusMessage = string.Join(
                    "；",
                    result.Errors.Select(error => error.Description));

                return RedirectToUsersPage();
            }
        }

        StatusMessage = $"已將 {user.Email} 從角色 {roleName} 移除。";
        return RedirectToUsersPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return NotFound();
        }

        if (user.Id == _userManager.GetUserId(User))
        {
            StatusMessage = "不能刪除目前登入的管理員帳號。";
            return RedirectToUsersPage();
        }

        var result = await _userManager.DeleteAsync(user);
        StatusMessage = result.Succeeded
            ? $"使用者 {user.Email} 已刪除。"
            : string.Join("；", result.Errors.Select(error => error.Description));

        return RedirectToUsersPage();
    }

    private async Task LoadAsync()
    {
        Roles = await _roleManager.Roles
            .OrderBy(role => role.Name)
            .Select(role => role.Name!)
            .ToListAsync();

        SearchTerm = SearchTerm?.Trim();

        var usersQuery = _userManager.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            var searchTerm = SearchTerm;

            usersQuery = usersQuery.Where(user =>
                (user.FullName != null && user.FullName.Contains(searchTerm)) ||
                (user.Email != null && user.Email.Contains(searchTerm)));
        }

        TotalUsers = await usersQuery.CountAsync();
        TotalPages = Math.Max(
            1,
            (int)Math.Ceiling(TotalUsers / (double)PageSize));

        PageNumber = Math.Clamp(PageNumber, 1, TotalPages);

        var pageUsers = await usersQuery
            .OrderBy(user => user.Email)
            .ThenBy(user => user.Id)
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        Users = [];

        foreach (var user in pageUsers)
        {
            Users.Add(new UserRow
            {
                Id = user.Id,
                Email = user.Email ?? user.UserName ?? "(無 Email)",
                FullName = user.FullName ?? "",
                Roles = await _userManager.GetRolesAsync(user)
            });
        }
    }

    private IActionResult RedirectToUsersPage()
    {
        return RedirectToPage(new
        {
            searchTerm = SearchTerm,
            pageNumber = PageNumber
        });
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    public sealed class CreateUserInput
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, MinLength(4)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        public string FullName { get; set; } = "";
        public DateTimeOffset CreatedAtUtc { get; set; }
        public string? Role { get; set; }
    }

    public sealed class UserRow
    {
        public string Id { get; init; } = "";
        public string Email { get; init; } = "";
        public string Password { get; init; } = "";
        public string FullName { get; init; } = "";
        public DateTimeOffset CreatedAtUtc { get; init; }
        public IList<string> Roles { get; init; } = [];
    }
}