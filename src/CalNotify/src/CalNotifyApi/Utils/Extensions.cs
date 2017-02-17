using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;
using CalNotify.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.PlatformAbstractions;

namespace CalNotify.Utils
{
    public static class Extensions
    {
        public static void ThrowIfNull<T>(this T o, string paramName) where T : class
        {
            if (o == null)
                throw new ArgumentNullException(paramName);
        }


     
        public static double FromMilesToMeters(this double miles)
        {
            return miles * 1609.34;
        }

        public static double FromMetersToMiles(this double meters)
        {
            return meters * 0.000621371;
        }
       

        public static object GetReflectedProperty(this object obj, string propertyName)
        {
            obj.ThrowIfNull("obj");
            propertyName.ThrowIfNull("propertyName");

            var property = obj.GetType().GetProperty(propertyName);

            return property?.GetValue(obj, null);
        }

        /// <summary>
        /// Converts string to enum value (opposite to Enum.ToString()).
        /// </summary>
        /// <typeparam name="T">Type of the enum to convert the string into.</typeparam>
        /// <param name="s">string to convert to enum value.</param>
        public static T ToEnum<T>(this string s) where T : struct
        {
            T newValue;
            return Enum.TryParse(s, out newValue) ? newValue : default(T);
        }

        /// <summary>
        /// see http://stackoverflow.com/a/24520014/1403990
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbSet"></param>
        public static void Clear<T>(this DbSet<T> dbSet) where T : class
        {
            dbSet.RemoveRange(dbSet);
        }


        public static BusinessDbContext GetBusinessDbContext(this ValidationContext validationContext)
        {
            return (BusinessDbContext)validationContext.GetService(typeof(BusinessDbContext));
        }


        /// <summary>
        /// Gets the full path to the target project path that we wish to test
        /// </summary>
        /// <param name="solutionRelativePath">
        /// The parent directory of the target project.
        /// e.g. src, samples, test, or test/Websites
        /// </param>
        /// <returns>The full path to the target project.</returns>
        public static string GetProjectPath(string solutionRelativePath)
        {


            // Get currently executing test project path
            var applicationBasePath = PlatformServices.Default.Application.ApplicationBasePath;

            // Find the folder which contains the solution file. We then use this information to find the target
            // project which we want to test.
            var directoryInfo = new DirectoryInfo(applicationBasePath);
            do
            {
                var solutionFileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, Constants.SolutionName));
                if (solutionFileInfo.Exists)
                {
                    return Path.GetFullPath(Path.Combine(directoryInfo.FullName, solutionRelativePath, Constants.ProjectName));
                }

                directoryInfo = directoryInfo.Parent;
            } while (directoryInfo.Parent != null);

            throw new Exception($"Solution root could not be located using application root {applicationBasePath}.");
        }
    }

    

}
