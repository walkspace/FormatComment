using EnvDTE;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace FormatComment
{
    [Command(PackageIds.FormatCommentLineTabLeftCommand)]
    internal sealed class FormatCommentLineTabLeftCommand : BaseCommand<FormatCommentLineTabLeftCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await Package.JoinableTaskFactory.SwitchToMainThreadAsync();

            DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync();
            ITextSnapshot snapshot = docView?.TextView?.TextSnapshot;
            ITextSelection selection = docView?.TextView?.Selection;
            if (snapshot == null || selection == null)
                return;

            int tabspace = General.Instance.TabSpace;
            int start = selection.Start.Position;
            int end = selection.End.Position;
            string oldText = snapshot.GetText(start, end - start);
            string newText = CommandHelper.FormatCommentLineTab(oldText, tabspace, -1);
            if (newText != "")
            {
                docView.TextBuffer?.Replace(new Span(start, end - start), newText);                             // 替换原有字符

                snapshot = docView.TextView.TextSnapshot;
                docView.TextView.Selection.Select(new SnapshotSpan(snapshot, start, newText.Length), false);    // 重新选择
                docView.TextView.Caret.MoveTo(new SnapshotPoint(snapshot, start + newText.Length));             // 设置光标
            }
        }
    }
}
