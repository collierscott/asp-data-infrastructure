using System.Collections.Generic;

namespace Infrastructure.Data.Utilities
{

#pragma warning disable 1570
    /// <summary>
    /// Converts an IEnumerable into a HashSet
    /// 
    /// Usage:  ToHashSet<ObjectType>(anIEnumerable)
    /// Returns: A HashSet of the object type specified
    /// </summary>
#pragma warning restore 1570
    public static class HashSetExtension
    {
        /// <summary>
        /// Returns a HashSet 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source">IEnumerable that is to be converted into a HashSet</param>
        /// <returns></returns>
        public static HashSet<TEntity> ToHashSet<TEntity>(this IEnumerable<TEntity> source)
        {
            return new HashSet<TEntity>(source);
        }

    }

}
