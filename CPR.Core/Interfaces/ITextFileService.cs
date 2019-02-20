using System;
using System.IO;
using System.Threading.Tasks;
using CPR.Core.DTOs;

namespace CPR.Core.Interfaces
{
    public interface ITextFileService : IDisposable
    {
        string[] ReadTxtFile(string fullFileName);
        Task WriteLineToTxtFile(string fullFileName, string txt);
        StreamWriter OpenSteamWriter(string fullFileName);
        Task WriteProductToTxtFile(Product product);
    }
}