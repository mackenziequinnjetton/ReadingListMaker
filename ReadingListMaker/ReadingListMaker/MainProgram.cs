using System;
using System.Collections.Generic;

namespace ReadingListMaker
{
    internal class MainProgram
    {
        static void Main(string[] args)
        {
            IntroText();
            var response = MainMenu();
            ParseUserChoice(response, "mainMenu");
        }

        // Prints an introductory message informing the user of the
        // program's function and how to enter input
        static void IntroText()
        {
            Console.WriteLine("Welcome! This program searches for books and " +
                "creates a reading list.");
            Console.WriteLine();
            Console.WriteLine("Please enter one of the following:");
            Console.WriteLine();
        }

        // Prints a main menu, asks the user to select a choice,
        // and checks if their choice is one of the menu options
        static string MainMenu()
        {
            // Loops until the user inputs a valid menu selection
            while (true)
            {
                Console.WriteLine("To look up a book by title, enter 1.");
                Console.WriteLine("To view your current reading list, enter 2.");
                Console.WriteLine("To quit, enter 3.");
                Console.WriteLine();

                var response = Console.ReadLine();

                if (new List<string> { "1", "2", "3" }.Contains(response.Trim()))
                {
                    return response;
                }
                else
                {
                    Console.WriteLine("We didn't understand your entry, please try again.");
                    Console.WriteLine();
                }
            }
        }


        static void ParseUserChoice(string response, string menu)
        {
            switch (menu)
            {
                case "mainMenu":
                    switch (response)
                    {
                        case "1":

                    }
                    break;
            }
        }
    }
}
