using System.Collections.Generic;

namespace Infrastructure.Data.Utilities
{
    public static class HashSetExtension
    {
        /// <summary>
        /// Convert an IEnumerable to a HashSet
        /// </summary>
        /// <typeparam name="TEntity">Object the the HashSet contains</typeparam>
        /// <param name="source">An IEnumerable to be converted to HashSet</param>
        /// <returns>HashSet of objects of type TEntity</returns>
        public static HashSet<TEntity> ToHashSet<TEntity>(this IEnumerable<TEntity> source)
        {
            return new HashSet<TEntity>(source);
        }

    }
}
