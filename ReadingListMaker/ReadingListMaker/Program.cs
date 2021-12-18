using System;
using System.Collections.Generic;

namespace ReadingListMaker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
        }

        static void IntroText()
        {
            Console.WriteLine("Welcome! This program searches for books and " +
                "creates a reading list.");
            Console.WriteLine();
            Console.WriteLine("Please enter one of the following:");
            Console.WriteLine();
        }

        static string MainMenu()
        {
            Console.WriteLine("To look up a book by title, enter 1.");
            Console.WriteLine("To view your current reading list, enter 2.");
            Console.WriteLine("To quit, enter 3.");
            Console.WriteLine();
            
            var response = Console.ReadLine();

            if (new List<string> {"1", "2", "3"}.Contains(response.Trim()))
            {
                return response;
            }
            else { }
        }
    }
}
