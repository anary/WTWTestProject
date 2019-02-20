using CPR.Core.DTOs;
using CPR.Core.Interfaces;

namespace CPR.Business.Services
{
    public class ClaimsValueProcessService : IClaimsValueProcessService
    {
        #region Implement IClaimsValueProcessService

        public void IncrementValuesByProduct(Product product, ProductYearInfo productYearInfo, int totalCount)
        {
            var accumulateValue = 0m;
            int pointer = productYearInfo.Span;
            int breakCounter = 1;

            //If the value is on same year span, accmulate them, otherwise the pointer
            for (int i = 0; i < totalCount; i++)
            {
                if (i < pointer)
                {
                    accumulateValue = accumulateValue + product.Values[i];
                    product.Values[i] = +accumulateValue;
                }
                else
                {
                    pointer = pointer + productYearInfo.Span - breakCounter;
                    accumulateValue = product.Values[i];
                    breakCounter++;
                }

                product.Values[i] = accumulateValue;
            }
        }

        #endregion

    }
}