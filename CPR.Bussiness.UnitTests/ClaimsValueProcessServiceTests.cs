using System.Collections.Generic;
using System.Linq;
using CPR.Business.Services;
using CPR.Core.DTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CPR.Bussiness.UnitTests
{
    [TestClass]
    public class ClaimsValueProcessServiceTests
    {
        [TestMethod]
        public void IncrementValuesByProduct_IfProductSpan1Year_shouldReturnCorrectValue()
        {
            //Arrange
            var product = new Product { Name = "Any", Values = new Dictionary<int, decimal> { { 0, 10.0m } } };
            var sut = new ClaimsValueProcessService();
            var expect = new List<decimal>() { 10.0m };

            //Act
            sut.IncrementValuesByProduct(product, new ProductYearInfo { Origin = 1990, Span = 1 }, 1);

            //Assert
            Assert.IsTrue(product.Values.Values.SequenceEqual(expect));
        }

        [TestMethod]
        public void IncrementValuesByProduct_IfProductSpan2Year_shouldReturnCorrectValue()
        {
            //Arrange
            var product = new Product
            {
                Name = "Any",
                Values = new Dictionary<int, decimal> { { 0, 10.0m }, { 1, 20.0m }, { 2, 30.0m } }
            };
            var sut = new ClaimsValueProcessService();
            var expect = new List<decimal>() { 10.0m, 30m, 30m };

            //Act
            sut.IncrementValuesByProduct(product, new ProductYearInfo { Origin = 1990, Span = 2 }, 3);

            //Assert
            Assert.IsTrue(product.Values.Values.SequenceEqual(expect));
        }

        [TestMethod]
        public void IncrementValuesByProduct_IfProductSpan3Year_shouldReturnCorrectValue()
        {
            //Arrange
            var product = new Product
            {
                Name = "Any",
                Values = new Dictionary<int, decimal> { { 0, 10.0m }, { 1, 20.0m }, { 2, 30.0m }, { 3, 1.0m }, { 4, 2.0m }, { 5, 5.0m } }
            };
            var sut = new ClaimsValueProcessService();
            var expect = new List<decimal>() { 10.0m, 30m, 60m, 1m, 3m, 5m };

            //Act
            sut.IncrementValuesByProduct(product, new ProductYearInfo { Origin = 1990, Span = 3 }, 6);

            //Assert
            Assert.IsTrue(product.Values.Values.SequenceEqual(expect));
        }

        [TestMethod]
        public void IncrementValuesByProduct_IfProductSpan4Year_shouldReturnCorrectValue()
        {
            //Arrange
            var product = new Product
            {
                Name = "Any",
                Values = new Dictionary<int, decimal>
                {
                    { 0, 10.0m }, { 1, 20.0m }, { 2, 30.0m }, { 3, 1.0m }, { 4, 2.0m }, { 5, 5.0m },
                    { 6, 15.0m }, { 7, 20.0m }, { 8, 30.0m }, { 9, 100.0m }
                }
            };
            var sut = new ClaimsValueProcessService();
            var expect = new List<decimal>() { 10.0m, 30m, 60m, 61m, 2m, 7m, 22.0m, 20m, 50m, 100m };

            //Act
            sut.IncrementValuesByProduct(product, new ProductYearInfo { Origin = 1990, Span = 4 }, 10);

            //Assert
            Assert.IsTrue(product.Values.Values.SequenceEqual(expect));
        }

        [TestMethod]
        public void IncrementValuesByProduct_IfProductWithNoValue_shouldReturnEmptyValue()
        {
            //Arrange
            var product = new Product
            {
                Name = "Any",
                Values = new Dictionary<int, decimal>()
            };
            var sut = new ClaimsValueProcessService();

            //Act
            sut.IncrementValuesByProduct(product, new ProductYearInfo { Origin = 1990, Span = 0 }, 0);

            //Assert
            Assert.IsFalse(product.Values.Values.Any());
        }
    }
}
