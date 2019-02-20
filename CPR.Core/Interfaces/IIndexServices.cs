using System.Collections.Generic;
using CPR.Core.Enums;

namespace CPR.Core.Interfaces
{
    /// <summary>
    /// Index service to get all indexes from Titles.
    /// </summary>
    public interface IIndexServices
    {
        string[] Titles { set; }
        IDictionary<FileTitles, int> Indexes { get; }
    }
}