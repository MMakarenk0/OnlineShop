using OnlineShop.DataLayer;
using OnlineShop.DataLayer.Data.Infrastructure;
using DataLayer;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDataAccessLayer(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
