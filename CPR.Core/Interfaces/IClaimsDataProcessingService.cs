using System.Collections.Generic;
using CPR.Core.DTOs;

namespace CPR.Core.Interfaces
{
    public interface IClaimsDataProcessingService
    {
        ProductYearInfo GetProductYearInfo(string[] lines);

        IEnumerable<Product> CalculateProductsValues(IEnumerable<string> lines,
            ProductYearInfo productYearInfo);
    }
}