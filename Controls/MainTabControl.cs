using System.Linq;
using System.Windows.Forms;

namespace DeskPad.Controls
{
    public class MainTabControl : TabControl
    {
        private const string TAB_CONTROL_NAME = "MainTabControl";
        private TabControlContextMenuStrip _contextMenuStrip;
        private MainForm _form;

        public MainTabControl()
        {
            _contextMenuStrip = new TabControlContextMenuStrip();

            Name = TAB_CONTROL_NAME;
            ContextMenuStrip = _contextMenuStrip;
            Dock = DockStyle.Fill;

            HandleCreated += (s, e) =>
            {
                _form = FindForm() as MainForm;
            };

            SelectedIndexChanged += (s, e) =>
            {
                _form.CurrentFile = _form.Session.TextFiles[SelectedIndex];
                _form.CurrentRtb = (CustomRichTextBox)_form.MainTabControl.TabPages[SelectedIndex].Controls.Find("RTBTextFileContent", true).First();
                _form.Text = $"{_form.CurrentFile.FileName} - Deskpad";
            };

            MouseUp += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    for (int i = 0; i < TabCount; i++)
                    {
                        var r = GetTabRect(i);

                        if (r.Contains(e.Location))
                        {
                            SelectedIndex = i;
                            break;
                        }
                    }
                }
            };
        }
    }
}