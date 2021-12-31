using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            var result = MainProgram.MainMenu();

            Assert.AreEqual("Option 1 worked", result);

            MainProgram.TestingMainMenuOption1 = false;
        }

        [TestMethod]
        public void MainMenu_Option2_ShouldWork()
        {
            MainProgram.TestingMainMenuOption2 = true;

            var result = MainProgram.MainMenu();

            Assert.AreEqual("Option 2 worked", result);

            MainProgram.TestingMainMenuOption2 = false;
        }

        [TestMethod]
        public void MainMenu_Option3_ShouldWork()
        {
            MainProgram.TestingMainMenuOption3 = true;

            var result = MainProgram.MainMenu();

            Assert.AreEqual("Option 3 worked", result);

            MainProgram.TestingMainMenuOption3 = false;
        }
    }
}
