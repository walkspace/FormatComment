using System.Collections.Generic;
using System.Linq;

namespace FormatComment
{
    public class TextPostion(string text, int position)
    {
        public char[] AnyofEmpty { get; set; } = [' ', '\t', '\n', '\r'];
        public string Text { get; } = text;
        public int Start { get; } = position;
        public int End { get; } = position + text.Length;

        public TextPostion TrimStart(params char[] trimChars)
        {
            var text = Text.TrimStart(trimChars);
            var index = Text.IndexOf(text);
            return Substring(index, text.Length);
        }
        public TextPostion TrimEnd(params char[] trimChars)
        {
            var text = Text.TrimEnd(trimChars);
            var index = Text.LastIndexOf(text);
            return Substring(index, text.Length);
        }
        public TextPostion Trim(params char[] trimChars)
        {
            var text = Text.Trim(trimChars);
            var index = Text.IndexOf(text);
            return Substring(index, text.Length);
        }
        public TextPostion Substring(int startIndex, int length)
        {
            return new TextPostion(Text.Substring(startIndex, length), Start + startIndex);
        }
        public List<TextPostion> Split(string[] separator, StringSplitOptions options)
        {
            List<TextPostion> list = [];

            var arr = Text.Split(separator, StringSplitOptions.None);
            int pos = Start;
            for (int i = 0; i < arr.Length; i++)
            {
                string line = arr[i];
                if (options != StringSplitOptions.RemoveEmptyEntries || line.Trim().Length > 0)
                {
                    list.Add(new TextPostion(line, pos));
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
