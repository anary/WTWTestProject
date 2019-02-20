using System;
using CPR.Business.Services;
using CPR.Core.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CPR.Bussiness.UnitTests
{
    [TestClass]
    public class IndexServicesTests
    {
        [TestMethod]
        public void GetIndex_IfValidHeader_ShouldReturnCorrect()
        {
            //Arrange
            var sut = new IndexServices();
            var line = "Product, Origin year, Development year,Incremental Value";

            sut.Titles = line.Split(',');

            //Act
            var result = sut.Indexes;

            //Assert
            Assert.AreEqual(0, result[FileTitles.Product]);
            Assert.AreEqual(1, result[FileTitles.Origin]);
            Assert.AreEqual(2, result[FileTitles.Development]);
            Assert.AreEqual(3, result[FileTitles.Value]);
        }

        [TestMethod]
        public void GetIndex_IfValidHeaderInDifferentOrder_ShouldReturnCorrect()
        {
            //Arrange
            var sut = new IndexServices();
            var line = "Development year,Incremental Value, Product, Origin year";

            sut.Titles = line.Split(',');

            //Act
            var result = sut.Indexes;

            //Assert
            Assert.AreEqual(2, result[FileTitles.Product]);
            Assert.AreEqual(3, result[FileTitles.Origin]);
            Assert.AreEqual(0, result[FileTitles.Development]);
            Assert.AreEqual(1, result[FileTitles.Value]);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void GetIndex_IfValidHeaderWithExtra_ShouldThrow()
        {
            //Arrange
            var sut = new IndexServices();
            var line = "Development year,Incremental Value, Product, Origin year, Extra bit";

            sut.Titles = line.Split(',');

            //Act
            var result = sut.Indexes;

            //Assert
            Assert.AreEqual(2, result[FileTitles.Product]);
            Assert.AreEqual(3, result[FileTitles.Origin]);
            Assert.AreEqual(0, result[FileTitles.Development]);
            Assert.AreEqual(1, result[FileTitles.Value]);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void GetIndex_IfHeaderMissing_ShouldThrow()
        {
            //Arrange
            var sut = new IndexServices();
            var line = "Development year,Incremental Value, Product";

            sut.Titles = line.Split(',');

            //Act
            var result = sut.Indexes;

            //Assert- Execption
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void GetIndex_IfCanNotFindField_ShouldThrow()
        {
            //Arrange
            var sut = new IndexServices();
            var line = "Development year,Incremental Value, ErrorField, Origin year";

            sut.Titles = line.Split(',');

            //Act
            var result = sut.Indexes;

            //Assert- Execption
        }
    }
}
