using GiaSuService.AppDbContext;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<TmdtDvgsContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TutorConnection") ?? throw new InvalidOperationException("Connection string 'TutorConnection' not found.")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
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
            Passwordhash = "superadmin",
            Logoaccount = "https://media.tenor.com/RtmcggFXF04AAAAe/cat-kitten.png"
        }
        );

        await context.SaveChangesAsync();
    }

}