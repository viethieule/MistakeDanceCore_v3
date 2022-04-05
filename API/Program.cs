using API.Extensions;
using Application.Common;
using Application.SeedData;
using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var configuration = builder.Configuration;

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MistakeDanceDbContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScopedAsSelfByConvention(typeof(BaseService<,>).Assembly, type => type.Name.EndsWith("Service"));
builder.Services.AddScopedAsSelfByConvention(typeof(DTCBase<,>).Assembly, type => type.Name.EndsWith("DTC"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
    SeedDataService seedDataService = app.Services.GetRequiredService<SeedDataService>();
    await seedDataService.RunAsync(new SeedDataRq());
}
catch (Exception ex)
{
    ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while seeding data.");
}

app.Run();
