using Microsoft.EntityFrameworkCore;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Application.Mapping;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Data;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Repositories;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Services;
using QRCodeBasedMetroTicketingSystem.Web.Mapping;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Redis
var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnectionString")
                            ?? throw new InvalidOperationException("Redis connection string is missing.");
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisConnectionString));

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile), typeof(ViewModelMappingProfile));

// Register repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IStationRepository, StationRepository>();
builder.Services.AddScoped<IStationDistanceRepository, StationDistanceRepository>();
builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();

// Register other services
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<IStationService, StationService>();
builder.Services.AddScoped<IDistanceCalculationService, DistanceCalculationService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();

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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "Admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}",
    defaults: new { area = "Admin" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
