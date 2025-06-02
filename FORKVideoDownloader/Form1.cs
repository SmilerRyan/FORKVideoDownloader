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

            // Read values from ComboBox and TextBox
            string qualitySelection = qualityComboBox.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(qualitySelection)) // Default if nothing selected or item is weird
            {
                // Attempt to read Text property if SelectedItem is null (e.g. user typed value not in list)
                qualitySelection = qualityComboBox.Text;
                if (string.IsNullOrWhiteSpace(qualitySelection)) // Final fallback
                {
                    qualitySelection = "Best Video + Best Audio";
                }
            }

            string formatArgumentString = "";
            switch (qualitySelection)
            {
                case "Best Video + Best Audio":
                    formatArgumentString = "-f \"bestvideo+bestaudio/best\"";
                    break;
                case "Best Video":
                    formatArgumentString = "-f \"bestvideo\"";
                    break;
                case "Best Audio":
                    formatArgumentString = "-f \"bestaudio\" -x --audio-format mp3";
                    break;
                case "1080p":
                    formatArgumentString = "-f \"bestvideo[height<=1080]+bestaudio/best[height<=1080]\"";
                    break;
                case "720p":
                    formatArgumentString = "-f \"bestvideo[height<=720]+bestaudio/best[height<=720]\"";
                    break;
                default:
                    // If the text in comboBox is something not recognized, try to use it directly as a format string.
                    // This allows users to type custom formats if they wish.
                    // However, to prevent errors, we should validate it or have a safer default.
                    // For now, let's default to best if it's not one of the predefined ones.
                    // A more advanced approach might be to check if qualitySelection contains spaces or special chars
                    // and if not, assume it's a direct format string like "140" or "22".
                    // But the prompt implies a default for unrecognized selections.
                    formatArgumentString = "-f \"bestvideo+bestaudio/best\"";
                    break;
            }

            string otherOptionsString = otherOptionsTextBox.Text.Trim();

            // Construct the final arguments string
            List<string> argParts = new List<string>();
            argParts.Add($"\"{link}\""); // URL

            if (!string.IsNullOrEmpty(formatArgumentString))
            {
                argParts.Add(formatArgumentString);
            }
            if (!string.IsNullOrEmpty(otherOptionsString))
            {
                argParts.Add(otherOptionsString);
            }

            argParts.Add($"-o \"{outputPath}\""); // Output template
            argParts.Add("--no-playlist");       // Other fixed options

            string finalArguments = string.Join(" ", argParts);

            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "youtube-dlp.exe",
                Arguments = finalArguments,
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
