using eVOL.API.Hubs;
using Microsoft.EntityFrameworkCore;
using Serilog;
using eVOL.API.Configuration;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentUserName()
    .Enrich.WithMachineName()
    .Enrich.WithProcessId()
    .Enrich.WithThreadId()
    .Enrich.WithCorrelationId()
    .WriteTo.Console()
    .WriteTo.Seq("http://evol.seq:80")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services
    .AddCaching(builder.Configuration)
    .AddDatabases(builder.Configuration)
    .AddPresentation()
    .AddAuthenticationAndAuthorization(builder.Configuration)
    .AddCorsService()
    .AddMapper()
    .AddScopedUseCases()
    .AddRateLimiterService();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseRateLimiter();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chat-hub");

app.Run();
