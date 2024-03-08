using System.Collections;
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
            var curr = new string(text.Where(c => c != '\r').ToArray());    // 删除 '\r'
            var list = new List<(string, int, int)>();
            foreach (string line in curr.Split('\n'))
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
    }
}

