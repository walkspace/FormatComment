using FormatComment;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class TextPostionTest
    {
        [TestMethod]
        public void TrimTest()
        {
            TextPostion actual;
            TextPostion source = new TextPostion("HxxaxxH", 10);

            // 正常测试
            actual = source.TrimStart(new char[] { 'H' });
            Assert.AreEqual(11, actual.Start);
            Assert.AreEqual("xxaxxH", actual.Text);

            actual = source.TrimEnd(new char[] { 'H' });
            Assert.AreEqual(10, actual.Start);
            Assert.AreEqual("Hxxaxx", actual.Text);

            actual = source.Trim(new char[] { 'H' });
            Assert.AreEqual(11, actual.Start);
            Assert.AreEqual("xxaxx", actual.Text);

            // 边界测试
            actual = source.TrimStart(new char[] { 'H', 'x', 'a' });
            Assert.AreEqual(17, actual.Start);
            Assert.AreEqual("", actual.Text);

            actual = source.TrimEnd(new char[] { 'H', 'x', 'a' });
            Assert.AreEqual(10, actual.Start);
            Assert.AreEqual("", actual.Text);

            actual = source.Trim(new char[] { 'H', 'x', 'a' });
            Assert.AreEqual(10, actual.Start);
            Assert.AreEqual("", actual.Text);
        }

        [TestMethod]
        public void SubstringTest()
        {
            TextPostion actual;
            TextPostion source = new TextPostion("HxxaxxH", 10);

            // 正常测试
            actual = source.Substring(1, 5);
            Assert.AreEqual(11, actual.Start);
            Assert.AreEqual("xxaxx", actual.Text);

            actual = source.Substring(1);
            Assert.AreEqual(11, actual.Start);
            Assert.AreEqual("xxaxxH", actual.Text);

            // 边界测试
            actual = source.Substring(7);
            Assert.AreEqual(17, actual.Start);
            Assert.AreEqual("", actual.Text);
        }

        [TestMethod]
        public void SplitTest()
        {
            List<TextPostion> actual;
            TextPostion source = new TextPostion("H == == a == H", 10);

            // 正常测试 - 1
            actual = source.Split(new string[] { "==" }, StringSplitOptions.None);
            Assert.AreEqual(4, actual.Count);

            Assert.AreEqual(10, actual[0].Start);
            Assert.AreEqual("H ", actual[0].Text);

            Assert.AreEqual(14, actual[1].Start);
            Assert.AreEqual(" ", actual[1].Text);

            Assert.AreEqual(17, actual[2].Start);
            Assert.AreEqual(" a ", actual[2].Text);

            Assert.AreEqual(22, actual[3].Start);
            Assert.AreEqual(" H", actual[3].Text);

            // 正常测试 - 2
            actual = source.Split(new string[] { "==" }, StringSplitOptions.RemoveEmptyEntries);
            Assert.AreEqual(3, actual.Count);

            Assert.AreEqual(10, actual[0].Start);
            Assert.AreEqual("H ", actual[0].Text);

            Assert.AreEqual(17, actual[1].Start);
            Assert.AreEqual(" a ", actual[1].Text);

            Assert.AreEqual(22, actual[2].Start);
            Assert.AreEqual(" H", actual[2].Text);
        }
    }
}
