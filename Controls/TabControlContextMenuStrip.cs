using DeskPad.Objects;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace DeskPad.Controls
{
    public class TabControlContextMenuStrip : ContextMenuStrip
    {
        private const string NAME = "TabControlContextMenuStrip";
        private MainForm _form;

        public TabControlContextMenuStrip()
        {
            Name = NAME;


            var closeTab = new ToolStripMenuItem("Fermer");
            var closeAllTabEcecptThis = new ToolStripMenuItem("Fermer tout sauf ce fichier");
            var openFileInExplorer = new ToolStripMenuItem("Ouvrir le répertoire du fichier en cour");

            Items.AddRange(new ToolStripItem[] { closeTab, closeAllTabEcecptThis, openFileInExplorer });

            HandleCreated += (s, e) =>
            {
                _form = SourceControl.FindForm() as MainForm;
            };

            closeTab.Click += (s, e) =>
            {
                var selectedTab = _form.MainTabControl.SelectedTab;

                _form.Session.TextFiles.Remove(_form.CurrentFile);

                if (_form.MainTabControl.TabCount > 1)
                {
                    _form.MainTabControl.TabPages.Remove(selectedTab);
                    var newIndex = _form.MainTabControl.TabCount - 1;
                    _form.MainTabControl.SelectedIndex = newIndex;
                    _form.CurrentFile = _form.Session.TextFiles[newIndex];

                }
                else
                {
                    var fileName = "Sans titre 1";
                    var file = new TextFile(fileName);

                    _form.CurrentFile = file;
                    _form.CurrentRtb.Clear();

                    _form.MainTabControl.SelectedTab.Text = file.FileName;
                    _form.Session.TextFiles.Add(file);
                    _form.Text = "Sans titre 1 - Deskpad";
                };
            };

            closeAllTabEcecptThis.Click += (s, e) =>
            {
                var filesToDelete = new List<TextFile>();

                if (_form.MainTabControl.TabCount > 1)
                {
                    TabPage selectedTab = _form.MainTabControl.SelectedTab;

                    foreach (TabPage tabPage in _form.MainTabControl.TabPages)
                    {
                        if (tabPage != selectedTab)
                        {
                            _form.MainTabControl.TabPages.Remove(tabPage);

                        }
                    }

                    foreach (var file in _form.Session.TextFiles)
                    {
                        if (file != _form.CurrentFile)
                        {
                            filesToDelete.Add(file);
                        }
                    }

                    _form.Session.TextFiles = _form.Session.TextFiles.Except(filesToDelete).ToList();
                }
            };

            openFileInExplorer.Click += (s, e) =>
            {
                var args = $"/select, \"{_form.CurrentFile.FileName}\"";
                Process.Start("explorer.exe", args);
            };

        }
    }
}
