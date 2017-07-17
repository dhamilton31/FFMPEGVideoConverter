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
        private string lastDirectoryOpenedFile = "lastDir.txt";
        private string lastOpenedDirectory;

        public MainVideoConverterForm()
        {
            InitializeComponent();
            fileConversionManager = new FileConversionManager();
            lastOpenedDirectory = ReadLastOpenedDirectory();
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
                if(vd != null)
                {
                    AddNewDirectoryAndFilesToLists(fbd.SelectedPath);
                    lastOpenedDirectory = fbd.SelectedPath;
                    SaveLastOpenedDirectory(fbd.SelectedPath);
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
            tbTime.Text = vd.StartDateTime.ToString("HH:MM:ss:fff");
            tbOutputFileName.Text = vd.OutputFileName;
        }

        private void AddNewDirectoryAndFilesToLists(string dirName)
        {
            VideoDirectory newVidDir = new VideoDirectory(dirName);
            lBDirectories.Items.Add(newVidDir);
        }

        private void tbPatientName_TextChanged(object sender, EventArgs e)
        {
            VideoDirectory selectedVidDir = (VideoDirectory)lBDirectories.SelectedItem;
            if (selectedVidDir != null && !String.IsNullOrEmpty(tbPatientName.Text))
            {
                fileConversionManager.UpdatePatientName(selectedVidDir.FullPath, tbPatientName.Text);
            }
        }

        private void btnRemoveDir_Click(object sender, EventArgs e)
        {
            VideoDirectory selectedVidDir = (VideoDirectory)lBDirectories.SelectedItem;
            if (selectedVidDir != null)
            {
                if(fileConversionManager.DeleteFileConverter(selectedVidDir.FullPath))
                {
                    lBDirectories.Items.Remove(selectedVidDir);
                }
            }
        }

        private void tbTime_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbOutputFileName_TextChanged(object sender, EventArgs e)
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

        private void StartStopConversion_Click(object sender, EventArgs e)
        {

        }

        private bool ValidateReadyForConversion()
        {
            bool readyForConversion = false;
            return readyForConversion;
        }
    }
}
