using Shopping.Data;

namespace Shopping.Utils
{
    public class LoadData
    {
        internal static void SeedData(WebApplication app)
        {
            IServiceScopeFactory? scopedFactory = app.Services.GetService<IServiceScopeFactory>();
            using IServiceScope? scope = scopedFactory!.CreateScope();
            SeedDb? service = scope.ServiceProvider.GetService<SeedDb>();
            service!.SeedAsync().Wait();
        }
    }
}
