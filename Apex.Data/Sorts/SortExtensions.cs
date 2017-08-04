using System;
using System.Linq;
using System.Reflection;
using Apex.Data.Entities;

namespace Apex.Data.Sorts
{
    public static class SortExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
            where T : BaseEntity
        {
            return OrderByPropertyName(source, propertyName, SortDirection.Ascending);
        }

        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
            where T : BaseEntity
        {
            return OrderByPropertyName(source, propertyName, SortDirection.Descending);
        }

        private static IQueryable<T> OrderByPropertyName<T>(
            this IQueryable<T> source,
            string propertyName,
            SortDirection sortDirection) where T : BaseEntity
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

        private static PropertyInfo GetProperty<T>(string propertyName) where T : BaseEntity
        {
            return typeof(T).GetProperties()
                .FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
