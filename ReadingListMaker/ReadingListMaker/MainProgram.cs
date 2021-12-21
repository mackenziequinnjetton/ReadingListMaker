using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text.RegularExpressions;

namespace ReadingListMaker
{
    internal class MainProgram
    {
        // Indicates whether it is the first time MainMenu() has been called
        private static bool MainMenuFirstCall { get; set; } = true;
        private static bool APIResponsePending { get; set; }
        private static Book SelectedBook { get; set; }

        // Holds a temporary copy of the reading list. I judged serializing it
        // to a file for permanent storage to be beyond the permitted
        // scope of this project
        private static List<Book> ReadingList { get; set; } = 
            new List<Book>();

        static void Main(string[] args)
        {
            IntroText();
            MainMenu();
        }

        // Prints an introductory message informing the user of the
        // program's function and how to enter input
        static void IntroText()
        {
            Console.WriteLine();
            Console.WriteLine($"   Welcome! This program searches for " +
                "books and creates a reading list.");
            Console.WriteLine();
            Console.WriteLine("   Please select one of the " +
                "following options:");
        }

        // Prints a main menu, asks the user to select a choice,
        // and checks if their choice is one of the menu options
        static void MainMenu()
        {
            // If it is not the first call of MainMenu(), clear the console
            if (!MainMenuFirstCall)
            {
                Console.Clear();
            }

            Console.WriteLine();
            Console.WriteLine("   1: Look up a book by title");
            Console.WriteLine("   2: View your current reading list");
            Console.WriteLine("   3: Quit");
            Console.WriteLine();

            string response;

            // Loops until the user inputs a valid menu selection
            while (true)
            {
                // Maintains margin with user prompt
                Console.Write("   ");
                response = Console.ReadLine();

                // Checks if the user selected a valid menu item
                if (new List<string> { "1", "2", "3" }
                    .Contains(response.Trim()))
                {
                    break;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("   We didn't understand your entry. " +
                        "Please enter the number of the option " +
                        "you wish to select:");
                    Console.WriteLine();
                }
            }

            // For future calls of MainMenu,
            // the console will be cleared first
            MainMenuFirstCall = false;

            // Calls the appropriate method depending on
            // the user's selection
            switch (response)
            {
                case "1":
                    BookSearchMenu();
                    break;
                case "2":
                    ViewReadingList();
                    break;
                case "3":
                    Environment.Exit(0);
                    break;
            }

            // This is used during a later asynchronous API query
            // in BookSearch() where control bounces back to MainMenu()
            // until the API query returns. It displays a message while
            // the query is still ongoing, and the while loop prevents the
            // user from entering input and causing unexpected results
            // in the program
            while (APIResponsePending)
            {
                Console.Clear();
                Console.WriteLine();
                Console.Write("   Searching... ");
                Console.ReadLine();
            }
        }

        // Displays a menu prompting the user to enter a book title to
        // search for, and then calls BookSearch() to execute the search
        static void BookSearchMenu()
        {
            Console.Clear();

            Console.WriteLine();
            Console.WriteLine("   Please enter a book title:");
            Console.WriteLine();

            string searchQuery;
            
            while (true)
            {
                Console.Write("   ");
                searchQuery = Console.ReadLine().Trim().ToLower();

                // Checks to make sure the user did not enter an empty string
                // or whitespace characters
                if (searchQuery == "" 
                    || new Regex(@"^\s+$").Match(searchQuery).Success)
                {
                    Console.WriteLine();
                    Console.WriteLine("   That is not a valid book title. " +
                        "Please enter a book title:");
                    Console.WriteLine();
                    continue;
                }
                else
                {
                    break;
                }
            }

            Console.Clear();
            BookSearch(searchQuery);
        }

