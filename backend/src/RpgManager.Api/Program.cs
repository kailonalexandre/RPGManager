using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using RpgManager.Api.Extensions;
using RpgManager.Infrastructure;
using RpgManager.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration["DATABASE_URL"]
    ?? builder.Configuration["ConnectionStrings__DefaultConnection"];

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
    
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(GetAllowedOrigins(builder.Configuration))
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Frontend");
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

static string[] GetAllowedOrigins(IConfiguration configuration)
{
    var allowedOrigins = configuration["AllowedOrigins"];
    if (!string.IsNullOrWhiteSpace(allowedOrigins))
    {
        return allowedOrigins
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToArray();
    }

    return configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
}
