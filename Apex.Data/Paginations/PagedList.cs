using System.Collections.Generic;
using System.Linq;

namespace Apex.Data.Paginations
{
    public sealed class PagedList<T> : List<T>, IPagedList<T>
    {
        private PagedList(IEnumerable<T> source, int totalRecords, int totalRecordsFiltered)
        {
            //Page = page;
            //Size = size;
            TotalRecords = totalRecords;
            TotalRecordsFiltered = totalRecordsFiltered;

            //TotalPages = (int)Math.Ceiling(TotalRecords / (double)Size);

            if (TotalRecordsFiltered > 0)
            {
                AddRange(source);
            }
        }

        //public int Page { get; }

		//public int Size { get; }

		public int TotalRecords { get; }

        public int TotalRecordsFiltered { get; }

        //public int TotalPages { get; }

        public static IPagedList<T> Empty()
        {
            return new PagedList<T>(Enumerable.Empty<T>(), 0, 0);
        }

        public static IPagedList<T> Empty(int totalRecords)
        {
            return new PagedList<T>(Enumerable.Empty<T>(), totalRecords, 0);
        }

        public static IPagedList<T> Create(IEnumerable<T> source, int totalRecords, int totalRecordsFiltered)
        {
            return new PagedList<T>(source, totalRecords, totalRecordsFiltered);
        }
    }
}