        // Carries out the search for the user's book by title, calling
        // BookSearchHelper() to asynchronously query the Google Books API,
        // then once the query returns, displays the search results and 
        // prompts the user to select a book to add to their reading list
        static async void BookSearch(string searchQuery)
        {
            // Calls BookSearchHelper to carry out the API query in a
            // separate thread
            Task<IEnumerable<Book>> bookTitleQuery = Task.Run(
                () => BookSearchHelper(searchQuery));

            // Used for the while loop at the end of MainMenu() which
            // displays a message while the API query is ongoing
            APIResponsePending = true;

            Console.WriteLine();
            Console.Write("   ");

            // Control bounces back to MainMenu(), after the switch
            // statement on line 88
            await bookTitleQuery;
            APIResponsePending = false;

            var searchResult = bookTitleQuery.Result.ToArray();

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("   Results:");
            Console.WriteLine();

            foreach (var item in searchResult)
            {
                Console.WriteLine($"   Title:      {item.Title}");
                if (item.Authors != null)
                {
                    foreach (var author in item.Authors)
                    {
                        Console.WriteLine($"   Author:     {author}");
                    }
                }

                // Accounts for edge case where there is no author listed
                else
                {
                    Console.WriteLine("   Author:     None listed");
                }
                if (item.Publisher != null)
                {
                    Console.WriteLine($"   Publisher:  {item.Publisher}");
                }

                // Accounts for edge case where there is no publisher listed
                else
                {
                    Console.WriteLine($"   Publisher:  None listed");
                }
                
                Console.WriteLine();
            }

            Console.WriteLine("   Please select a result to add " +
                "to reading list");
            Console.WriteLine("   (enter 1-5, or enter 6 to return " +
                "to the main menu): ");
            Console.WriteLine();

            while (true)
            {
                Console.Write("   ");
                var response = Console.ReadLine().Trim();

                if (new List<string> { "1", "2", "3", "4", "5", "6" }
                    .Contains(response))
                {
                    if (new List<string> { "1", "2", "3", "4", "5" }
                        .Contains(response))
                    {
                        // Keeps track of the current book selected 
                        // by the user, for use in AddToReadingList()
                        SelectedBook = searchResult[int.Parse(response)];
                        AddToReadingList(SelectedBook);
                        break;
                    }
                    else
                    {
                        MainMenu();
                        break;
                    }
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("   We didn't understand your entry. " +
                        "Please enter the number of the option " +
                        "you wish to select:");
                    Console.WriteLine();
                }
            }
        }

        // Asynchronously handles the Google Books API query
        static IEnumerable<Book> BookSearchHelper(string searchQuery)
        {
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

            // Releases system resources
            apiRequest.Abort();
            apiStream.Dispose();
            apiReader.Dispose();

            // Returns an IEnumerable<Book> containing search results
            return resultCollection;
        }

        static void AddToReadingList(Book SelectedBook)
        { 
            ReadingList.Add(SelectedBook);

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("   Would you like to search for another " +
                "book? (Enter 1 for yes, 2 for no):");
            Console.WriteLine();
            
            while (true)
            {
                Console.Write("   ");
                var response = Console.ReadLine().Trim();

                if (new List<string> { "1", "2" }.Contains(response))
                {
                    if (response == "1")
                    {
                        BookSearchMenu();
                    }
                    else
                    {
                        MainMenu();
                    }
                    break;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("   We didn't understand your entry. " +
                        "Please enter the number of the option " +
                        "you wish to select:");
                    Console.WriteLine();
                }
            }
        }

        static void ViewReadingList()
        {
            Console.Clear();

            if (ReadingList.Count == 0)
            {
                Console.WriteLine();
                Console.WriteLine("   Your reading list has no books in it.");
                Console.WriteLine();
            }
            else
            {
                // Prints reading list contents
                foreach (var item in ReadingList)
                {
                    Console.WriteLine();
                    Console.WriteLine($"   Title:      {item.Title}");
                    foreach (var author in item.Authors)
                    {
                        Console.WriteLine($"   Author:     {author}");
                    }
                    Console.WriteLine($"   Publisher:  {item.Publisher}");
                    Console.WriteLine();
                }
            }

            Console.WriteLine("   Press enter to return to the main menu:");
            Console.WriteLine();
            Console.Write("   ");
            Console.ReadLine();
            MainMenu();
        }
    }
}
