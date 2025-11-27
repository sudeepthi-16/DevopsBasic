using Prometheus;
using DevopsBasic.Middleware;

var builder = WebApplication.CreateBuilder(args);

// No database � in-memory student store only
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS for Angular dev on http://localhost:4200
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("dev", p => p
        .WithOrigins("http://localhost:8080")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();



// Expose /metrics endpoint automatically
app.UseMetricServer();     // <-- exposes http://localhost:5000/metrics
app.UseHttpMetrics();      // <-- collects HTTP request metrics


app.UseMiddleware<RequestLoggingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("dev");






app.MapControllers();

app.Run();
