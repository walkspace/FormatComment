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

        [TestMethod]
        public void TestFormatCommentToC()
        {
            string text = "";
            text += "/// <summary>\n";
            text += "///\n";
            text += " \t \n";
            text += "/// </summary>\n";
            text += "/*******/\n";
            text += "/* AA*/ \n";
            text += "///BB \n";
            text += "//CC \n";

            int maxColumn = 20;
            var prevSpace = " ";
            var list = CommandHelper.GetCommentContent(new TextPostion(text, 0));
            string result = CommandHelper.FormatCommentToC(list, prevSpace, maxColumn, '-');
            string newText = "";
            newText += " /*---------------*/\n";
            newText += " /*               */\n";
            newText += " /*               */\n";
            newText += " /* AA            */\n";
            newText += " /* BB            */\n";
            newText += " /* CC            */\n";
            newText += " /*---------------*/";
            Assert.AreEqual(newText, result);
        }
    }
}
