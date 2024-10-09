using ServicesMonitorApplicationBackend.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add CORS services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("SpecificOriginPolicy", builder =>
    {
       builder.WithOrigins("http://service1:8199") 
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.TypeInfoResolver = AppJsonSerializerContext.Default;
    });

var app = builder.Build();

// Use CORS policy
app.UseCors("SpecificOriginPolicy");

app.MapControllers();

app.Run();

[JsonSerializable(typeof(ServiceInfo))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
