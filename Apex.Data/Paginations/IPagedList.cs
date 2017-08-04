using System.Collections.Generic;

namespace Apex.Data.Paginations
{
    public interface IPagedList<out T> : IEnumerable<T>
    {
        int TotalRecords { get; }

        int TotalRecordsFiltered { get; }
    }
}
