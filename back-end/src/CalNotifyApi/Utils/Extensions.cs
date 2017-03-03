using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using CalNotifyApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.PlatformAbstractions;

namespace CalNotifyApi.Utils
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

        public static bool IsPhone(this string phone)
        {
            var rgx = new Regex(@"^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$");
            return rgx.IsMatch(phone);
        }

        private static readonly Regex digitsOnly = new Regex(@"[^\d]");

        public static string CleanPhone(this string phone)
        {
            return digitsOnly.Replace(phone, "");
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
