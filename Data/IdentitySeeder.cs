using Microsoft.AspNetCore.Identity;

namespace RazorPageBase.Data;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = ["Admin", "User"];

        foreach (var role in roles)
        {
            if (await roleManager.RoleExistsAsync(role))
            {
                continue;
            }

            var result = await roleManager.CreateAsync(new IdentityRole(role));
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(error => error.Description));
                throw new InvalidOperationException(
                    $"Cannot create role '{role}': {errors}");
            }
        }

        await EnsureSeedUserAsync(
            userManager,
            email: "admin@pccu.com",
            password: "123456",
            fullName: "管理員,王",
            role: "Admin");

        await EnsureSeedUserAsync(
            userManager,
            email: "user1@pccu.com",
            password: "123456",
            fullName: "使用者,王",
            role: "User");
    }

    private static async Task EnsureSeedUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string fullName,
        string role)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true // 測試帳號直接設為已驗證，開啟 Email 驗證後才能登入
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    createResult.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"Cannot create seed user '{email}': {errors}");
            }
        }

        // 已存在但尚未驗證的測試帳號（開啟 Email 驗證前建立的）→ 補設為已驗證
        if (!user.EmailConfirmed)
        {
            user.EmailConfirmed = true;
            await userManager.UpdateAsync(user);
        }

        if (await userManager.IsInRoleAsync(user, role))
        {
            return;
        }

        var roleResult = await userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
        {
            var errors = string.Join(
                "; ",
                roleResult.Errors.Select(error => error.Description));

            throw new InvalidOperationException(
                $"Cannot add seed user '{email}' to role '{role}': {errors}");
        }
    }
}