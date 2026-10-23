using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using RazorPageBase.Areas.Identity;
using RazorPageBase.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("RazorPageBaseContext") ?? throw new InvalidOperationException("Connection string 'RazorPageBaseContext' not found.");

builder.Services.AddDbContext<RazorPageBaseContext>(options => options.UseSqlServer(connectionString));


// 2. Identity
builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        // Password policy
        options.Password.RequiredLength = 4;
        options.Password.RequiredUniqueChars = 0;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;

        // Account confirmation
        options.SignIn.RequireConfirmedAccount = true;  // 講義第7頁：開啟註冊 Email 驗證
        options.SignIn.RequireConfirmedEmail = true; //True for send email to confirm

        // Lockout
        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(15);

        // User
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<RazorPageBaseContext>()
    .AddErrorDescriber<LocalizedIdentityErrorDescriber>(); // 加上這一行for server side log localization MVC?


//2.1.註冊基礎當地語系化服務（可指定資源檔路徑，例如：Resources）
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");



// 3. Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "__Host-MyApp.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.Path = "/";

    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath =
        "/Identity/Account/AccessDenied";
});




// 4. Email sender（講義第8頁：把註解拿掉）
// Gmail 帳號與應用程式密碼請放在「管理使用者密碼」(secrets.json)，不要寫在 appsettings.json
builder.Services.AddTransient<IEmailSender, EmailSender>();

// 5. Razor Pages
builder.Services.AddRazorPages(options =>
{
    // 只有 Admin 角色可以進入 /Admin 下的頁面
    options.Conventions.AuthorizeFolder(
        "/Admin", "AdminOnly");

    // 儀器預約系統權限：預約需登入；儀器新增/修改/刪除限管理員
    options.Conventions.AuthorizeFolder("/ReservationPages");
    options.Conventions.AuthorizePage("/InstrumentPages/Create", "AdminOnly");
    options.Conventions.AuthorizePage("/InstrumentPages/Edit", "AdminOnly");
    options.Conventions.AuthorizePage("/InstrumentPages/Delete", "AdminOnly");
}).AddViewLocalization();   //註冊 Razor Pages 服務，並鏈結 View .cshtml 本地化支援

// 5.1. 註冊 MVC 服務，並鏈結 View 本地化支援
//builder.Services.AddControllersWithViews().AddViewLocalization(); // ✨ 必須加入這行


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


//放在 app.UseRouting() 與 app.UseAuthorization() 之前，否則當地的語系判定不會生效。 request參數的localization 
app.UseRequestLocalization();


app.UseRouting();

app.UseAuthentication();
 
app.UseAuthorization();

app.MapStaticAssets();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
//app.MapRazorPages()
//  .WithStaticAssets();

// 5.9 啟動時自動套用 Migration（等同 Update-Database），資料庫不存在會自動建立
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RazorPageBaseContext>();
    await db.Database.MigrateAsync();
}

// 6. Seed roles
await IdentitySeeder.SeedRolesAsync(app.Services);

// 7. Seed demo reservations (儀器預約示範資料)
await LabDataSeeder.SeedAsync(app.Services);

 


app.Run();
