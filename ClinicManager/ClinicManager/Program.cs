using ClinicManager.Data;
using ClinicManager.Models;
using ClinicManager.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region DB + EF CORE
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
#endregion

#region IDENTITY + ROLE
//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();
#endregion

#region AUTH COOKIE
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});
#endregion

#region MVC
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
#endregion

#region SERVICES (BUSINESS)
builder.Services.AddScoped<IKhamBenhService, KhamBenhService>();
builder.Services.AddScoped<IBuoiDieuTriService, BuoiDieuTriService>();
builder.Services.AddScoped<IChamCongService, ChamCongService>();
builder.Services.AddScoped<IChamCongAdminService, ChamCongAdminService>();
#endregion

var app = builder.Build();

#region MIDDLEWARE
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
#endregion

#region ROUTING
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
#endregion

#region SEED DATA (ROLE + ADMIN + GOI MAC DINH)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Seed Role + Admin
    await RoleInitializer.SeedAsync(services);

    // Seed Goi dieu tri mac dinh
    var context = services.GetRequiredService<ApplicationDbContext>();
    DbInitializer.Seed(context);
}
#endregion
app.Run();
