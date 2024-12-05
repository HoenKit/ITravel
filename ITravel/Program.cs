using ITravel.Data;
using ITravel.Models;
using ITravel.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true; // Customize sign-in options here
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultUI()
.AddDefaultTokenProviders();


builder.Services.AddRazorPages();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ElevatedRights", policy =>
          policy.RequireRole("Administrator", "Users", "Provider"));
});


builder.Services.ConfigureApplicationCookie(options => {

    options.LoginPath = "/Login/";
    options.LogoutPath = "/Logout/"; // Customize logout path
    options.AccessDeniedPath = "/AccessDenied/"; // Customize access denied path
    options.SlidingExpiration = false; // Disable sliding expiration if not needed
});
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<IdentityOptions>(options =>
{
    // Configure Password settings
    options.Password.RequireDigit = true; // No need for digits
    options.Password.RequireLowercase = true; // No need for lowercase
    options.Password.RequireNonAlphanumeric = false; // No special characters
    options.Password.RequireUppercase = true; // No uppercase required
    options.Password.RequiredLength = 6; // Minimum length 3 characters
    options.Password.RequiredUniqueChars = 1; // At least 6 unique character

    // Configure Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Lock for 5 minutes
    options.Lockout.MaxFailedAccessAttempts = 5; // Lock after 5 failed attempts
    options.Lockout.AllowedForNewUsers = true;

    // Configure User settings
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true; // Unique email required

    // Configure Sign-in settings
    options.SignIn.RequireConfirmedEmail = true; // Confirm email is required
    options.SignIn.RequireConfirmedPhoneNumber = false; // Phone confirmation not required
    options.SignIn.RequireConfirmedAccount = true; // Confirmed account required


});
builder.Services.AddSingleton<IEmailSender, SendMailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
//
app.UseDeveloperExceptionPage();

app.Run();
