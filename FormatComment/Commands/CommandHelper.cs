using System.Collections.Generic;
using System.Linq;

namespace FormatComment
{
    public static class CommandHelper
    {
        // 对齐多行中的注释，tabCount 表示左移或右移 tab 的个数
        public static string FormatCommentLineTab(string text, int tabspace, int tabCount)
        {
            char[] anyofEmpty = [' ', '\t', '\n', '\r'];
            var list = new List<(string, int, int)>();
            foreach (string line in text.Split('\n'))
            {
                int commentIndex = line.IndexOf("//");                      // 找到注释
                if (commentIndex == -1)
                {
                    list.Add((line, -1, -1));
                    continue;
                }

                int codeAfterIndex = -1;
                for (int i = commentIndex - 1; i >= 0; i--)
                {
                    if (!anyofEmpty.Contains(line[i]))                      // 找到代码
                    {
                        codeAfterIndex = i + 1;
                        break;
                    }
                }
                list.Add((line, commentIndex, codeAfterIndex));
            }

            var used = list.Where(v => v.Item3 != -1).ToList();
            if (used.Count == 0)
                return "";

            int newCommentIndex = 0;
            if (used.All(v => v.Item2 == used[0].Item2))
            {
                newCommentIndex = used[0].Item2;                            // 注释所在列数全部相等，则该列数为新注释列
                newCommentIndex = tabspace * (newCommentIndex / tabspace + tabCount);
            }
            else
            {
                newCommentIndex = used.Max(v => v.Item3);                   // 新注释列为最后代码列数的最大值
                newCommentIndex = tabspace * (newCommentIndex / tabspace + 1);
            }

            var result = new List<string>();
            foreach ((string line, int commentIndex, int codeAfterIndex) in list)
            {
                if (codeAfterIndex == -1)                                   // 没有代码，则不操作
                {
                    result.Add(line);
                    continue;
                }
                if (newCommentIndex > commentIndex)                         // 新注释往右移动
                {
                    result.Add(line.Substring(0, commentIndex) + new string(' ', newCommentIndex - commentIndex) + line.Substring(commentIndex));
                    continue;
                }
                if (newCommentIndex >= codeAfterIndex)                      // 新注释往左移动，但在代码后面
                {
                    result.Add(line.Substring(0, newCommentIndex) + line.Substring(commentIndex));
                    continue;
                }

                int newIndex = tabspace * (codeAfterIndex / tabspace + 1);
                result.Add(line.Substring(0, codeAfterIndex) + new string(' ', newIndex - codeAfterIndex) + line.Substring(commentIndex));
            }

            return string.Join("\n", [.. result]);
        }

        // 获取注释内容
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

        // 转成 C 语言风格的注释
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

        // 获取最终注释的行
        private static string GetCommentLineText(string content, int contentLength, string prevSpace, int maxColumn, char tag)
        {
            string fill = "";
            int length = maxColumn - (prevSpace.Length + 6 + contentLength);
            if (length > 0)
            {
                fill = new string(tag, length);
            }
            return prevSpace + "/*" + tag + content + fill + tag + "*/";
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

