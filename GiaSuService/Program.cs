using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.Handler;
using GiaSuService.Handler.MyCustomAuthorize;
using GiaSuService.Repository;
using GiaSuService.Repository.Interface;
using GiaSuService.Services;
using GiaSuService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var datasourceBuilder = new Npgsql.NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("TutorConnection") ?? throw new InvalidOperationException("Connection string 'TutorConnection' not found."));

builder.Services.AddDbContext<DvgsDbContext>(options =>
{
    options.UseNpgsql(datasourceBuilder.Build())
        .EnableServiceProviderCaching(false);
}
);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);


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
        policy.RequireRole(AppConfig.ADMINROLENAME);
    });

    o.AddPolicy(AppConfig.EMPLOYEEPOLICY, policy =>
    {
        policy.AddAuthenticationSchemes(AppConfig.AUTHSCHEME);
        policy.RequireAuthenticatedUser();
        policy.RequireRole(AppConfig.EMPLOYEEROLENAME);
    });

    o.AddPolicy(AppConfig.CUSTOMERPOLICY, policy =>
    {
        policy.AddAuthenticationSchemes(AppConfig.AUTHSCHEME);
        policy.RequireAuthenticatedUser();
        policy.RequireRole(AppConfig.CUSTOMERROLENAME);
    });

    o.AddPolicy(AppConfig.TUTORPOLICY, policy =>
    {
        policy.AddAuthenticationSchemes(AppConfig.AUTHSCHEME);
        policy.RequireAuthenticatedUser();
        policy.RequireRole(AppConfig.TUTORROLENAME);
    });

    o.AddPolicy(AppConfig.PROFILE_POLICY, policy =>
    {
        policy.AddAuthenticationSchemes(AppConfig.AUTHSCHEME);
        policy.RequireAuthenticatedUser();
        policy.RequireRole(AppConfig.ADMINROLENAME, AppConfig.EMPLOYEEROLENAME, AppConfig.CUSTOMERROLENAME, AppConfig.TUTORROLENAME);
    });
});

// Size of file upload
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 25 * 1024 * 1024; // 25 MB limit
});

//Add Repository
builder.Services.AddTransient<IAccountRepo, AccountRepo>();
builder.Services.AddTransient<IAddressRepo, AddressRepo>();
builder.Services.AddTransient<IProfileRepo, ProfileRepo>();
builder.Services.AddTransient<ICategoryRepo, CategoryRepo>();
builder.Services.AddTransient<IStatusRepo, StatusRepo>();
builder.Services.AddTransient<ITutorRepo, TutorRepo>();
builder.Services.AddTransient<ITutorRequestRepo, TutorRequestRepo>();
builder.Services.AddTransient<IQueueRepo, QueueRepo>();
builder.Services.AddTransient<ITransactionRepo, TransactionRepo>();


//Add Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<ITutorService, TutorService>();
builder.Services.AddScoped<ITutorRequestFormService,  TutorRequestFormService>();
builder.Services.AddScoped<ITransactionService,  TransactionService>();
builder.Services.AddScoped<IUploadFileService, UploadFileService>();
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
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("superadmin"),
            LockEnable = false,
            Avatar = "https://media.tenor.com/RtmcggFXF04AAAAe/cat-kitten.png",
            CreateDate = DateTime.Now,
            RoleId = 1,
            
            Employee = new GiaSuService.EntityModel.Employee()
            {
                AddressDetail = "Nowhere",
                Birth = new DateOnly(2002, 05, 31),
                FullName = "SuperAdmin Nguyen",
                Gender = "M",
                DistrictId = 568,
                Identity = new GiaSuService.EntityModel.IdentityCard()
                {
                    IdentityNumber = "123456789",
                    FrontIdentityCard = "https://media.tenor.com/RtmcggFXF04AAAAe/cat-kitten.png",
                    BackIdentityCard = "https://media.tenor.com/RtmcggFXF04AAAAe/cat-kitten.png",

                }
            },
        }
        );

        await context.SaveChangesAsync();
    }

}