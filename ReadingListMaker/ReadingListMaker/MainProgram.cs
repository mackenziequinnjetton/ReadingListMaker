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
        static string MainMenu()
        {
            // Loops until the user inputs a valid menu selection
            while (true)
            {
                Console.WriteLine("   1: Look up a book by title");
                Console.WriteLine("   2: View your current reading list");
                Console.WriteLine("   3: Quit");
                Console.WriteLine();

                // Maintains margin with user prompt
                Console.Write("   ");
                var response = Console.ReadLine();

                // Checks if the user selected a valid menu item
                if (new List<string> { "1", "2", "3" }
                    .Contains(response.Trim()))
                {
                    return response;
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
                            // Book search function here
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
    }
}
