using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EasyFitHub.Data;
using EasyFitHub.Services;
using Microsoft.Extensions.Configuration;
using Stripe;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EasyFitHubContext>(options => 
{
    //var conectionString = builder.Configuration.GetConnectionString("LocalEasyFitHubContext");
    var conectionString = builder.Configuration.GetConnectionString("EasyFitHubContext"); 
    options.UseSqlServer(conectionString
    ?? throw new InvalidOperationException("Connection string not found."));
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// Adiciona o serviço de sessão para as Coockies
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<ValidationService>();
builder.Services.AddScoped<AuthorizationService>();
builder.Services.AddScoped<StripeService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<FirebaseService>();


//StatisticsService.DelayTime = TimeSpan.FromMinutes(1); //For Test
builder.Services.AddHostedService<StatisticsService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Uso de Sessão para as Coockies
app.UseSession();

app.UseRouting();

//Stripe
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
