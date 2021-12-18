using System;

namespace ReadingListMaker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome! This program searches for books and " +
                "creates a reading list.");
            Console.WriteLine();
            Console.WriteLine("Please enter one of the following:");
            Console.WriteLine();
            Console.WriteLine("To look up a book by title, enter 1.");
            Console.WriteLine("To view your current reading list, enter 2.");
            Console.WriteLine("To quit, enter 3.");
            Console.WriteLine();
            Console.ReadLine();
        }
    }
}
