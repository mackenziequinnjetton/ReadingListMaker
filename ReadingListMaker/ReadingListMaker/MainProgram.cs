using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace ReadingListMaker
{
    public class MainProgram
    {
        private static Book SelectedBook { get; set; }

        // Holds a temporary copy of the reading list. I judged serializing it
        // to a file for permanent storage to be beyond the permitted
        // scope of this project
        private static List<Book> ReadingList { get; set; } =
            new List<Book>();

        // Below properties are for testing purposes
        public static bool TestingMainMenuOption1 { get; set; }
        public static bool TestingMainMenuOption2 { get; set; }
        public static bool TestingMainMenuOption3 { get; set; }

        private static void Main(string[] args)
        {
            IntroText();
            MainMenu(Console.In);
        }

        // Prints an introductory message informing the user of the
        // program's function and how to enter input
        public static void IntroText()
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
        public static string MainMenu(TextReader reader)
        {
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
                response = reader.ReadLine();

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

            // Used for testing MainMenu() options
            string testingResult = string.Empty;
            
            // Calls the appropriate method depending on
            // the user's selection
            switch (response)
            {
                case "1":
                    testingResult = BookSearchMenu();
                    break;

                case "2":
                    testingResult = ViewReadingList();
                    break;

                case "3":

                    // Tests that MainMenu() option 3 worked
                    if (TestingMainMenuOption3)
                    {
                        testingResult = "Option 3 worked";
                        break;
                    }
                    else
                    {
                        Environment.Exit(0);
                        break;
                    }
            }

            return testingResult;
        }

        // Displays a menu prompting the user to enter a book title to
        // search for, and then calls BookSearch() to execute the search
        public static string BookSearchMenu()
        {
            // Tests that MainMenu() option 1 worked
            if (TestingMainMenuOption1)
            {
                return "Option 1 worked";
            }

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

            // Used for testing MainMenu() options
            return string.Empty;
        }

        // Carries out the search for the user's book by title, calling
        // BookSearchHelper() to query the Google Books API,
        // then once the query returns, displays the search results and
        // prompts the user to select a book to add to their reading list
        public static void BookSearch(string searchQuery)
        {
            Book[] bookTitleQuery;
            
            Console.Clear();
            Console.WriteLine();
            Console.Write("   Searching... ");

            bookTitleQuery = BookSearchHelper(searchQuery);

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("   Results");
            Console.WriteLine();

            foreach (var item in bookTitleQuery)
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
                        SelectedBook = bookTitleQuery[int.Parse(response) - 1];
                        AddToReadingList(SelectedBook);
                        break;
                    }
                    else
                    {
                        Console.Clear();
                        MainMenu(Console.In);
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

        // Handles the Google Books API query
        public static Book[] BookSearchHelper(string searchQuery)
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

            // Returns a Book[] containing search results
            return resultCollection.ToArray();
        }

        public static void AddToReadingList(Book SelectedBook)
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
                        Console.Clear();
                        MainMenu(Console.In);
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

        public static string ViewReadingList()
        {
            // Tests that MainMenu() option 2 works
            if (TestingMainMenuOption2)
            {
                return "Option 2 worked";
            }
            
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
            Console.Clear();
            MainMenu(Console.In);

            // Used for testing MainMenu() options
            return string.Empty;
        }
    }
}