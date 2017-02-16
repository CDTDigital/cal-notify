using System.IO;
using Microsoft.Extensions.Configuration;

namespace Tests
{
    public class BaseFixture
    {
      
        public readonly string ContentRoot;

        public static  IConfigurationRoot Configuration;
        


        public BaseFixture()
        {
         
            ContentRoot =  CalNotify.Utils.Extensions.GetProjectPath(Path.Combine("src"));

            Configuration = new ConfigurationBuilder()
                .SetBasePath(ContentRoot)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.testing.json", optional: true)
                .AddEnvironmentVariables().Build();
        }

        public void Dispose() {}

      
    }
}
