using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using ReadingListMaker;

namespace ReadingListMakerTests
{
    [TestClass]
    public class MainProgramTests
    {
        [TestMethod]
        public void MainMenu_Option1_ShouldWork()
        {
            MainProgram.TestingMainMenuOption1 = true;
            const string input = "1";
            var reader = new StringReader(input);

            var result = MainProgram.MainMenu(reader);

            Assert.AreEqual(result, "Option 1 worked");
        }

        [TestMethod]
        public void MainMenu_Option2_ShouldWork()
        {
            MainProgram.TestingMainMenuOption2 = true;
            const string input = "2";
            var reader = new StringReader(input);

            var result = MainProgram.MainMenu(reader);

            Assert.AreEqual(result, "Option 2 worked");
        }

        [TestMethod]
        public void MainMenu_Option3_ShouldWork()
        {
            MainProgram.TestingMainMenuOption3 = true;
            const string input = "3";
            var reader = new StringReader(input);

            var result = MainProgram.MainMenu(reader);

            Assert.AreEqual(result, "Option 3 worked");
        }
    }
}
