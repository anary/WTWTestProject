using System;
using System.IO;
using System.Threading.Tasks;
using CPR.Core.DTOs;
using CPR.Core.Interfaces;

namespace CPR.IO.Services
{
    public class TextFileService : ITextFileService
    {
        private StreamWriter _streamWriter;

        #region Implement ITextFileService Interface

        public string[] ReadTxtFile(string fullFileName)
        {
            return File.ReadAllLines(fullFileName);
        }

        public async Task WriteLineToTxtFile(string fullFileName, string txt)
        {
            using (var streamWriter = new StreamWriter(Environment.CurrentDirectory + fullFileName, false))
            {
                await streamWriter.WriteLineAsync(txt);
            }
        }

        public StreamWriter OpenSteamWriter(string fullFileName)
        {
            _streamWriter = new StreamWriter(Environment.CurrentDirectory + fullFileName, true);
            return _streamWriter;
        }

        public async Task WriteProductToTxtFile(Product product)
        {
            await _streamWriter.WriteLineAsync($"{product.Name}:{string.Join(",", product.Values.Values)}");
        }

        #endregion

        #region Implement IDisposable Interface

        public void Dispose()
        {
            _streamWriter.Close();
        }

        #endregion
    }
}
