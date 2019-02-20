using System;
using System.Collections.Generic;
using System.Linq;
using CPR.Business.Services;
using CPR.Core.DTOs;
using CPR.Core.Enums;
using CPR.Core.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace CPR.Bussiness.UnitTests
{
    [TestClass]
    public class ClaimsDataProcessingServiceTests
    {
        private readonly IDictionary<FileTitles, int> _indexes = new Dictionary<FileTitles, int>{
            {FileTitles.Product, 0},
            {FileTitles.Origin, 1},
            {FileTitles.Development, 2},
            {FileTitles.Value, 3}
        };

        private ClaimsDataProcessService _sut;
        private IIndexServices _indexService;
        private IClaimsValueProcessService _claimsValueProcessService;

        [TestInitialize]
        public void TestInitialize()
        {
            _indexService = MockRepository.GenerateStub<IIndexServices>();
            _claimsValueProcessService = MockRepository.GenerateMock<IClaimsValueProcessService>();

            _sut = new ClaimsDataProcessService(_indexService, _claimsValueProcessService);
        }

        #region CalculateProductValuesTests

        [TestMethod]
        public void CalculateProductValues_InputOneProduct_ShouldReturnCorrectProductName()
        {
            //Arrange
            const string PRODUCT_NAME = "Comp";
            _indexService.Stub(i => i.Indexes).Return(_indexes);
            List<string> lines = new List<string>() { $"{PRODUCT_NAME}, 1992,1992,110" };

            //Act
            IEnumerable<Product> result = _sut.CalculateProductsValues(lines, new ProductYearInfo { Origin = 1992, Span = 1 });

            //Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(PRODUCT_NAME, result.First().Name);
        }

        [TestMethod]
        public void CalculateProductValues_InputOneProductWith4YearsSpanData_ShouldCallclaimsValueProcessServiceToIncrementValues()
        {
            //Arrange
            const string PRODUCT_NAME = "Comp";
            _indexService.Stub(i => i.Indexes).Return(_indexes);
            List<string> lines = new List<string>()
                {
                    $"{PRODUCT_NAME}, 1992,1992,10",
                    $"{PRODUCT_NAME}, 1992,1993,20",
                    $"{PRODUCT_NAME}, 1992,1994,30",
                    $"{PRODUCT_NAME}, 1992,1995,40",
                    $"{PRODUCT_NAME}, 1993,1993,5",
                    $"{PRODUCT_NAME}, 1993,1994,10",
                    $"{PRODUCT_NAME}, 1993,1995,20",
                    $"{PRODUCT_NAME}, 1994,1994,2",
                    $"{PRODUCT_NAME}, 1994,1995,4",
                    $"{PRODUCT_NAME}, 1995,1995,100",

                };

            List<decimal> productValues = new List<decimal> { 10, 20, 30, 40, 5, 10, 20, 2, 4, 100 };

            //Act
            foreach (var product in _sut.CalculateProductsValues(lines, new ProductYearInfo { Origin = 1992, Span = 4 }))
            {
                //Assert
                Assert.IsNotNull(product);
                _claimsValueProcessService
                    .AssertWasCalled(s => s.IncrementValuesByProduct(
                        Arg<Product>.Matches(p =>
                            p.Name.Equals(PRODUCT_NAME) && p.Values.Values.ToList().SequenceEqual(productValues)),
                        Arg<ProductYearInfo>.Matches(i => i.Origin == 1992 && i.Span == 4),
                        Arg<int>.Matches(c => c == 10)), option => option.Repeat.Once());
            }
        }

        [TestMethod]
        public void CalculateProductValues_InputOneProductWithRedulantData_ShouldReturnCorrectValue()
        {
            //Arrange
            const string PRODUCT_NAME = "Comp";
            _indexService.Stub(i => i.Indexes).Return(_indexes);
            List<string> lines = new List<string>()
            {
                $"{PRODUCT_NAME}, 1992,1992,10,10",
                $"{PRODUCT_NAME}, 1992,1993,20",
                $"{PRODUCT_NAME}, 1992,1994,30",
                $"{PRODUCT_NAME}, 1992,1995,40,redulantdata",
                $"{PRODUCT_NAME}, 1993,1993,5,1,1,1,1",
                $"{PRODUCT_NAME}, 1993,1994,10",
                $"{PRODUCT_NAME}, 1993,1995,20",
                $"{PRODUCT_NAME}, 1994,1994,2",
                $"{PRODUCT_NAME}, 1994,1995,4",
                $"{PRODUCT_NAME}, 1995,1995,100",

            };

            //Act
            foreach (var product in _sut.CalculateProductsValues(lines, new ProductYearInfo { Origin = 1992, Span = 4 }))
            {
                //Assert
                Assert.IsNotNull(product);
            }
        }

        [TestMethod]
        public void CalculateProductValues_InputOneProductWithdefaultMissingLine_ShouldReturnCorrectValue()
        {
            //Arrange
            const string PRODUCT_NAME = "Comp";
            _indexService.Stub(i => i.Indexes).Return(_indexes);
            List<string> lines = new List<string>()
            {
                $"{PRODUCT_NAME}, 1992,1993,20",
                $"{PRODUCT_NAME}, 1992,1994,30",
                $"{PRODUCT_NAME}, 1992,1995,40",
                $"{PRODUCT_NAME}, 1993,1993,5",
                $"{PRODUCT_NAME}, 1993,1994,10",
                $"{PRODUCT_NAME}, 1993,1995,20",
                $"{PRODUCT_NAME}, 1994,1994,2",
                $"{PRODUCT_NAME}, 1994,1995,4",
                $"{PRODUCT_NAME}, 1995,1995,100",

            };

            //Act
            foreach (var product in _sut.CalculateProductsValues(lines, new ProductYearInfo { Origin = 1992, Span = 4 }))
            {
                //Assert
                Assert.IsNotNull(product);
            }
        }

        [TestMethod]
        public void CalculateProductValues_InputOneProductWithWrongYearFormat_ShouldNotReturnTheProduct()
        {
            //Arrange
            const string PRODUCT_NAME = "Comp";
            _indexService.Stub(i => i.Indexes).Return(_indexes);
            List<string> lines = new List<string>()
            {
                $"{PRODUCT_NAME}, error,1992,10",
                $"{PRODUCT_NAME}, 1992,1993,20",
                $"{PRODUCT_NAME}, 1992,1994,30",
                $"{PRODUCT_NAME}, 1992,1995,40",
                $"{PRODUCT_NAME}, 1993,1993,5",
                $"{PRODUCT_NAME}, 1993,1994,10",
                $"{PRODUCT_NAME}, 1993,1995,20",
                $"{PRODUCT_NAME}, 1994,1994,2",
                $"{PRODUCT_NAME}, error,1995,4",
                $"{PRODUCT_NAME}, 1995,1995,100",
            };

            //Act
            List<Product> result = _sut.CalculateProductsValues(lines, new ProductYearInfo { Origin = 1992, Span = 4 }).ToList();

            //Assert
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void CalculateProductValues_InputOneProductWithWrongValueFormat_ShouldNotReturnTheProduct()
        {
            //Arrange
            const string PRODUCT_NAME = "Comp";
            _indexService.Stub(i => i.Indexes).Return(_indexes);
            List<string> lines = new List<string>()
            {
                $"{PRODUCT_NAME}, 1992,1992,error",
                $"{PRODUCT_NAME}, 1992,1993,20",
                $"{PRODUCT_NAME}, 1993,1993,5",
            };

            //Act
            List<Product> result = _sut.CalculateProductsValues(lines, new ProductYearInfo { Origin = 1992, Span = 2 }).ToList();

            //Assert
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void CalculateProductValues_TextFileWithDifferentTitleSequence_ShouldStillReturnTheProduct()
        {
            //Arrange
            IDictionary<FileTitles, int> indexes = new Dictionary<FileTitles, int>{
                {FileTitles.Product, 3},
                {FileTitles.Origin, 2},
                {FileTitles.Development, 1},
                {FileTitles.Value, 0}
            };
            const string PRODUCT_NAME = "Comp";
            _indexService.Stub(i => i.Indexes).Return(indexes);
            List<string> lines = new List<string>()
            {
                $"10,1992,1992,{PRODUCT_NAME}",
                $"20, 1993,1992,{PRODUCT_NAME}",
                $"5, 1993,1993,{PRODUCT_NAME}",
            };

            //Act
            List<Product> result = _sut.CalculateProductsValues(lines, new ProductYearInfo { Origin = 1992, Span = 2 }).ToList();

            //Assert
            Assert.IsTrue(result.Any());
        }

        [TestMethod]
        public void CalculateProductValues_Input2ProductsWithErrorAndNot_ShouldReturnTheCorrectProduct()
        {
            //Arrange
            const string PRODUCT_NAME1 = "ProductWithError";
            const string PRODUCT_NAME2 = "ProductWithoutError";
            _indexService.Stub(i => i.Indexes).Return(_indexes);
            List<string> lines = new List<string>()
            {
                $"{PRODUCT_NAME1}, 1992,1992,error",
                $"{PRODUCT_NAME1}, 1992,1993,20",
                $"{PRODUCT_NAME1}, 1993,1993,5",
                $"{PRODUCT_NAME2}, 1992,1992,10",
                $"{PRODUCT_NAME2}, 1992,1993,20",
                $"{PRODUCT_NAME2}, 1993,1993,5",
            };

            //Act
            List<Product> result = _sut.CalculateProductsValues(lines, new ProductYearInfo { Origin = 1992, Span = 2 }).ToList();

            //Assert
            Assert.IsTrue(result.Any());
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(PRODUCT_NAME2, result.First().Name);
        }

        [TestMethod]
        public void CalculateProductValues_Input2Products_ShouldReturnProducts()
        {
            //Arrange
            const string PRODUCT_NAME1 = "Product1";
            const string PRODUCT_NAME2 = "Product2";
            _indexService.Stub(i => i.Indexes).Return(_indexes);
            List<string> lines = new List<string>()
            {
                $"{PRODUCT_NAME1}, 1992,1992,10",
                $"{PRODUCT_NAME1}, 1992,1993,20",
                $"{PRODUCT_NAME1}, 1993,1993,5",
                $"{PRODUCT_NAME2}, 1992,1992,1",
                $"{PRODUCT_NAME2}, 1992,1993,2",
                $"{PRODUCT_NAME2}, 1993,1993,50",
            };
            List<decimal> expectProduct1Values = new List<decimal> { 10, 20, 5 };
            List<decimal> expectProduct2Values = new List<decimal> { 1, 2, 50 };

            //Act
            var result = _sut.CalculateProductsValues(lines, new ProductYearInfo { Origin = 1992, Span = 2 }).ToList();

            //Assert
            Assert.AreEqual(2, result.Count);
            _claimsValueProcessService
                .AssertWasCalled(s => s.IncrementValuesByProduct(
                    Arg<Product>.Matches(p =>
                        p.Name.Equals(PRODUCT_NAME1) && p.Values.Values.ToList().SequenceEqual(expectProduct1Values)),
                    Arg<ProductYearInfo>.Matches(i => i.Origin == 1992 && i.Span == 2),
                    Arg<int>.Matches(c => c == 3)), option => option.Repeat.Once());
            _claimsValueProcessService
                .AssertWasCalled(s => s.IncrementValuesByProduct(
                    Arg<Product>.Matches(p =>
                        p.Name.Equals(PRODUCT_NAME2) && p.Values.Values.ToList().SequenceEqual(expectProduct2Values)),
                    Arg<ProductYearInfo>.Matches(i => i.Origin == 1992 && i.Span == 2),
                    Arg<int>.Matches(c => c == 3)), option => option.Repeat.Once());
        }

        [TestMethod]
        public void CalculateProductValues_Input2ProductsWithCaseSenstive_ShouldReturnAs2Products()
        {
            //Arrange
            const string PRODUCT_NAME1 = "Product";
            const string PRODUCT_NAME2 = "product";
            _indexService.Stub(i => i.Indexes).Return(_indexes);
            List<string> lines = new List<string>()
            {
                $"{PRODUCT_NAME1}, 1992,1992,10",
                $"{PRODUCT_NAME2}, 1992,1992,1",
            };

            //Act
            List<Product> result = _sut.CalculateProductsValues(lines, new ProductYearInfo { Origin = 1992, Span = 2 }).ToList();

            //Assert
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void CalculateProductValues_ProductsWithCaseSenstive_ShouldReturnAs2Products()
        {
            //Arrange
            const string PRODUCT_NAME1 = "Product1";
            const string PRODUCT_NAME2 = "product2";
            _indexService.Stub(i => i.Indexes).Return(_indexes);
            List<string> lines = new List<string>()
            {
                $"{PRODUCT_NAME1}, 1992,1992,10",
                $"{PRODUCT_NAME2}, 1992,1992,1",
            };

            //Act
            List<Product> result = _sut.CalculateProductsValues(lines, new ProductYearInfo { Origin = 1992, Span = 2 }).ToList();

            //Assert
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void CalculateProductValues_Input2ProductsWithMixSequences_ShouldReturnProducts()
        {
            //Arrange
            const string PRODUCT_NAME1 = "Product1";
            const string PRODUCT_NAME2 = "Product2";
            _indexService.Stub(i => i.Indexes).Return(_indexes);
            List<string> lines = new List<string>()
            {
                $"{PRODUCT_NAME2}, 1992,1992,1",
                $"{PRODUCT_NAME1}, 1992,1992,10",
                $"{PRODUCT_NAME1}, 1992,1993,20",
                $"{PRODUCT_NAME2}, 1993,1993,50",
                $"{PRODUCT_NAME1}, 1993,1993,5",
                $"{PRODUCT_NAME2}, 1992,1993,2",

            };
            List<decimal> expectProduct1Values = new List<decimal> { 10, 20, 5 };
            List<decimal> expectProduct2Values = new List<decimal> { 1, 2, 50 };

            //Act
            List<Product> result = _sut.CalculateProductsValues(lines, new ProductYearInfo { Origin = 1992, Span = 2 }).ToList();

            //Assert
            Assert.AreEqual(2, result.Count);
            _claimsValueProcessService
                .AssertWasCalled(s => s.IncrementValuesByProduct(
                    Arg<Product>.Matches(p =>
                        p.Name.Equals(PRODUCT_NAME1) && p.Values.Values.ToList().SequenceEqual(expectProduct1Values)),
                    Arg<ProductYearInfo>.Matches(i => i.Origin == 1992 && i.Span == 2),
                    Arg<int>.Matches(c => c == 3)), option => option.Repeat.Once());
            _claimsValueProcessService
                .AssertWasCalled(s => s.IncrementValuesByProduct(
                    Arg<Product>.Matches(p =>
                        p.Name.Equals(PRODUCT_NAME2) && p.Values.Values.ToList().SequenceEqual(expectProduct2Values)),
                    Arg<ProductYearInfo>.Matches(i => i.Origin == 1992 && i.Span == 2),
                    Arg<int>.Matches(c => c == 3)), option => option.Repeat.Once());
        }

        #endregion

        #region CalculateProductYearInfoTests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetProductYearInfo_IfLineIsNull_ShouldThrow()
        {
            //Arrange - in Setup

            //Act
            _sut.GetProductYearInfo(null);

            //Assert - exception
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetProductYearInfo_IfLineIsEmpty_ShouldThrow()
        {
            //Arrange- in Setup

            //Act
            _sut.GetProductYearInfo(new string[0]);

            //Assert - exception
        }

        [TestMethod]
        public void GetProductYearInfo_IfLine1YearSpan_ShouldReturnCorrectResult()
        {
            //Arrange
            const string PRODUCT_NAME = "Comp";
            const int ORIGIN_YEAR = 1992;
            _indexService.Stub(i => i.Indexes).Return(_indexes);

            string[] lines =
            {
                "Product, Origin year, Development year,Incremental Value",
                $"{PRODUCT_NAME}, {ORIGIN_YEAR},1992,10",
            };

            int expectDevelopmentYear = 1;

            //Act
            ProductYearInfo result = _sut.GetProductYearInfo(lines);

            //Assert
            Assert.AreEqual(ORIGIN_YEAR, result.Origin);
            Assert.AreEqual(expectDevelopmentYear, result.Span);
        }

        [TestMethod]
        public void GetProductYearInfo_IfLineContainsValidData_ShouldReturnCorrectResult()
        {
            //Arrange
            const string PRODUCT_NAME = "Comp";
            const int ORIGIN_YEAR = 1990;
            _indexService.Stub(i => i.Indexes).Return(_indexes);

            string[] lines =
            {
                "Product, Origin year, Development year,Incremental Value",
                $"{PRODUCT_NAME}, 1992,1992,10",
                $"{PRODUCT_NAME}, 1992,1993,20",
                $"{PRODUCT_NAME}, 1992,1994,30",
                $"{PRODUCT_NAME}, 1992,1995,40",
                $"{PRODUCT_NAME}, 1993,1993,5",
                $"{PRODUCT_NAME}, 1993,1994,10",
                $"{PRODUCT_NAME}, 1993,1995,20",
                $"{PRODUCT_NAME}, 1994,1994,2",
                $"{PRODUCT_NAME}, 1994,1995,4",
                $"{PRODUCT_NAME}, {ORIGIN_YEAR},1995,100",
            };

            int expectDevelopmentYear = 6;

            //Act
            ProductYearInfo result = _sut.GetProductYearInfo(lines);

            //Assert
            Assert.AreEqual(ORIGIN_YEAR, result.Origin);
            Assert.AreEqual(expectDevelopmentYear, result.Span);
        }

        [TestMethod]
        public void GetProductYearInfo_IfErrorHeader_ShouldReturnNull()
        {
            //Arrange
            const string PRODUCT_NAME = "Comp";
            _indexService.Stub(i => i.Indexes).Return(_indexes);

            string[] lines =
            {
                "Product, Oorigin year, Development year,Incremental Value",
                $"{PRODUCT_NAME}, 1993 , 1992,10",
            };

            //Act
            ProductYearInfo result = _sut.GetProductYearInfo(lines);

            //Assert
            Assert.IsNull(result);
        }

        #endregion
    }
}
