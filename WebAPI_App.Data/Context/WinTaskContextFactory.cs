using System.Data.Entity.Infrastructure;

namespace WebAPI_App.Data
{
    public class WinTaskContextFactory : IDbContextFactory<WinTaskContext>
    {
        public WinTaskContext Create()
        {
            WinTaskContext context = new WinTaskContext("server = (localdb)\\MSSQLLocalDB; initial catalog = WinTaskManager; " +
                "integrated security = True; MultipleActiveResultSets = True; App = EntityFramework;");

            return context;
        }
    }
}
