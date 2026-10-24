using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FormatComment
{
    public static class CommandHelper
    {
        /// <summary>
        /// 在代码行中找到最后注释的索引，如果不存在则返回 -1
        /// </summary>
        public static int FindCommentIndex(string codeLine)
        {
            int GetIndex(List<string> list, string tag)
            {
                if (tag == list[0])
                    return 0;
                if (tag == list[1])
                    return 1;
                if (tag[0] == list[2][0])
                    return 2;
                if (tag[0] == list[3][0])
                    return 3;
                return -1;
            }

            var line = codeLine.TrimEnd();
            var listPrev = new List<string> { "//", "/*", "\"", "'" };
            var listBack = new List<string> { "//", "*/", "\"", "'" };
            int result = -1;
            int index = -1;
            for (int i = 0; i < line.Length; i++)
            {
                string str = i == line.Length - 1 ? new([line[i]]) : new([line[i], line[i + 1]]);
                if (index == -1)
                {
                    index = GetIndex(listPrev, str);
                    if (index == 0)
                        return i;

                    if (index == 1)
                        result = i;
                }
                else
                {
                    var now = GetIndex(listBack, str);
                    if (index == now)
                        index = -1;
                }
            }

            return result;
        }

        /// <summary>
        /// 对齐多行中的注释
        /// </summary>
        /// <param name="textCode">在代码行后面进行注释的多行代码</param>
        /// <param name="tabspace">等于 Tab 宽度的空格数，一般设为 4 个空格</param>
        /// <param name="tabCount">左移或右移 Tab 的个数</param>
        /// <returns>注释对齐后的代码</returns>
        public static string FormatCommentLineTab(string textCode, int tabspace, int tabCount)
        {
            var anyofEmpty = new char[] { ' ', '\t', '\n', '\r' };                      // 空字符
            var list = new List<(string line, int commentIndex, int codeEndIndex)>();   // 代码行，开始注释索引，代码结尾索引
            foreach (string line in textCode.Split('\n'))
            {
                int commentIndex = FindCommentIndex(line);                              // 找到注释
                if (commentIndex == -1)
                {
                    list.Add((line, -1, -1));
                    continue;
                }

                int codeEndIndex = -1;
                for (int i = commentIndex - 1; i >= 0; i--)                             // 从注释开始往前找到代码
                {
                    if (!anyofEmpty.Contains(line[i]))
                    {
                        codeEndIndex = i + 1;
                        break;
                    }
                }
                list.Add((line, commentIndex, codeEndIndex));
            }

            var usedCodeLines = list.Where(v => v.codeEndIndex != -1).ToList();
            if (usedCodeLines.Count == 0)
                return "";

            var usedCodeFirst = usedCodeLines.First();
            var newCommentIndex = 0;
            if (usedCodeLines.Skip(1).All(v => v.commentIndex == usedCodeFirst.commentIndex))
            {
                newCommentIndex = usedCodeFirst.commentIndex;                            // 注释所在列数全部相等，则该列数为新注释列
                newCommentIndex = tabspace * (newCommentIndex / tabspace + tabCount);
            }
            else
            {
                newCommentIndex = usedCodeLines.Max(v => v.codeEndIndex);               // 新注释列为最后代码列数的最大值
                newCommentIndex = tabspace * (newCommentIndex / tabspace + 1);
            }

            var result = new List<string>();
            foreach ((string line, int commentIndex, int codeEndIndex) in list)
            {
                if (codeEndIndex == -1)                                                 // 没有代码，则不操作
                {
                    result.Add(line);
                    continue;
                }

                if (newCommentIndex > commentIndex)                                     // 新注释往右移动
                {
                    result.Add(line.Substring(0, commentIndex) + new string(' ', newCommentIndex - commentIndex) + line.Substring(commentIndex));
                    continue;
                }

                if (newCommentIndex >= codeEndIndex)                                    // 新注释往左移动，但在代码后面
                {
                    result.Add(line.Substring(0, newCommentIndex) + line.Substring(commentIndex));
                    continue;
                }

                int newIndex = tabspace * (codeEndIndex / tabspace + 1);
                result.Add(line.Substring(0, codeEndIndex) + new string(' ', newIndex - codeEndIndex) + line.Substring(commentIndex));
            }

            return string.Join("\n", [.. result]);
        }

        /// <summary>
        /// 获取注释内容
        /// </summary>
        public static List<TextPostion> GetCommentContent(TextPostion text)
        {
            char[] anyOfComment = ['/', '*', '-', '='];
            string[] equalOfComment =
            [
                "<summary>",
                "</summary>",
                "<returns></returns>"
            ];

            List<TextPostion> list = [];
            foreach (TextPostion line in text.Trim().Split(["\n"]))
            {
                TextPostion pos = line.Trim();
                TextPostion rnt = pos;
                if (pos.Length >= 4 && pos.Text.Substring(0, 2) == "/*" && pos.Text.Substring(pos.Text.Length - 2) == "*/")
                {
                    rnt = pos.Substring(2, pos.Text.Length - 4).TrimEnd();      // 注释 "/**/"
                }
                else if (pos.Length >= 3 && pos.Text.Substring(0, 3) == "///")  // 注释 "///"
                {
                    rnt = pos.Substring(3);
                }
                else if (pos.Length >= 2 && pos.Text.Substring(0, 2) == "//")   // 注释："//"
                {
                    rnt = pos.Substring(2);
                }

                string dst = rnt.Text.Trim();
                if (equalOfComment.Contains(dst))                               // 注释标识，比如 "<summary>" 则删除
                    continue;

                if (dst.Length > 0 && dst.All(c => anyOfComment.Contains(c)))   // 比如全是 = 或 * 则删除
                    continue;

                list.Add(rnt.Length >= 1 && rnt[0] == ' ' ? rnt.Substring(1) : rnt);
            }
            return list;
        }

        /// <summary>
        /// 转成 C 语言风格的注释
        /// </summary>
        /// <param name="list">代码行</param>
        /// <param name="prevSpace">注释在代码上面，需要和代码对齐，需要在此设置空格数</param>
        /// <param name="maxColumn">注释的最长列宽</param>
        /// <param name="commentChar">注释符号，比如："---", "==="</param>
        /// <returns></returns>
        public static string FormatCommentToC(List<TextPostion> list, string prevSpace, int maxColumn, char commentChar)
        {
            List<string> result = [];
            result.Add(GetCommentLineText("", 0, prevSpace, maxColumn, commentChar));
            foreach (TextPostion line in list)
            {
                if (line.Text.Length == 0)
                {
                    result.Add(GetCommentLineText("", 0, prevSpace, maxColumn, ' '));
                }
                else
                {
                    result.Add(GetCommentLineText(line.Text, line.ColumnWidth, prevSpace, maxColumn, ' '));
                }
            }
            result.Add(GetCommentLineText("", 0, prevSpace, maxColumn, commentChar));

            return string.Join("\n", result);
        }

        /// <summary>
        /// 获取最终注释的行
        /// </summary>
        /// <param name="codeLine">一行代码</param>
        /// <param name="codeLength">代码长度</param>
        /// <param name="prevSpace">注释在代码上面，需要和代码对齐，需要在此设置空格数</param>
        /// <param name="maxColumn">注释的最长列宽</param>
        /// <param name="tag">注释符号，内容需要空格填充</param>
        /// <returns></returns>
        private static string GetCommentLineText(string codeLine, int codeLength, string prevSpace, int maxColumn, char tag)
        {
            string fill = "";
            int length = maxColumn - (prevSpace.Length + 6 + codeLength);
            if (length > 0)
            {
                fill = new string(tag, length);
            }
            return prevSpace + "/*" + tag + codeLine + fill + tag + "*/";
        }
    }

    public class TextLine
    {
        public TextPostion TextPostion { get; set; }
        public List<TextPostion> List { get; set; } = [];
    }

    public class TextContent
    {
        public List<TextLine> List { get; set; } = [];
    }
}

