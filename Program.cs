using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PatientRegistrationService.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configure EF Core to use SQLite
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repository and data context services for dependency injection
builder.Services.AddScoped<IPatientRepository, PatientRepository>();

// Configure controllers with JSON options to serialize enums as strings
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())); // Add this line

// Configure Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Patient Registration Service API",
        Version = "v1",
        Description = "An API for managing patient registration information"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Patient Registration Service API v1");
        c.RoutePrefix = string.Empty; // Load Swagger UI at the root
    });
}

app.UseAuthorization();
app.MapControllers();
app.Run();
