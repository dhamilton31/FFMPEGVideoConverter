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
                    AddNewDirectoryAndFilesToLists(GetDirectoryName(fbd.SelectedPath));
                }
            }
        }

        private string GetDirectoryName(string path)
        {
            return new DirectoryInfo(path).Name;
        }

        private void AddNewDirectoryAndFilesToLists(string dirName)
        {
            lBDirectories.Items.Add(dirName);
        }
    }
}
