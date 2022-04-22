using DeskPad.Objects;
using DeskPad.Services;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DeskPad.Controls
{
    public class MainMenuStrip : MenuStrip
    {
        private const string MENU_NAME = "MainMenuStrip";

        private MainForm _form;
        private FontDialog _fontDialog;
        private OpenFileDialog _openFileDialog;
        private SaveFileDialog _saveFileDialog;

        public MainMenuStrip()
        {
            Name = MENU_NAME;
            Dock = DockStyle.Top;

            _fontDialog = new FontDialog();
            _openFileDialog = new OpenFileDialog();
            _saveFileDialog = new SaveFileDialog();

            FileDropDownMenu();
            EditDrowpDownMenu();
            FormatDropDownMenu();
            ViewDropDownMenu();

            HandleCreated += (s, e) =>
            {
                _form = FindForm() as MainForm;
            };
        }

        public void FileDropDownMenu()
        {
            var fileDropDownMenu = new ToolStripMenuItem("Fichier");

            var newMenu = new ToolStripMenuItem("Nouveau", null, null, Keys.Control | Keys.N);
            var openMenu = new ToolStripMenuItem("Ouvrir...", null, null, Keys.Control | Keys.O);
            var saveMenu = new ToolStripMenuItem("Enregister", null, null, Keys.Control | Keys.S);
            var saveAsMenu = new ToolStripMenuItem("Enregister sous...", null, null, Keys.Control | Keys.Shift | Keys.S);
            var quitMenu = new ToolStripMenuItem("Quitter", null, null, Keys.Alt | Keys.F4);

            newMenu.Click += (s, e) =>
            {
                var tabControl = _form.MainTabControl;
                var tabPagesCount = tabControl.TabCount;

                var fileName = $"Sans titre {tabPagesCount + 1}";
                var newFile = new TextFile(fileName);
                var rtb = new CustomRichTextBox();

                tabControl.TabPages.Add(newFile.SafeFileName);
                var newTabPage = tabControl.TabPages[tabPagesCount];

                newTabPage.Controls.Add(rtb);
                _form.Session.TextFiles.Add(newFile);
                tabControl.SelectedTab = newTabPage;
                _form.CurrentFile = newFile;
                _form.CurrentRtb = rtb;
            };

            openMenu.Click += async (s, e) =>
            {
                if (_openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var tabControl = _form.MainTabControl;
                    var tabPagesCount = tabControl.TabCount;

                    var file = new TextFile(_openFileDialog.FileName);
                    var rtb = new CustomRichTextBox();

                    _form.Text = $"{file.FileName} - DeskPad";

                    using (StreamReader reader = new StreamReader(file.FileName))
                    {
                        file.Content = await reader.ReadToEndAsync();
                    };

                    rtb.Text = file.Content;

                    tabControl.TabPages.Add(file.SafeFileName);
                    tabControl.TabPages[tabPagesCount].Controls.Add(rtb);

                    _form.Session.TextFiles.Add(file);
                    _form.CurrentRtb = rtb;
                    _form.CurrentFile = file;
                    tabControl.SelectedTab = tabControl.TabPages[tabPagesCount];
                }
            };

            saveMenu.Click += async (s, e) =>
            {
                var currentFile = _form.CurrentFile;
                var currentRtbText = _form.CurrentRtb.Text;

                
                    if (File.Exists(currentFile.FileName))
                    {
                        using (StreamWriter writer = File.CreateText(currentFile.FileName))
                        {
                            await writer.WriteAsync(currentFile.Content);
                        }
                        currentFile.Content = currentRtbText;

                        _form.Text = currentFile.FileName;
                        _form.MainTabControl.SelectedTab.Text = currentFile.SafeFileName;
                    }
                    else
                    {
                        saveAsMenu.PerformClick();
                    }
            };

            saveAsMenu.Click += async (s, e) =>
            {
                if (_saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var newFileName = _saveFileDialog.FileName;
                    var alreadyExists = false;

                    foreach (var file in _form.Session.TextFiles)
                    {
                        if (file.FileName == newFileName)
                        {
                            MessageBox.Show(
                                "Ce fichier est déja ouvert dans Deskpad.",
                                "ERREUR",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            alreadyExists = true;
                            break;
                        }
                    }

                    if (!alreadyExists)
                    {
                        var file = new TextFile(newFileName) { Content = _form.CurrentRtb.Text };

                        var oldFile = _form.Session.TextFiles.Where(x => x.FileName == _form.CurrentFile.FileName).First();

                        _form.Session.TextFiles.Replace(oldFile, file);

                        using (StreamWriter writer = File.CreateText(file.FileName))
                        {
                            await writer.WriteAsync(file.Content);
                        }

                        _form.MainTabControl.SelectedTab.Text = file.SafeFileName;
                        _form.Text = file.FileName;
                        _form.CurrentFile = file;
                    };
                }
            };

            quitMenu.Click += (s, e) =>
            {
                Application.Exit();
            };

            fileDropDownMenu.DropDownItems.AddRange(new ToolStripItem[] { newMenu, openMenu, saveMenu, saveAsMenu, quitMenu });

            Items.Add(fileDropDownMenu);
        }

        public void EditDrowpDownMenu()
        {
            var editDropDownMenu = new ToolStripMenuItem("Edition");

            var cancelMenu = new ToolStripMenuItem("Annuler", null, null, Keys.Control | Keys.Z);
            var restoreMenu = new ToolStripMenuItem("Restaurer...", null, null, Keys.Control | Keys.Y);


            cancelMenu.Click += (s, e) => { if (_form.CurrentRtb.CanUndo) _form.CurrentRtb.Undo(); };
            restoreMenu.Click += (s, e) => { if (_form.CurrentRtb.CanRedo) _form.CurrentRtb.Redo(); };

            editDropDownMenu.DropDownItems.AddRange(new ToolStripItem[] { cancelMenu, restoreMenu });

            Items.Add(editDropDownMenu);
        }

        public void FormatDropDownMenu()
        {
            var formatDropDownMenu = new ToolStripMenuItem("Edition");

            var fontMenu = new ToolStripMenuItem("Police...");
            fontMenu.Click += (s, e) =>
            {
                _fontDialog.Font = _form.CurrentRtb.Font;
                _fontDialog.ShowDialog();

                _form.CurrentRtb.Font = _fontDialog.Font;
            };

            formatDropDownMenu.DropDownItems.AddRange(new ToolStripItem[] { fontMenu });

            Items.Add(formatDropDownMenu);
        }

        public void ViewDropDownMenu()
        {
            var viewDropDown = new ToolStripMenuItem("Affichage");
            var alwaysOnTop = new ToolStripMenuItem("Toujour devant");

            var zoomDropDown = new ToolStripMenuItem("Zoom");
            var zoomIn = new ToolStripMenuItem("Zoom avant", null, null, Keys.Control | Keys.Up);
            var zoomOut = new ToolStripMenuItem("Zoom arrière", null, null, Keys.Control | Keys.Down);
            var zoomReset = new ToolStripMenuItem("Reset", null, null, Keys.Control | Keys.Subtract);

            alwaysOnTop.Click += (s, e) =>
            {
                if (alwaysOnTop.Checked)
                {
                    alwaysOnTop.Checked = false;
                    Program.MainForm.TopMost = false;
                }
                else
                {
                    alwaysOnTop.Checked = true;
                    Program.MainForm.TopMost = true;
                }
            };

            zoomIn.Click += (s, e) =>
            {
                if (_form.CurrentRtb.ZoomFactor < 3F)
                {
                    _form.CurrentRtb.ZoomFactor += 0.3F;


                };
            };
            zoomOut.Click += (s, e) =>
            {
                if (_form.CurrentRtb.ZoomFactor > 0.6F)
                {
                    _form.CurrentRtb.ZoomFactor -= 0.3F;
                }
            };
            zoomReset.Click += (s, e) => _form.CurrentRtb.ZoomFactor = 1F;

            zoomDropDown.DropDownItems.AddRange(new ToolStripItem[] { zoomIn, zoomOut, zoomReset });
            viewDropDown.DropDownItems.AddRange(new ToolStripItem[] { alwaysOnTop, zoomDropDown });

            Items.Add(viewDropDown);
        }
    }
}
