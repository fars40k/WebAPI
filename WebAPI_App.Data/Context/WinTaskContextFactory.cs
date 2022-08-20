using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI_App.Data
{
    public class WinTaskContextFactory : IDbContextFactory<WinTaskContext>
    {
        public WinTaskContext Create()
        {
            return new WinTaskContext("server = (localdb)\\MSSQLLocalDB; initial catalog = WinTaskManager; " +
                "integrated security = True; MultipleActiveResultSets = True; App = EntityFramework;");
        }
    }
}
