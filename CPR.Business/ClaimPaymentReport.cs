using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CPR.Core;
using CPR.Core.Interfaces;

namespace CPR.Business
{
    public class ClaimPaymentReport : IClaimPaymentReport
    {
        private readonly ITextFileService _textFileService;
        private readonly IClaimsDataProcessingService _dataProcessingService;

        public ClaimPaymentReport(ITextFileService textFileService,
            IClaimsDataProcessingService claimsDataProcessingService)
        {
            _textFileService = textFileService;
            _dataProcessingService = claimsDataProcessingService;
        }

        #region Implement IClaimPaymentReport

        public async Task Generator(string sourceFileName, string targetFileName)
        {
            var lines = ConvertFileAsLines(sourceFileName);

            try
            {
                var productYearInfo = _dataProcessingService.GetProductYearInfo(lines);

                if (productYearInfo == null)
                {
                    await _textFileService.WriteLineToTxtFile(targetFileName, GlobalConstants.TEXT_HEADER_FORMAT_ERROR);
                    return;
                }

                //Write the origin year and development year before write all products with incremental values
                await _textFileService.WriteLineToTxtFile(targetFileName, $"{productYearInfo.Origin},{productYearInfo.Span}")
                    .ContinueWith(async (_) =>
                    {
                        using (_textFileService.OpenSteamWriter(targetFileName))
                        {
                            foreach (var product in _dataProcessingService.CalculateProductsValues(lines.Skip(1), productYearInfo))
                            {
                                await _textFileService.WriteProductToTxtFile(product);
                            }
                        }
                    });
            }
            catch (Exception e)
            {
                await _textFileService.WriteLineToTxtFile(targetFileName, string.Format($"Error in Process.. details: {e.Message}"));
            }
        }

        #endregion

        #region Private Methods

        private string[] ConvertFileAsLines(string sourceFileName)
        {
            string file = Path.GetFullPath(Environment.CurrentDirectory + sourceFileName);

            if (file == null)
                throw new FileNotFoundException();

            //read the file
            return _textFileService.ReadTxtFile(file);
        }

        #endregion
    }
}