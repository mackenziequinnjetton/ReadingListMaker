﻿using System;
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
        // private static IEnumerable<Book> SearchResult { get; set; }

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
            Console.WriteLine();
        }

        // Prints a main menu, asks the user to select a choice,
        // and checks if their choice is one of the menu options
        static void MainMenu()
        {
            Console.WriteLine("   1: Look up a book by title");
            Console.WriteLine("   2: View your current reading list");
            Console.WriteLine("   3: Quit");
            Console.WriteLine();

            // If it is not the first call of MainMenu(), clear the console
            if (!MainMenuFirstCall)
            {
                Console.Clear();
            }

            // Loops until the user inputs a valid menu selection
            while (true)
            {

                // Maintains margin with user prompt
                Console.Write("   ");
                var response = Console.ReadLine();

                // Checks if the user selected a valid menu item
                if (new List<string> { "1", "2", "3" }
                    .Contains(response.Trim()))
                {

                    // For future calls of MainMenu,
                    // the console will be cleared first
                    MainMenuFirstCall = false;
                    ParseUserChoice(response, "mainMenu");

                    if (response == "1")
                    {    
                        while (true)
                        {
                            Console.Clear();
                            Console.WriteLine();
                            Console.Write("   Searching...");
                            Console.ReadLine();
                        }
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

        // Parses user menu input from anywhere in the program
        // and calls the appropriate function
        static void ParseUserChoice(string response, string menu)
        {

            // Note that before this function is called, user input
            // will already have been validated by the calling function

            // Outer switch function checks which menu the user's 
            // input is from
            switch (menu)
            {
                case "mainMenu":
                    
                    // Inner switch function checks which choice from that
                    // menu the user selected and calls the
                    // appropriate function
                    switch (response)
                    {
                        case "1":
                            BookSearchMenu();
                            break;
                        case "2":
                            // Reading list display function here
                            break;
                        case "3":
                            // Quit function here
                            break;
                    }
                    break;
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

            Console.WriteLine();
            Console.Write("   ");

            await bookTitleQuery;

            var searchResult = bookTitleQuery.Result;

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

            Console.WriteLine("Please select a result to add to reading list");
            Console.WriteLine("(enter 1-5, or press ESC to return " +
                "to the main menu): ");
            Console.WriteLine();

            var responseKey = Console.ReadKey();

            if (responseKey.Key == ConsoleKey.Escape)
            {
                MainMenu();
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
    }
}
