using Microsoft.EntityFrameworkCore;
using CinemaSystem.Infrastructure.Persistence;
using CinemaSystem.API.Hubs;
using CinemaSystem.API.Middlewares;
using CinemaSystem.API.Services;
using CinemaSystem.Application;
using CinemaSystem.Application.Common.Interfaces;
using CinemaSystem.Application.Interfaces;
using CinemaSystem.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication("Bearer").AddJwtBearer();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IQRGeneratorService, QRCoderService>();
builder.Services.AddScoped<IPaymentService, PaymobPaymentService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISeatRealTimeNotifier, SignalRSeatNotifier>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policyBuilder => policyBuilder
            .WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<CinemaDbContext>();
        await context.Database.MigrateAsync();
        await CinemaDataSeeder.SeedAsync(context);
    }

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<CinemaHub>("/hubs/cinema");

app.Run();
