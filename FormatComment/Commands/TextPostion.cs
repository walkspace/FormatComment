using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FormatComment
{
    public class TextPostion(string text, int position) : IEnumerable<char>
    {
        public string Text { get; } = text;                                         // 文本
        public int Start { get; } = position;                                       // 开始位置
        public int End { get; } = position + text.Length;                           // 结束位置
        public int ColumnWidth { get; set; } = text.Split('\n').Max(s => s.Length); // 文本列宽
        public int Length => Text.Length;

        public char this[int index] => Text[index];

        public TextPostion TrimStart(params char[] trimChars)
        {
            var text = Text.TrimStart(trimChars);
            if (text.Length == 0)
                return new TextPostion("", End);
            else
                return Substring(Length - text.Length, text.Length);
        }
        public TextPostion TrimEnd(params char[] trimChars)
        {
            var text = Text.TrimEnd(trimChars);
            if (text.Length == 0)
                return new TextPostion("", Start);
            else
                return Substring(0, text.Length);
        }
        public TextPostion Trim(params char[] trimChars)
        {
            var text = Text.Trim(trimChars);
            if (text.Length == 0)
                return new TextPostion("", Start);
            else
                return Substring(Text.IndexOf(text), text.Length);
        }
        public TextPostion Substring(int startIndex)
        {
            return new TextPostion(Text.Substring(startIndex), Start + startIndex);
        }
        public TextPostion Substring(int startIndex, int length)
        {
            return new TextPostion(Text.Substring(startIndex, length), Start + startIndex);
        }
        public List<TextPostion> Split(string[] separator, StringSplitOptions options = StringSplitOptions.None)
        {
            List<TextPostion> list = [];

            var arr = Text.Split(separator, StringSplitOptions.None);
            int pos = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                string line = arr[i];
                if (options != StringSplitOptions.RemoveEmptyEntries || line.Trim().Length > 0)
                {
                    list.Add(new TextPostion(line, pos + Start));
                }
                if (i < arr.Length - 1)
                {
                    pos += line.Length;
                    pos += separator.First(v => pos + v.Length <= Text.Length && Text.Substring(pos, v.Length) == v).Length;
                }
            }
            return list;
        }

        public IEnumerator<char> GetEnumerator()
        {
            return ((IEnumerable<char>)Text).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Text).GetEnumerator();
        }
    }
}
