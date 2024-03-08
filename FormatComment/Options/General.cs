using System.ComponentModel;
using System.Runtime.InteropServices;

namespace FormatComment
{
    internal partial class OptionsProvider
    {
        // Register the options with this attribute on your package class:
        // [ProvideOptionPage(typeof(OptionsProvider.GeneralOptions), "FormatComment", "General", 0, 0, true, SupportsProfiles = true)]
        [ComVisible(true)]
        public class GeneralOptions : BaseOptionPage<General> { }
    }

    public class General : BaseOptionModel<General>
    {
        [Category("Settings")]
        [DisplayName("Maximum Column")]
        [DefaultValue(120)]
        public int MaxColumn { get; set; } = 120;

        [Category("Settings")]
        [DisplayName("Comment Text")]
        [DefaultValue("-")]
        public char CommentChar { get; set; } = '-';

        [Category("Settings")]
        [DisplayName("Tab Space")]
        [DefaultValue("4")]
        public int TabSpace { get; set; } = 4;
    }
}
