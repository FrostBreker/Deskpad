using System.Windows.Forms;

namespace DeskPad.Controls
{
    class RichTextBoxContextMenuStrip : ContextMenuStrip
    {
        private RichTextBox _richTextBox;
        private const string NAME = "RTBContextMenuStrip";
        public RichTextBoxContextMenuStrip(RichTextBox richTextBox)
        {
            _richTextBox = richTextBox;
            //Name = NAME;

            var cut = new ToolStripMenuItem("Couper", null, null, Keys.Control | Keys.X);
            var copy = new ToolStripMenuItem("Copier", null, null, Keys.Control | Keys.C);
            var paste = new ToolStripMenuItem("Coller", null, null, Keys.Control | Keys.V);
            var selectAll = new ToolStripMenuItem("Sélectionner tout", null, null, Keys.Control | Keys.A);

            cut.Click += (s, e) => _richTextBox.Cut();
            copy.Click += (s, e) => _richTextBox.Copy();
            paste.Click += (s, e) => _richTextBox.Paste();
            selectAll.Click += (s, e) => _richTextBox.SelectAll();

            Items.AddRange(new ToolStripItem[] { cut, copy, paste, selectAll });
        }
    }
}
