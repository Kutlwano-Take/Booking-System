using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PracticeBookingSystem.Data;
using PracticeBookingSystem.Models;
using PracticeBookingSystem.Services;
using PracticeBookingSystem.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");               
    options.Conventions.AllowAnonymousToFolder("/Account"); 
});

builder.Services.AddScoped<PatientMemoryStore>();
builder.Services.AddDbContext<PracticeBookingDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication
builder.Services.AddScoped<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddScoped<IAppointmentService, EfAppointmentService>();
builder.Services.AddScoped<IPracticeDataService, PracticeDataService>();
builder.Services.AddSingleton<InMemoryEmailSender>();
builder.Services.AddSingleton<IEmailSender>(sp => sp.GetRequiredService<InMemoryEmailSender>());
builder.Services.AddScoped<INotificationService, NotificationService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<PracticeBookingDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<AppUser>>();

    db.Database.Migrate();  

    if (!db.Users.Any())
    {
        var user = new AppUser
        {
            Email = "thabiso@example.com",
            Username = "thabiso",
            DisplayName = "Thabiso Mokoena",
            JobTitle = "Practice administrator"
        };
        user.PasswordHash = hasher.HashPassword(user, "ChangeMe!123");
        db.Users.Add(user);
        db.SaveChanges();
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();   
app.UseAuthorization();    
app.MapRazorPages();

app.MapGet("/", (HttpContext ctx) => ctx.User.Identity?.IsAuthenticated == true ? Results.Redirect("/Appointments") : Results.Redirect("/Account/Login"));

app.Run();