using OnlineShop.DataLayer.Data.Infrastructure;

namespace OnlineShop.DataLayer;

public class DbInitializer
{
    public static void InitializeDatabase(OnlineShopDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();
    }
}
