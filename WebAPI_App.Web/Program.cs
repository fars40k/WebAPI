using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using WebAPI_App.Data;

namespace WebAPI_App.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {  
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
