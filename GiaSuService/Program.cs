using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.Handler;
using GiaSuService.Handler.MyCustomAuthorize;
using GiaSuService.Repository;
using GiaSuService.Repository.Interface;
using GiaSuService.Services;
using GiaSuService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<TmdtDvgsContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TutorConnection") ?? throw new InvalidOperationException("Connection string 'TutorConnection' not found.")));
builder.Services.AddAuthentication().AddCookie(AppConfig.AUTHSCHEME, o =>
{
    o.Cookie.Expiration = TimeSpan.FromMinutes(15);
    o.ExpireTimeSpan = TimeSpan.FromMinutes(15);
    o.LoginPath = "/Identity/Index";
    o.LogoutPath = "/Identity/Logout";
    o.AccessDeniedPath = "/";
});
builder.Services.AddAuthorization(o =>
{
    o.AddPolicy(AppConfig.ADMINPOLICY, policy =>
    {
        policy.AddAuthenticationSchemes(AppConfig.AUTHSCHEME);
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new ShouldRoleRequire(AppConfig.ADMINROLENAME));
    });
});

//Add Repository
builder.Services.AddTransient<IAccountRepository, AccountRepository>();


//Add Services
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddSingleton<IAuthorizationHandler, ShouldBeAdminRequirementAuthorization>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//Setup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TmdtDvgsContext>();
    await GenSuperAdmin(context);
}

app.Run();



static async Task GenSuperAdmin(TmdtDvgsContext context)
{
    if (!context.Accounts.Any())
    {
        context.Accounts.Add(new
            GiaSuService.EntityModel.Account
        {
            Address = new GiaSuService.EntityModel.Address() { Addressname = "Nowhere", Districtid = 1 },
            Birth = new DateOnly(2002, 05, 31),
            Email = "superadmin@gmail.com",
            Fullname = "SuperAdmin Nguyen",
            Gender = "Nam",
            Identitycard = "123456789",
            Phone = "0869696969",
            Roleid = 1,
            Passwordhash = BCrypt.Net.BCrypt.HashPassword("superadmin"),
            Logoaccount = "https://media.tenor.com/RtmcggFXF04AAAAe/cat-kitten.png"
        }
        );

        await context.SaveChangesAsync();
    }

}