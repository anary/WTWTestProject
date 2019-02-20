using System;
using System.Collections.Generic;
using System.Linq;
using CPR.Business.Helpers;
using CPR.Core;
using CPR.Core.DTOs;
using CPR.Core.Enums;
using CPR.Core.Interfaces;

namespace CPR.Business.Services
{
    public class ClaimsDataProcessService : IClaimsDataProcessingService
    {
        private readonly IIndexServices _indexServices;
        private readonly IClaimsValueProcessService _claimsValueProcessService;

        public ClaimsDataProcessService(IIndexServices indexServices, IClaimsValueProcessService claimsValueProcessService)
        {
            _indexServices = indexServices;
            _claimsValueProcessService = claimsValueProcessService;
        }

        #region Implement IClaimsDataProcessingService

        public ProductYearInfo GetProductYearInfo(string[] lines)
        {
            const int TITLE_LENGTH = 4;
            string[] titles = lines.FirstOrDefault()?.GetColumns();

            if (titles == null || titles.Length < TITLE_LENGTH)
            {
                throw new ArgumentException();
            }

            _indexServices.Titles = titles;
            return CalculateProductYearInfo(lines);
        }

        public IEnumerable<Product> CalculateProductsValues(IEnumerable<string> lines, ProductYearInfo productYearInfo)
        {
            foreach (var product in ReadDataFromDataLines(lines, productYearInfo))
            {
                _claimsValueProcessService.IncrementValuesByProduct(product, productYearInfo,
                    GetTotalElementsCount(productYearInfo.Span));
                yield return product;
            }
        }

        #endregion

        #region Private Methods

        private ProductYearInfo CalculateProductYearInfo(IEnumerable<string> lines)
        {
            int originYear = 10000;
            int endYear = 0;

            //calculate the Year Span and Max, Min years.
            foreach (string line in lines.Skip(1))
            {
                string[] cols = line.GetColumns();
                int? lineOriginYear = cols[_indexServices.Indexes[FileTitles.Origin]].ParseToInt();
                int? lineEndYear = cols[_indexServices.Indexes[FileTitles.Development]].ParseToInt();

                // if the years format is not correct, return null.
                if (lineOriginYear == null || lineEndYear == null)
                {
                    throw new ArgumentException(string.Format(GlobalConstants.YEAR_FORMAT_WRONG_MESSAGE, line));
                }

                // get origin and end year.
                originYear = lineOriginYear < originYear ? lineOriginYear.Value : originYear;
                endYear = lineEndYear > endYear ? lineEndYear.Value : endYear;
            }

            int span = endYear - originYear + 1;

            return span == 0 ? null : new ProductYearInfo() { Origin = originYear, Span = span };
        }

        /// <summary>
        /// calculate the total elements counts by the incrementalYears
        /// It is actually a trangle shape data
        /// </summary>
        private static int GetTotalElementsCount(int incrementalYears)
        {
            return (incrementalYears * (incrementalYears + 1)) / 2;
        }

        private IEnumerable<Product> ReadDataFromDataLines(IEnumerable<string> lines, ProductYearInfo productYearInfo)
        {

            List<Product> products = new List<Product>();
            List<string> productsWithError = new List<string>();

            var valueDic = InitialProductValuesList(productYearInfo);

            foreach (string line in lines)
            {
                string[] cols = line.GetColumns();

                string productName = cols[_indexServices.Indexes[FileTitles.Product]].Trim();

                //If the product has error, we should not continue.
                if (productsWithError.Contains(productName))
                {
                    continue;
                }

                int? index = CalculateValueIndex(productYearInfo, cols);
                if (index == null)
                {
                    AddToEorrProductList(productsWithError, productName);
                    continue;
                }

                decimal? value = cols[_indexServices.Indexes[FileTitles.Value]].ParseToDecimal();
                if (value == null)
                {
                    AddToEorrProductList(productsWithError, productName);
                    continue;
                }

                if (products.All(p => p.Name != productName))
                {
                    //Initial products
                    Product product = new Product
                    {
                        Name = productName,
                        Values = new Dictionary<int, decimal>(valueDic) { [index.Value] = value.Value }
                    };

                    products.Add(product);
                }
                else
                {
                    Product product = products.First(p => p.Name == productName);
                    product.Values[index.Value] = value.Value;
                }
            }

            return products;
        }

        /// <summary>
        /// Initial the product value list
        /// set 0 as default value
        /// </summary>
        private IDictionary<int, decimal> InitialProductValuesList(ProductYearInfo productYearInfo)
        {
            const int DEFAULT_VALUE = 0;
            var valueDic = new Dictionary<int, decimal>();
            var totalElementsCount = GetTotalElementsCount(productYearInfo.Span);
            for (int i = 0; i < totalElementsCount; i++)
            {
                valueDic[i] = DEFAULT_VALUE;
            }

            return valueDic;
        }

        private int? CalculateValueIndex(ProductYearInfo productYearInfo, string[] cols)
        {
            //calculate the index of the value
            var startYear = cols[_indexServices.Indexes[FileTitles.Origin]].ParseToInt();
            var endYear = cols[_indexServices.Indexes[FileTitles.Development]].ParseToInt();

            if (startYear == null || endYear == null)
            {
                return null;
            }

            var yearSpan = startYear - productYearInfo.Origin;
            var triangleElementsCount = (yearSpan - 1) * yearSpan / 2;

            //Calculate the product value index, which is key in the value dictionary
            return endYear.Value - startYear.Value + ((yearSpan * productYearInfo.Span) - triangleElementsCount);
        }

        private static void AddToEorrProductList(IList<string> productsWithError, string productName)
        {
            if (!productsWithError.Any(p => p.Equals(productName)))
            {
                productsWithError.Add(productName);
            }
        }

        #endregion
    }
}