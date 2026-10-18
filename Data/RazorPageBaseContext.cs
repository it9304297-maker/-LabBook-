using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class RazorPageBaseContext(DbContextOptions<RazorPageBaseContext> options) : IdentityDbContext<RazorPageBase.Data.ApplicationUser, IdentityRole, string>(options)
{
 
}
