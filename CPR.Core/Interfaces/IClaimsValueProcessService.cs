using CPR.Core.DTOs;

namespace CPR.Core.Interfaces
{
    public interface IClaimsValueProcessService
    {
        void IncrementValuesByProduct(Product product, ProductYearInfo productYearInfo, int totalCount);
    }
}