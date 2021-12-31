﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReadingListMaker;

namespace ReadingListMakerTests
{
    [TestClass]
    public class BookTests
    {
        [TestMethod]
        public void Book_ShouldCreateNewBook()
        {
            // Arrange
            var searchQuery = "dune";

            // Act
            var result = MainProgram.BookSearchHelper(searchQuery);

            // Assert
            Assert.IsInstanceOfType(result, typeof(Book[]),
                "Did not successfully create a Book");
        }
    }
}
