using System;
using System.Linq;
using System.Reflection;

namespace Apex.Data.Sorts
{
    public static class SortExtensions
    {
        public static IQueryable<T> OrderBy<T>(
            this IQueryable<T> source,
            string propertyName,
            SortDirection sortDirection) where T : class
        {
            if (!source.Any() || string.IsNullOrEmpty(propertyName))
            {
                return source;
            }

            PropertyInfo property = GetProperty<T>(propertyName);

            if (property == null)
            {
                return source;
            }

            var sortedSource = sortDirection == SortDirection.Ascending ?
                source.OrderBy(property.GetValue) :
                source.OrderByDescending(property.GetValue);

            return sortedSource.AsQueryable();
        }

        private static PropertyInfo GetProperty<T>(string propertyName) where T : class
        {
            return typeof(T).GetProperties()
                .FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
