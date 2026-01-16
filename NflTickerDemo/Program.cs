using SportsTickerDemo;
using SportsTickerDemo.Services;

var builder = WebApplication.CreateBuilder(args);

// Bind options from configuration
builder.Services.Configure<SportsTickerOptions>(builder.Configuration.GetSection("SportsTicker"));

// Razor Pages
builder.Services.AddRazorPages();

// MVC controllers (needed for ScoresController)
builder.Services.AddControllersWithViews();

// Caching
builder.Services.AddMemoryCache();

// Register tickers services and typed HttpClients with resilience policies
builder.Services.AddSportsTickers(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Map Razor Pages
app.MapRazorPages();

// Map MVC controllers with conventional routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();