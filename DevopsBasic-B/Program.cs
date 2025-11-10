using DevopsBasic.Data;
using Microsoft.EntityFrameworkCore;
using DevopsBasic.Middleware;

var builder = WebApplication.CreateBuilder(args);

// SQL Server via EF Core
var connStr = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connStr));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS for Angular dev on http://localhost:4200
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("dev", p => p
        .WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod());
});


var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("dev");
app.MapControllers();

// Auto-migrate & seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    if (!db.Students.Any())
    {
        db.Students.AddRange(
            new() { Name = "Asha", Dept = "CSE" },
            new() { Name = "Vikram", Dept = "ECE" }
        );
        db.SaveChanges();
    }
}
//app.Urls.Add("http://0.0.0.0:5000");

app.Run();
