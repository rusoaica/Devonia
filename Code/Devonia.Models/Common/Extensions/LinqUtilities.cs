/// Written by: Yulia Danilova
/// Creation Date: 14th of October, 2021
/// Purpose: LINQ extension methods
#region ========================================================================= USING =====================================================================================
using System;
using System.Collections.Generic;
#endregion

namespace Devonia.Models.Common.Extensions
{
    public static class LinqUtilities
    {
        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Performs <paramref name="action"/> on each item of <paramref name="source"/>
        /// </summary>
        /// <param name="source">The items source upon the action is performed</param>
        /// <param name="action">A delegate to a function that is executed on each item of the source</param>
        /// <typeparam name="TSource">The type of the items inside <paramref name="source"/></typeparam>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains elements from the input sequence.</returns>
        /// <exception cref="NullReferenceException">Exception thrown when either <paramref name="source"/> or <paramref name="action"/> is null</exception>
        public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            try
            {
                if (source == null)
                    throw new NullReferenceException("source");
                if (action == null)
                    throw new NullReferenceException("action");
                foreach (TSource element in source)
                    action(element);
                return source;
            }
            catch (Exception e)
            {
                // TODO: remove in production!
                throw;
            }
        }
        #endregion
    }
}