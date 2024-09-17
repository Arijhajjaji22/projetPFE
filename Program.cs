using App_plateforme_de_recurtement.Data;
using App_plateforme_de_recurtement.Data.Repositories;
using App_plateforme_de_recurtement.Models;
using App_plateforme_de_recurtement.Repositories;
using App_plateforme_de_recurtement.Services;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// Configure Entity Framework Core
var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add scoped services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<StageOfferRepository>();
builder.Services.AddScoped<StageOfferService>();
builder.Services.AddScoped<AdminStageOfferRepository>();
builder.Services.AddScoped<AdminStageOfferService>();
builder.Services.AddScoped<MappingService>();
builder.Services.AddScoped<NotificationRepository>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TestService>();
builder.Services.AddScoped<TestRepository>();
builder.Services.AddScoped<FormRepository>();
builder.Services.AddScoped<FormService>();
builder.Services.AddScoped<AvailabilityRepository>();
builder.Services.AddScoped<AvailabilityService>();
builder.Services.AddScoped<AvailabilityConfirmationService>();
// Configure CORS policies
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowGoogle", policy =>
    {
        policy.WithOrigins("https://accounts.google.com")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configure SMTP settings
var smtpConfigSection = builder.Configuration.GetSection("SmtpConfig");
builder.Services.Configure<SmtpConfig>(smtpConfigSection);
builder.Services.AddScoped<EmailService>(provider =>
{
    var smtpConfig = provider.GetRequiredService<IOptions<SmtpConfig>>().Value;
    return new EmailService(smtpConfig.SmtpServer, int.Parse(smtpConfig.SmtpPort), smtpConfig.SmtpUsername, smtpConfig.SmtpPassword, smtpConfig.SenderEmail);
});

// Configuration Google OAuth
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");

    options.ClientId = googleAuthNSection["ClientId"];
    options.ClientSecret = googleAuthNSection["ClientSecret"];
    options.CallbackPath = "/signin-google";

});

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
builder.Logging.AddConsole();

// Add middleware to handle multipart forms
app.Use(next => async context =>
{
    if (context.Request.HasFormContentType)
    {
        await context.Request.ReadFormAsync();
    }
    await next(context);
});

app.UseRouting();
// Utilisation de la politique CORS définie

app.UseSession(); // Use session middleware
app.UseAuthentication(); // Ajoutez cette ligne pour utiliser l'authentification
app.UseAuthorization();

app.UseCors("AllowGoogle"); // Enable CORS using the policy
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
