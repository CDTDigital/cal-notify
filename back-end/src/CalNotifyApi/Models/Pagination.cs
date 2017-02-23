using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalNotifyApi.Models
{
    /// <summary>
    /// Allows users to modify the result of a query for a list of objects.
    /// Provides sorting, and pagination abilities.
    /// </summary>
    [DataContract]
    public class Pagination
    {
        public Pagination()
        {
            Offset = 0;
            Limit = null;
        }

        /// <summary>
        ///     Provides a way to pagniate through the records
        /// </summary>
        /// <param name="offset">The number of records to skip</param>
        /// <param name="limit">Max records to return</param>
        public Pagination(int offset, int? limit)
        {
            Offset = offset;
            Limit = limit;
            
        }

        /// <summary>
        ///     The number of records to skip
        /// </summary>
        [DataMember(Name = "offset")]
        public int Offset { get; set; }

        /// <summary>
        ///     Max records to return
        /// </summary>
        [DataMember(Name = "limit")]
        public int? Limit { get; set; }

        /// <summary>
        /// Performs pagination through a query from our database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<T> SkipAndTake<T>(IQueryable<T> query)
        {
            if (Offset != 0)
                query = query.Skip(Offset);

            if (Limit.HasValue)
                query = query.Take(Limit.Value);
            return query;
        }
    }

    /// <summary>
    /// Options which can filter and sort a list of users
    /// </summary>
    [DataContract]
    public class UserFilteringOptions : Pagination
    {

        public UserFilteringOptions()
        {
          Sort = SortOption.Default;
        }

        /// <summary>
        ///     *Not Implemented*: Sorts on the user property
        /// </summary>
        [DataMember(Name = "sort")]
        public SortOption Sort { get; set; }


        /// <summary>
        /// Provides a way to sort users based on a fixed set of criteria
        /// See <see cref="SortOption"/> for the possible ways to sort
        /// </summary>
        /// <param name="query">The users to sort on</param>
        /// <returns></returns>
        public IQueryable<GenericUser> SortUsers(IQueryable<GenericUser> query)
        {
            if (Sort == SortOption.JoinDate)
                query = query.OrderBy(u => u.JoinDate);

            if (Sort == SortOption.JoinDateDesc)
                query = query.OrderByDescending(u => u.JoinDate);


            return query;
        }
    }


    [JsonConverter(typeof(StringEnumConverter))]
    public enum SortOption
    {
        Default,
        /// <summary>
        /// Sort based on the user's name
        /// </summary>
        Name,

        /// <summary>
        /// Shows the oldest users first
        /// </summary>
        JoinDate,

        /// <summary>
        /// Shows the most recently joined users first
        /// </summary>
        JoinDateDesc
    }
}