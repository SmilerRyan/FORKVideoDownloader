using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
using FORKVideoDownloader.Properties;

namespace FORKVideoDownloader
{
    public partial class Form1 : Form
    {
        NotifyIcon notifyIcon1 = new NotifyIcon();

        public Form1()
        {
            InitializeComponent();
        }

        private void aboutLink_Click(object sender, EventArgs e)
        {
            MessageBox.Show("FORK Video Downloader, Version " + Application.ProductVersion.ToString() + " activated forever", "About FORK Video Downloader");
        }

        private void pasteLink_Click(object sender, EventArgs e) {
            string clipboardText = Clipboard.GetText();
            if (!string.IsNullOrEmpty(clipboardText)) {
                string[] links = clipboardText.Split('\n');
                if (links.Length == 0) {
                    MessageBox.Show("No links found: \n" + clipboardText);
                } else {
                    foreach (string link in links) {
                        if (!string.IsNullOrWhiteSpace(link)) {
                            StartDownload(link);
                        }
                    }
                }

            }
        }

        private void StartDownload(string link)
        {
            DialogResult dr = MessageBox.Show(link, "Download confirmation:", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(dr !=  DialogResult.Yes) { return; }
            string outputFolder = Environment.CurrentDirectory;
            string outputPath = Path.Combine(outputFolder, "%(title)s.%(ext)s");
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "youtube-dlp.exe",
                Arguments = $"\"{link}\" -o \"{outputPath}\" --no-playlist",
                UseShellExecute = false,
            };
            Process process = new Process { StartInfo = processStartInfo };
            process.Start();
            process.WaitForExit();

            //MessageBox.Show(string.Join("\n", Assembly.GetExecutingAssembly().GetManifestResourceNames()), "Embedded Resources", MessageBoxButtons.OK, MessageBoxIcon.Information);
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FORKVideoDownloader.Resources.ding.wav")) {
                if (stream != null) {
                    using (SoundPlayer player = new SoundPlayer(stream)) {
                        player.Play();
                    }
                }
            }

            Process.Start("explorer.exe", outputFolder);
        }

        private void visitSiteLink_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/SmilerRyan/FORKVideoDownloader");
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_Resize(object sender1, EventArgs e1)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Visible = false;
                notifyIcon1.Icon = Icon;
                notifyIcon1.Text = Text;
                notifyIcon1.Visible = true;
                notifyIcon1.MouseClick += (sender2, e2) => {
                    if (e2.Button == MouseButtons.Left) {
                        Visible = true;
                        WindowState = FormWindowState.Normal;
                        notifyIcon1.Visible = false;
                    }
                };
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Icon = Properties.Resources.Icon;
        }
    }
}
