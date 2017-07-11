using FFMPEGVideoConverter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFMPEG_UI_Planning
{
    public partial class MainVideoConverterForm : Form
    {

        private FileConversionManager fileConversionManager;

        public MainVideoConverterForm()
        {
            InitializeComponent();
            fileConversionManager = new FileConversionManager();
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
            DialogResult result = fbd.ShowDialog();
            string folderName;
            if (result == DialogResult.OK)
            {
                folderName = fbd.SelectedPath;
                VideoData vd = fileConversionManager.AddNewDirectory(fbd.SelectedPath);
                if(vd != null)
                {
                    AddNewDirectoryAndFilesToLists(fbd.SelectedPath);
                }
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
            timePicker.Value = vd.StartDateTime;
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
    }
}
