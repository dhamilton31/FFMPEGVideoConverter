using FFMPEGVideoConverter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFMPEG_UI_Planning
{
    public partial class MainVideoConverterForm : Form
    {

        private FileConversionManager fileConversionManager;
        private string lastDirectoryOpenedFile;
        private string lastOpenedDirectory;
        private string btnStartConversionText = "START CONVERSION";
        private string btnInProgressConversionText = "CONVERSION IN PROGRESS...THIS MAY TAKE AWHILE";
        private Timer eventTimer;

        public MainVideoConverterForm()
        {
            InitializeComponent();
            fileConversionManager = new FileConversionManager();
            string path;
            path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            lastDirectoryOpenedFile = path + "\\lastDir.txt";
            lastOpenedDirectory = ReadLastOpenedDirectory();
            fileConversionManager.OnOutputTextReceived += FileConversionManager_OnOutputTextReceived;
        }

        private void FileConversionManager_OnOutputTextReceived(object sender, FFMPEGVIdeoConverter.OutputTextEventArgs e)
        {
            if (InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    FileConversionManager_OnOutputTextReceived(sender, e);
                });
            }
            else
            {
                List<string> output = e.ReadOutputText();
                foreach (string s in output)
                {
                    tbOutputText.Text += s + "\r\n";
                }
                tbOutputText.SelectionStart = tbOutputText.TextLength;
                tbOutputText.ScrollToCaret();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            VideoDirectory selectedVidDir = (VideoDirectory)lBDirectories.SelectedItem;
            if (selectedVidDir != null)
            {
                VideoData vd = fileConversionManager.GetVideoDataFromDirectory(selectedVidDir.FullPath);
                UpdateVideoDataDetails(vd);
            }
        }

        private void btnAddDir_Click(object sender, EventArgs e)
        {
            if (AcceptingInput())
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.Description = "Select the directory containing the video files you wish to add.";
                if (!String.IsNullOrEmpty(lastOpenedDirectory))
                {
                    fbd.SelectedPath = lastOpenedDirectory;
                }
                DialogResult result = fbd.ShowDialog();
                string folderName;
                if (result == DialogResult.OK)
                {
                    folderName = fbd.SelectedPath;
                    VideoData vd = fileConversionManager.AddNewDirectory(fbd.SelectedPath);
                    if (vd != null)
                    {
                        AddNewDirectoryAndFilesToLists(fbd.SelectedPath);
                        lastOpenedDirectory = fbd.SelectedPath;
                        SaveLastOpenedDirectory(fbd.SelectedPath);
                    }
                }
            }
        }

        private string ReadLastOpenedDirectory()
        {
            string directory = "";
            try
            {
                var fileStream = new FileStream(lastDirectoryOpenedFile, FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    directory = streamReader.ReadToEnd();
                }
            }
            catch (IOException ioEx)
            {
                Console.WriteLine("IO Exception: " + ioEx.ToString());
            }
            return directory;
        }

        private void SaveLastOpenedDirectory(string dirPath)
        {
            try
            {
                System.IO.File.WriteAllText(lastDirectoryOpenedFile, dirPath);
            }
            catch(IOException ioEx)
            {
                Console.WriteLine("IO Exception: " + ioEx.ToString());
            }
        }

        public void UpdateVideoDataDetails(VideoData vd)
        {
            tbPatientName.Text = vd.PatientName;
            lbFiles.Items.Clear();
            foreach(string fileName in vd.FilesInDirectory)
            {
                lbFiles.Items.Add(Path.GetFileName(fileName));
            }
            datePicker.Value = vd.StartDateTime;
            tbTime.Text = vd.StartDateTime.ToString("HH:mm:ss:fff");
            tbOutputFileName.Text = vd.OutputFileName;
        }

        private void AddNewDirectoryAndFilesToLists(string dirName)
        {
            if (AcceptingInput())
            {
                VideoDirectory newVidDir = new VideoDirectory(dirName);
                lBDirectories.Items.Add(newVidDir);
                // We will get an update for each major conversion step completed for each output video
                progressBar.Maximum = fileConversionManager.GetTotalNumberOfConversionSteps();
            }
        }

        private void tbPatientName_TextChanged(object sender, EventArgs e)
        {
            UpdatePatiantName();
        }

        private void UpdatePatiantName()
        {
            if (AcceptingInput())
            {
                VideoDirectory selectedVidDir = (VideoDirectory)lBDirectories.SelectedItem;
                if (selectedVidDir != null && !String.IsNullOrEmpty(tbPatientName.Text))
                {
                    fileConversionManager.UpdatePatientName(selectedVidDir.FullPath, tbPatientName.Text);
                }
            }
        }

        private void btnRemoveDir_Click(object sender, EventArgs e)
        {
            if (AcceptingInput())
            {
                VideoDirectory selectedVidDir = (VideoDirectory)lBDirectories.SelectedItem;
                if (selectedVidDir != null)
                {
                    if (fileConversionManager.DeleteFileConverter(selectedVidDir.FullPath))
                    {
                        lBDirectories.Items.Remove(selectedVidDir);
                        progressBar.Maximum = fileConversionManager.GetTotalNumberOfConversionSteps();
                    }
                }
            }
        }

        private void tbTime_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbOutputFileName_TextChanged(object sender, EventArgs e)
        {
            UpdateOutputFileName();
        }

        private void UpdateOutputFileName()
        {
            if (AcceptingInput())
            {
                Regex regex = new Regex(@".+\.avi");
                Match match = regex.Match(tbOutputFileName.Text);
                if (!String.IsNullOrEmpty(tbOutputFileName.Text))
                {
                    if (!match.Success)
                    {
                        tbOutputFileName.Text = tbOutputFileName.Text + ".avi";
                    }
                }
                VideoDirectory selectedVidDir = (VideoDirectory)lBDirectories.SelectedItem;
                if (selectedVidDir != null)
                {
                    fileConversionManager.UpdateOutputVideoFileName(selectedVidDir.FullPath, tbOutputFileName.Text);
                }
            }
        }

        private void StartConversion_Click(object sender, EventArgs e)
        {
            if (AcceptingInput())
            {
                btnAddDir.Enabled = false;
                btnRemoveDir.Enabled = false;
                tbOutputFileName.Enabled = false;
                tbPatientName.Enabled = false;
                fileConversionManager.BeginFileConversion();
                btnStartConversion.Text = btnInProgressConversionText;
                btnStartConversion.BackColor = Color.Salmon;
                eventTimer = new Timer();
                eventTimer.Tick += CheckConversionComplete_Tick;
                eventTimer.Interval = 500;
                eventTimer.Start();
                tbOutputText.Text += "\r\nVideo conversion started...this process may take up to several hours " +
                    "depending on the number and length of videos!!!\r\n";
            }
        }

        private bool AcceptingInput()
        {
            return btnStartConversion.Text == btnStartConversionText;
        }

        private void CheckConversionComplete_Tick(object sender, EventArgs e)
        {
            if(fileConversionManager.GetCompletedVideoConversion() >= fileConversionManager.GetTotalNumberOfConversionSteps())
            {
                btnStartConversion.Text = btnStartConversionText;
                btnStartConversion.BackColor = Color.LightGreen;
                eventTimer.Stop();
                if(fileConversionManager.ErrorsOccured())
                {
                    MessageBox.Show("WARNING!! Errors may have occurred! Please see output for details.","Video Conversion Error");
                }
                btnAddDir.Enabled = true;
                btnRemoveDir.Enabled = true;
                tbOutputFileName.Enabled = true;
                tbPatientName.Enabled = true;
            }
            progressBar.Value = fileConversionManager.GetCompletedVideoConversion();
        }

        private void datePicker_ValueChanged(object sender, EventArgs e)
        {
            if (AcceptingInput())
            {
                VideoDirectory selectedVidDir = (VideoDirectory)lBDirectories.SelectedItem;
                if (selectedVidDir != null)
                {
                    fileConversionManager.UpdateVidieoDate(selectedVidDir.FullPath, datePicker.Value);
                }
            }
        }
    }
}
