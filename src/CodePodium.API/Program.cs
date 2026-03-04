using CodePodium.Core.Interfaces;
using CodePodium.Core.Services;
using CodePodium.Infrastructure.Data;
using CodePodium.Infrastructure.ExternalApi.Codeforces;
using CodePodium.Infrastructure.ExternalApi.Clist;
using CodePodium.Infrastructure.ExternalApi.LeetCode;
using CodePodium.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (!string.IsNullOrEmpty(databaseUrl))
        options.UseNpgsql(databaseUrl);
});

builder.Services.AddScoped<IContestRepository, ContestRepository>();

builder.Services.AddHttpClient<CodeforcesContestFetcher>();
builder.Services.AddScoped<IContestFetcher, CodeforcesContestFetcher>();
builder.Services.AddHttpClient<LeetCodeContestFetcher>();
builder.Services.AddScoped<IContestFetcher, LeetCodeContestFetcher>();

var clistApiKey = builder.Configuration["Clist:ApiKey"] ?? string.Empty;
var clistUsername = builder.Configuration["Clist:Username"] ?? string.Empty;
if (!string.IsNullOrEmpty(clistApiKey) && !string.IsNullOrEmpty(clistUsername))
{
    builder.Services.AddHttpClient();
    builder.Services.AddScoped<IContestFetcher>(sp =>
        new ClistContestFetcher(sp.GetRequiredService<IHttpClientFactory>().CreateClient(), clistApiKey, clistUsername));
}

builder.Services.AddScoped<ContestService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!string.IsNullOrEmpty(databaseUrl))
        db.Database.Migrate();
}

app.UseCors();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();

public partial class Program { }
