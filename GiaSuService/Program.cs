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

var datasourceBuilder = new Npgsql.NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("TutorConnection") ?? throw new InvalidOperationException("Connection string 'TutorConnection' not found."));

builder.Services.AddDbContext<DvgsDbContext>(options =>
    options.UseNpgsql(datasourceBuilder.Build())
        .EnableServiceProviderCaching(false));
builder.Services.AddAuthentication().AddCookie(AppConfig.AUTHSCHEME, o =>
{
    o.ExpireTimeSpan = TimeSpan.FromMinutes(15);
    o.LoginPath = "/Identity/Index";
    o.LogoutPath = "/Identity/Logout";
    o.AccessDeniedPath = "/";
    o.Cookie.MaxAge = TimeSpan.FromMinutes(15);
});
builder.Services.AddAuthorization(o =>
{
    o.AddPolicy(AppConfig.ADMINPOLICY, policy =>
    {
        policy.AddAuthenticationSchemes(AppConfig.AUTHSCHEME);
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new ShouldRoleRequire(AppConfig.ADMINROLENAME));
    });

    o.AddPolicy(AppConfig.EMPLOYEEPOLICY, policy =>
    {
        policy.AddAuthenticationSchemes(AppConfig.AUTHSCHEME);
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new ShouldRoleRequire(AppConfig.EMPLOYEEROLENAME));
    });

    o.AddPolicy(AppConfig.CUSTOMERPOLICY, policy =>
    {
        policy.AddAuthenticationSchemes(AppConfig.AUTHSCHEME);
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new ShouldRoleRequire(AppConfig.CUSTOMERROLENAME));
    });

    o.AddPolicy(AppConfig.TUTORPOLICY, policy =>
    {
        policy.AddAuthenticationSchemes(AppConfig.AUTHSCHEME);
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new ShouldRoleRequire(AppConfig.TUTORROLENAME));
    });
});

//Add Repository
builder.Services.AddTransient<IAccountRepo, AccountRepo>();
builder.Services.AddTransient<IAddressRepo, AddressRepo>();
builder.Services.AddTransient<IProfileRepo, ProfileRepo>();
builder.Services.AddTransient<ICategoryRepo, CategoryRepo>();
builder.Services.AddTransient<IStatusRepo, StatusRepo>();
builder.Services.AddTransient<ITutorRepo, TutorRepo>();
//builder.Services.AddTransient<ISubjectRepository, SubjectRepository>();
//builder.Services.AddTransient<ITutorRequestFormRepository, TutorRequestFormRepository>();


//Add Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<ITutorService, TutorService>();
//builder.Services.AddScoped<ITutorRequestFormService,  TutorRequestFormService>();
builder.Services.AddScoped<IAuthorizationHandler, ShouldBeAdminRequirementAuthorization>();
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
    var context = scope.ServiceProvider.GetRequiredService<DvgsDbContext>();
    await GenSuperAdmin(context);
}

app.Run();



static async Task GenSuperAdmin(DvgsDbContext context)
{
    if (!context.Accounts.Any())
    {
        context.Accounts.Add(new
            GiaSuService.EntityModel.Account
        {
            Email = "superadmin@gmail.com",
            Phone = "0868273914",
            Passwordhash = BCrypt.Net.BCrypt.HashPassword("superadmin"),
            Lockenable = false,
            Avatar = "https://media.tenor.com/RtmcggFXF04AAAAe/cat-kitten.png",
            Createdate = DateTime.Now,
            Roleid = 1,
            
            Employee = new GiaSuService.EntityModel.Employee()
            {
                Addressdetail = "Nowhere",
                Birth = new DateOnly(2002, 05, 31),
                Fullname = "SuperAdmin Nguyen",
                Gender = "M",
                Districtid = 568,
                Identity = new GiaSuService.EntityModel.Identitycard()
                {
                    Identitynumber = "123456789",
                    Frontidentitycard = "https://media.tenor.com/RtmcggFXF04AAAAe/cat-kitten.png",
                    Backidentitycard = "https://media.tenor.com/RtmcggFXF04AAAAe/cat-kitten.png",

                }
            },
        }
        );

        await context.SaveChangesAsync();
    }

}