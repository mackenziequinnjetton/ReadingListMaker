This program allows users to query the Google Books API for books by title, select a book from the 5 displayed, and add it to a reading list. The user can also view their reading list.

PREREQUISITES

You will need .NET Runtime. The latest version at the time of this writing can be downloaded from https://dotnet.microsoft.com/en-us/download, or downloaded and installed through Visual Studio (2019 or higher recommended).

INSTALLATION

Clone the repository to your local machine by running: 

```git clone https://github.com/mackenziequinnjetton/ReadingListMaker.git```

Alternatively, you may clone the repository using GitHub's desktop client or Visual Studio.

RUNNING

I recommend opening ReadingListMaker.sln in Visual Studio and first building the solution and then running it from there, using the tools in the GUI. Alternatively, you may navigate to the local repository's directory containing the solution and type:

```dotnet build .\ReadingListMaker.sln```

```cd .\ReadingListMaker\```

```dotnet run .\MainProgram.cs```

TESTING

Again, I recommend using Visual Studio built-in testing capabilities to run the tests specified in ReadingListMakerTests. Alternatively, you may navigate to the local repository's folder containing the solution and type:

```dotnet test .\ReadingListMaker.sln```

NOTES

Please note that at the time of this writing, the reading list does not persist between sessions. Note also that the program is structured such that it reads the Google API key from a .txt file outside the repository. To make this program run properly, you will need to paste your API key into a .txt file with no other contents, save it to a location on your computer, and change the path on the first line of the BookSearchHelper function to point to the location of your API key file.


If you have any questions regarding this repository, you may contact me at mackenziequinnjetton@gmail.com.
