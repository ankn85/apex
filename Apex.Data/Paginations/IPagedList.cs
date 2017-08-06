using System.Collections.Generic;
using Apex.Data.Entities;

namespace Apex.Data.Paginations
{
    public interface IPagedList<out T> : IEnumerable<T>
    {
        int TotalRecords { get; }

        int TotalRecordsFiltered { get; }
    }
}
