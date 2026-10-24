
using FormatComment;
using ICSharpCode.Decompiler.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace UnitTest
{
    [TestClass]
    public class CommandHelperTest
    {
        [TestMethod]
        public void FindCommentIndexTest()
        {
            int index = CommandHelper.FindCommentIndex("int w = 0;");
            Assert.AreEqual(-1, index);

            index = CommandHelper.FindCommentIndex("int w = 0;// xx");
            Assert.AreEqual(10, index);

            index = CommandHelper.FindCommentIndex("var w = \"0 //xx\"; // This should");
            Assert.AreEqual(18, index);

            index = CommandHelper.FindCommentIndex("var w = \"0 //xx\"; /*aaa*/");
            Assert.AreEqual(18, index);

            index = CommandHelper.FindCommentIndex("var w = '//'/*aaa*/; /*aaa*/   ");
            Assert.AreEqual(21, index);
        }

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
