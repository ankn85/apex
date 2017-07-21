using System;
using System.Collections.Generic;
using System.Linq;

namespace Apex.Data.Paginations
{
    public sealed class PagedList<T> : List<T>, IPagedList<T>
    {
        public PagedList(int page, int size)
            : this(Enumerable.Empty<T>(), 0, 0, page, size)
        {
        }

        public PagedList(int totalRecords, int page, int size)
            : this(Enumerable.Empty<T>(), totalRecords, 0, page, size)
        {
        }

        public PagedList(IEnumerable<T> source, int totalRecords, int totalRecordsFiltered, int page, int size)
        {
            Page = page;
            Size = size;
            TotalRecords = totalRecords;
            TotalRecordsFiltered = totalRecordsFiltered;

            TotalPages = (int)Math.Ceiling(TotalRecords / (double)Size);

            if (TotalRecordsFiltered > 0)
            {
                AddRange(source);
            }
        }

        public int Page { get; }

		public int Size { get; }

		public int TotalRecords { get; }

        public int TotalRecordsFiltered { get; }

        public int TotalPages { get; }
    }
}
