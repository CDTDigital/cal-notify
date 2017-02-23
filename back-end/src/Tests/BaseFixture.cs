using System.IO;
using CalNotifyApi.Utils;
using Microsoft.Extensions.Configuration;

namespace Tests
{
    public class BaseFixture
    {
      
        public readonly string ContentRoot;

        public static  IConfigurationRoot Configuration;
        


        public BaseFixture()
        {
         
            ContentRoot =  Extensions.GetProjectPath(Path.Combine("src"));

            Configuration = new ConfigurationBuilder()
                .SetBasePath(ContentRoot)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.testing.json", optional: true)
                .AddEnvironmentVariables().Build();
        }

        public void Dispose() {}

      
    }
}
