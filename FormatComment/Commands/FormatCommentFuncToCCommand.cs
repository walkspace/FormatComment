using EnvDTE;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System.Collections.Generic;
using System.Linq;

namespace FormatComment
{
    [Command(PackageIds.FormatCommentFuncToCCommand)]
    internal sealed class FormatCommentFuncToCCommand : BaseCommand<FormatCommentFuncToCCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await Package.JoinableTaskFactory.SwitchToMainThreadAsync();

            DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync();
            ITextSnapshot snapshot = docView?.TextView?.TextSnapshot;
            ITextSelection selection = docView?.TextView?.Selection;
            if (snapshot == null || selection == null)
                return;

            IVsTextView vs = docView.TextView.ToIVsTextView();
            if (await Package.GetServiceAsync(typeof(DTE)) is not DTE dte)
                return;

            var maxColumn = General.Instance.MaxColumn - 1;     // VS2022 文档编辑窗口下面显示的列数从 1 开始
            var commentChar = General.Instance.CommentChar;
            GetSelectionTextRange(snapshot, selection, out int start, out int end, out string prevSpace);

            var sour = snapshot.GetText(start, end - start);
            var list = CommandHelper.GetCommentContent(new TextPostion(sour, start));
            foreach (TextPostion line in list)                  // 计算最大列数、每行内容的列数
            {
                if (line.Text.Length == 0)
                    continue;

                vs.GetLineAndColumn(line.Start, out int numBeg, out int idxBeg);
                vs.SetSelection(numBeg, idxBeg, numBeg, idxBeg);
                int? columnBeg = (dte.ActiveDocument.Selection as TextSelection)?.CurrentColumn;

                vs.GetLineAndColumn(line.End, out int numEnd, out int idxEnd);
                vs.SetSelection(numEnd, idxEnd, numEnd, idxEnd);
                int? columnEnd = (dte.ActiveDocument.Selection as TextSelection)?.CurrentColumn;

                int contentLength = line.ColumnWidth;
                if (columnBeg != null && columnEnd != null)
                {
                    contentLength = Math.Max(contentLength, columnEnd.Value - columnBeg.Value);
                    maxColumn = Math.Max(maxColumn, contentLength + prevSpace.Length + 6);
                    line.ColumnWidth = contentLength;
                }
            }
            var result = CommandHelper.FormatCommentToC(list, prevSpace, maxColumn, commentChar);
            var newText = "\n" + result + "\n" + prevSpace;

            // 替换原有字符
            if (start == end)
            {
                docView.TextBuffer?.Insert(start, newText);
            }
            else
            {
                docView.TextBuffer?.Replace(new Span(start, end - start), newText);
            }

            // 设置光标
            snapshot = docView.TextView.TextSnapshot;
            docView.TextView.Selection.Clear();
            docView.TextView.Caret.MoveTo(new SnapshotPoint(snapshot, start + newText.Length));
        }

        private static void GetSelectionTextRange(ITextSnapshot snapshot,
                                                  ITextSelection selection,
                                                  out int start,
                                                  out int end,
                                                  out string prevSpace)
        {
            prevSpace = "";
            start = selection.Start.Position;
            end = selection.End.Position;

            // 判断 selection 前一个位置是否为回车符
            if (start > 0 && snapshot[start - 1] == '\n')
            {
                start--;
            }
            if (start == snapshot.Length || end == snapshot.Length)
                return;

            // 从 selection 后一个位置开始，查找字符
            char[] separator = [' ', '\t', '\r', '\n'];
            int codePos = selection.End.Position;
            while (codePos < snapshot.Length && separator.Contains(snapshot[codePos]))
            {
                codePos++;
            }
            if (codePos == snapshot.Length)
            {
                end = codePos;
                return;
            }

            // 找到代码后，再往回查找回车符，指向回车符后面一个位置
            int afterEnterPos = codePos - 1;
            while (afterEnterPos >= 0 && snapshot[afterEnterPos] != '\n')
            {
                afterEnterPos--;
            }
            afterEnterPos++;

            // 获取当行代码前面的空白字符串，以及需要被替换的文字区域
            prevSpace = snapshot.GetText(afterEnterPos, codePos - afterEnterPos);
            start = Math.Min(start, afterEnterPos);
            end = codePos;
        }

        private static List<(string, int, int)> GetContentText(ITextSnapshot snapshot, int start, int end, char commentChar)
        {
            if (start == snapshot.Length)
                return [];

            char[] trimChars = [' ', '\t', '\r', '\n'];
            start = TrimBeg(snapshot, start, end - 1, trimChars);
            if (start > end - 1)
                return [];

            end = TrimEnd(snapshot, end - 1, start, trimChars) + 1;
            if (end < start)
                return [];

            List<(string, int, int)> lstNewText = [];
            char[] trimCharsBeg = ['-', '=', '*', '/', commentChar];
            char[] trimCharsEnd = ['-', '=', '*', '/', commentChar, ' ', '\t'];
            int lineNumberBeg = snapshot.GetLineNumberFromPosition(start);
            int lineNumberEnd = snapshot.GetLineNumberFromPosition(end);
            for (int i = lineNumberBeg; i <= lineNumberEnd; i++)
            {
                ITextSnapshotLine snapshotLine = snapshot.GetLineFromLineNumber(i);
                int lineBeg = snapshotLine.Start.Position;
                int lineEnd = snapshotLine.End.Position;
                if (lineBeg == lineEnd && snapshot[lineEnd] == '\r')        // 可能存在："\r\n"
                    continue;

                lineBeg = TrimBeg(snapshot, lineBeg, lineEnd - 1, trimChars);
                lineBeg = TrimBeg(snapshot, lineBeg, lineEnd - 1, trimCharsBeg);
                if (lineBeg > lineEnd - 1)
                    continue;

                lineEnd = TrimEnd(snapshot, lineEnd - 1, lineBeg, trimChars) + 1;
                lineEnd = TrimEnd(snapshot, lineEnd - 1, lineBeg, trimCharsEnd) + 1;
                if (lineEnd < lineBeg)
                    continue;

                if (lineBeg < lineEnd)
                {
                    if (snapshot[lineBeg] == ' ')                           // 最后注释行前面会加上 "/* "，如果存在空格，则无需加
                    {
                        lineBeg++;
                    }
                    lstNewText.Add((snapshot.GetText(lineBeg, lineEnd - lineBeg), lineBeg, lineEnd));
                }
            }
            return lstNewText;
        }

        private static int TrimBeg(ITextSnapshot snapshot, int position, int end, char[] trimChars)
        {
            int pos = position;
            while (pos <= end && trimChars.Contains(snapshot[pos]))
            {
                pos++;
            }
            return pos;
        }

        private static int TrimEnd(ITextSnapshot snapshot, int position, int beg, char[] trimChars)
        {
            int pos = position;
            while (pos >= beg && trimChars.Contains(snapshot[pos]))
            {
                pos--;
            }
            return pos;
        }

        private static string GetCommentLineText(string content, int contentLength, string tabspace, int maxColumn, char tag)
        {
            string fill = "";
            int length = maxColumn - (tabspace.Length + 6 + contentLength);
            if (length > 0)
            {
                fill = new string(tag, length);
            }
            return tabspace + "/*" + tag + content + fill + tag + "*/";
        }
    }
}
