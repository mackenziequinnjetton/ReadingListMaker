using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Books.v1;
using Google.Apis.Services;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace ReadingListMaker
{
    internal class MainProgram
    {
        // Indicates whether it is the first time MainMenu() has been called
        private static bool MainMenuFirstCall { get; set; } = true;
        private static bool APIResponsePending { get; set; } = true;
        private static Book SelectedBook { get; set; }
        private static List<Book> ReadingList { get; set; } = new List<Book>();

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

            // Loops until the user inputs a valid menu selection
            string response;
            
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

            while (APIResponsePending)
            {
                Console.Clear();
                Console.WriteLine();
                Console.Write("   Searching...");
                Console.ReadLine();
            }
        }

        static void BookSearchMenu()
        {
            Console.Clear();

            Console.WriteLine();
            Console.WriteLine("   Please enter a book title:");
            Console.WriteLine();
            Console.Write("   ");

            var searchQuery = Console.ReadLine().Trim().ToLower();
            Console.Clear();
            BookSearch(searchQuery);
        }

        static async void BookSearch(string searchQuery)
        {
            Task<IEnumerable<Book>> bookTitleQuery = Task.Run(
                () => BookSearchHelper(searchQuery));

            APIResponsePending = true;

            Console.WriteLine();
            Console.Write("   ");

            

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
                foreach (var author in item.Authors)
                {
                    Console.WriteLine($"   Author:     {author}");
                }
                Console.WriteLine($"   Publisher:  {item.Publisher}");
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

        static IEnumerable<Book> BookSearchHelper(string searchQuery)
        {
            var apiKeyPath = 
                @"C:\Users\macke\OneDrive\Documents\googleBooksAPIKey.txt";

            string apiKey;
            StreamReader apiReader;

            using (FileStream apiFileStream = 
                File.Open(apiKeyPath, FileMode.Open))
            {
                apiReader = new StreamReader(apiFileStream);
                apiKey = apiReader.ReadToEnd();
            }

            var searchQueryWords = searchQuery.Split(' ');

            var apiURL = "https://www.googleapis.com/books/v1/volumes?q=";

            foreach (var word in searchQueryWords)
            {
                apiURL += $"{word}+";
            }

            apiURL = apiURL.Substring(0, apiURL.Length - 1);
            apiURL += $"&maxResults=5&key={apiKey}";

            WebRequest apiRequest = WebRequest.Create(apiURL);
            Stream apiStream = apiRequest.GetResponse().GetResponseStream();

            apiReader = new StreamReader(apiStream);

            var result = apiReader.ReadToEnd().Trim();
            JObject resultObject = JObject.Parse(result);

            var resultCollection =
                from bookInfo in resultObject["items"]
                    .Children<JToken>()["volumeInfo"]
                select new Book
                (
                    bookInfo["title"], 
                    bookInfo["authors"], 
                    bookInfo["publisher"]
                );

            apiRequest.Abort();
            apiStream.Dispose();
            apiReader.Dispose();

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

                if (new List<string> { "1", "2"}.Contains(response))
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

            Console.WriteLine("   Press any key to return to the main menu:");
            Console.WriteLine();
            Console.Write("   ");
            Console.ReadLine();
            MainMenu();
        }
    }
}
