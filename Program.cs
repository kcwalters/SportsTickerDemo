// NBA scores service
builder.Services.AddHttpClient<NBAScoresService>();
builder.Services.AddScoped<SportsTickerDemo.Services.INBAScoresService, SportsTickerDemo.Services.NBAScoresService>();