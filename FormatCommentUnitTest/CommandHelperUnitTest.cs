using FormatComment;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FormatCommentUnitTest
{
    [TestClass]
    public class CommandHelperUnitTest
    {
        [TestMethod]
        public void TestFormatCommentLineTabRight()
        {
            string text = "";
            string result = CommandHelper.FormatCommentLineTab(text, 4, 1);
            Assert.AreEqual(result, "");

            text = "\nint a;\n";
            result = CommandHelper.FormatCommentLineTab(text, 4, 1);
            Assert.AreEqual(result, "");

            text = "1234//comment";
            result = CommandHelper.FormatCommentLineTab(text, 4, 1);
            Assert.AreEqual(result, "1234    //comment");

            text = "1234 //comment";
            result = CommandHelper.FormatCommentLineTab(text, 4, 1);
            Assert.AreEqual(result, "1234    //comment");

            text = "";
            text += "123 //comment\n";
            text += "123 //comment";
            result = CommandHelper.FormatCommentLineTab(text, 4, 1);
            Assert.AreEqual(result, "123     //comment\n123     //comment");

            text = "";
            text += "123 //comment\n";
            text += "//AAA\n";
            text += "123         //comment";
            result = CommandHelper.FormatCommentLineTab(text, 4, 1);
            Assert.AreEqual(result, "123 //comment\n//AAA\n123 //comment");
        }

        [TestMethod]
        public void TestFormatCommentLineTabLeft()
        {
            string text = "";
            string result = CommandHelper.FormatCommentLineTab(text, 4, -1);
            Assert.AreEqual(result, "");

            text = "\nint a;\n";
            result = CommandHelper.FormatCommentLineTab(text, 4, -1);
            Assert.AreEqual(result, "");

            text = "1234//comment";
            result = CommandHelper.FormatCommentLineTab(text, 4, -1);
            Assert.AreEqual(result, "1234    //comment");

            text = "1234 //comment";
            result = CommandHelper.FormatCommentLineTab(text, 4, -1);
            Assert.AreEqual(result, "1234    //comment");

            text = "";
            text += "123   //comment\n";
            text += "123   //comment";
            result = CommandHelper.FormatCommentLineTab(text, 4, -1);
            Assert.AreEqual(result, "123 //comment\n123 //comment");

            text = "";
            text += "123 //comment\n";
            text += "//AAA\n";
            text += "123         //comment";
            result = CommandHelper.FormatCommentLineTab(text, 4, -1);
            Assert.AreEqual(result, "123 //comment\n//AAA\n123 //comment");
        }
    }
}
