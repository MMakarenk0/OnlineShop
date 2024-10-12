using BLL.Extensions;
using DataLayer;
using OnlineShop.DataLayer;
using OnlineShop.DataLayer.Data.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDataAccessLayer(configuration);
builder.Services.AddBusinessLogicLayer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        var context = serviceProvider.GetRequiredService<OnlineShopDbContext>();
        DbInitializer.InitializeDatabase(context);

        // data seeding

        //var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        //var dataSeeder = new DataSeeder(unitOfWork);
        //await dataSeeder.Seed();
    }
    catch (Exception)
    {
        throw;
    }
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");
app.UseAuthorization();
app.MapControllers();

app.Run();
