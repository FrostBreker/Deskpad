using DeskPad.Controls;
using DeskPad.Objects;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DeskPad
{
    public partial class MainForm : Form
    {
        public RichTextBox CurrentRtb;
        public TextFile CurrentFile;
        public TabControl MainTabControl;
        public Session Session;

        public MainForm()
        {
            InitializeComponent();

            var menuStrip = new MainMenuStrip();
            MainTabControl = new MainTabControl();
            CurrentRtb = new CustomRichTextBox();

            Controls.AddRange(new Control[] { MainTabControl, menuStrip });

            InitializeFile();
        }

        private async void InitializeFile()
        {
            Session = await Session.Load();


            if (Session.TextFiles.Count == 0)
            {
                var file = new TextFile("Sans titre 1");

                MainTabControl.TabPages.Add(file.SafeFileName);
                var tabPage = MainTabControl.TabPages[0];
                var rtb = new CustomRichTextBox();
                tabPage.Controls.Add(rtb);
                rtb.Select();

                Session.TextFiles.Add(file);

                CurrentFile = file;
                CurrentRtb = rtb;
            }
            else
            {
                var activeIndex = Session.ActiveIndex;

                foreach (var file in Session.TextFiles)
                {
                    if (File.Exists(file.FileName) || File.Exists(file.BackupFileName))
                    {
                        var rtb = new CustomRichTextBox();
                        var tabCount = MainTabControl.TabCount;

                        MainTabControl.TabPages.Add(file.SafeFileName);
                        MainTabControl.TabPages[tabCount].Controls.Add(rtb);

                        rtb.Text = file.Content;
                    }
                }
                CurrentFile = Session.TextFiles[activeIndex];
                CurrentRtb = (CustomRichTextBox)MainTabControl.TabPages[activeIndex].Controls.Find("RTBTextFileContent", true).First();
                CurrentRtb.Select();

                MainTabControl.SelectedIndex = activeIndex;
                Text = $"{CurrentFile.FileName} - Deskpad";
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Session.ActiveIndex = MainTabControl.SelectedIndex;
            Session.Save();

            foreach (var file in Session.TextFiles)
            {
                var fileIndex = Session.TextFiles.IndexOf(file);
                var rtb = MainTabControl.TabPages[fileIndex].Controls.Find("RTBTextFileContent", true).First();

                if (file.FileName.StartsWith("Sans titre"))
                {
                    file.Content = rtb.Text;
                    Session.BackupFile(file);
                }
            };
        }
    }
}
