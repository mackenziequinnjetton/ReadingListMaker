using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReadingListMaker;

namespace ReadingListMakerTests
{
    [TestClass]
    public class BookTests
    {
        [TestMethod]
        public void Book_ShouldCreateNewBook()
        {
            var searchQuery = "dune";

            var result = MainProgram.BookSearchHelper(searchQuery);

            Assert.IsInstanceOfType(result, typeof(Book[]),
                "Did not successfully create a Book");
        }
    }
}
