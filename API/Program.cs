using Application;
using Application.SeedData;
using Infrastructure;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationModule();
builder.Services.AddPersistenceModule(configuration);
builder.Services.AddInfrastructureModule(configuration);

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
    using (IServiceScope scope = app.Services.CreateScope())
    {
        IServiceProvider serviceProvider = scope.ServiceProvider;
        SeedDataService seedDataService = serviceProvider.GetRequiredService<SeedDataService>();
        await seedDataService.RunAsync(new SeedDataRq());
    }
}
catch (Exception ex)
{
    // Singleton service so can use here without CreateScope
    ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while seeding data.");
}

app.Run();
