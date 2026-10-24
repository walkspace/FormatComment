using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FormatComment
{
    public class TextPostion(string text, int position)
    {
        public string Text { get; } = text;                                         // 文本索引：[0..n]
        public int Start { get; } = position;                                       // 开始位置，第 0   个索引
        public int End { get; } = position + text.Length;                           // 结束位置，第 n+1 个索引
        public int ColumnWidth { get; set; } = text.Split('\n').Max(s => s.Length); // 文本列宽
        public int Length => Text.Length;
        public char this[int index] => Text[index];

        public TextPostion TrimStart(params char[] trimChars)
        {
            var text = Text.TrimStart(trimChars);
            return new(text, End - text.Length);
        }

        public TextPostion TrimEnd(params char[] trimChars)
        {
            var text = Text.TrimEnd(trimChars);
            return new(text, Start);
        }

        public TextPostion Trim(params char[] trimChars)
        {
            var text = Text.Trim(trimChars);
            return new(text, Start + Text.IndexOf(text));
        }

        public TextPostion Substring(int startIndex)
        {
            return new(Text.Substring(startIndex), Start + startIndex);
        }

        public TextPostion Substring(int startIndex, int length)
        {
            return new(Text.Substring(startIndex, length), Start + startIndex);
        }

        public List<TextPostion> Split(string[] separator, StringSplitOptions options = StringSplitOptions.None)
        {
            List<TextPostion> list = [];

            var arr = Text.Split(separator, StringSplitOptions.None);
            var pos = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                var line = arr[i];
                if (options == StringSplitOptions.None || line.Trim().Length > 0)
                {
                    list.Add(new TextPostion(line, Start + pos));
                }
                if (i < arr.Length - 1)
                {
                    pos += line.Length;
                    pos += separator.First(v => pos + v.Length <= Text.Length && Text.Substring(pos, v.Length) == v).Length;
                }
            }
            return list;
        }
    }
}
