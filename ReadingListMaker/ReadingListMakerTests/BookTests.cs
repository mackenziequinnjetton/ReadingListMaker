using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Linq;
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

            // API key is stored in a local file outside the repository
            // for security reasons. If this program is run on another
            // computer, this path will need to be changed to point to
            // a file containing the new user's API key
            var apiKeyPath =
                @"C:\Users\macke\OneDrive\Documents\googleBooksAPIKey.txt";

            string apiKey;
            StreamReader apiReader;

            // Reads the API key from the file
            using (FileStream apiFileStream =
                File.Open(apiKeyPath, FileMode.Open))
            {
                apiReader = new StreamReader(apiFileStream);
                apiKey = apiReader.ReadToEnd();
            }

            // Splits the search query into individual words
            var searchQueryWords = searchQuery.Split(' ');

            // Partial Google Books API URL, to which the search query and
            // search options will be concatenated
            var apiURL = "https://www.googleapis.com/books/v1/volumes?q=";

            // Appends each word of the search query, as well as a "+",
            // to the API url
            foreach (var word in searchQueryWords)
            {
                apiURL += $"{word}+";
            }

            // Removes the final "+"
            apiURL = apiURL.Substring(0, apiURL.Length - 1);

            // Concatenates options that limit results to the first 5 and
            // include the API key
            apiURL += $"&maxResults=5&key={apiKey}";

            // Queries the API url and retrieves the response as a stream
            WebRequest apiRequest = WebRequest.Create(apiURL);
            Stream apiStream = apiRequest.GetResponse().GetResponseStream();

            // Reads the response from the API
            apiReader = new StreamReader(apiStream);
            var result = apiReader.ReadToEnd().Trim();

            // Parses the API's response into a JSON.NET object
            JObject resultObject = JObject.Parse(result);

            // Queries resultObject using LINQ to JSON
            // (enabled by JSON.NET) and returns a Book object
            // with the necessary information
            var resultCollection =
                from bookInfo in resultObject["items"]
                    .Children<JToken>()["volumeInfo"]
                select new Book
                (
                    bookInfo["title"],
                    bookInfo["authors"],
                    bookInfo["publisher"]
                );
        }
    }
}
