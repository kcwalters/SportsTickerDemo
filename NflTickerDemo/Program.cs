using SportsTickerDemo.Services;
var builder = WebApplication.CreateBuilder(args);

// Razor Pages
builder.Services.AddRazorPages();

// MVC controllers (needed for ScoresController)
builder.Services.AddControllersWithViews();

// Typed HTTP client
// NFL
builder.Services.AddHttpClient<NFLScoresService>();
builder.Services.AddScoped<INFLScoresService, NFLScoresService>();

// NBA
builder.Services.AddHttpClient<NBAScoresService>();
builder.Services.AddScoped<INBAScoresService, NBAScoresService>();

// NHL
builder.Services.AddHttpClient<NHLScoresService>();
builder.Services.AddScoped<INHLScoresService, NHLScoresService>();
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