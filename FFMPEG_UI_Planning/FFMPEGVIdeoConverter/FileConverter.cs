using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace FFMPEGVideoConverter
{
    public delegate void VideoConversionComplete(object sender, EventArgs e);

    /// <summary>
    /// Contains the list of files and info needed to 
    /// create the final converted video output.
    /// </summary>
    public class FileConverter
    {
        private VideoData videoData;
        private FileSorter fileSorter;
        private FFMPEGDriver ffmpegDriver;
        private string fileExt = "mp4";
        private OutputTextRelayer outputLogRelayer;
        private Thread convertVideoThread;
        public event VideoConversionComplete OnVideoConversionComplete;

        public FileConverter(string dirPath, OutputTextRelayer outputLogRelayer = null)
        {
            fileSorter = new FileSorter(dirPath);
            ffmpegDriver = new FFMPEGDriver(dirPath, outputLogRelayer);
            this.outputLogRelayer = outputLogRelayer;
            convertVideoThread = null;
        }

        /// <summary>
        /// Will read files out of directory and
        /// fill out the VideoData object with 
        /// start datetime and file info.
        /// </summary>
        /// <param name="dirPath">Path to directory, leave blank to use
        /// the directory passed in through constructor</param>
        /// <returns>True if files were found with the extension</returns>
        public bool AnalyzeDirectory(string dirPath = "")
        {
            bool bSuccess = false;
            if (convertVideoThread == null)
            {
                if (!String.IsNullOrEmpty(dirPath))
                {
                    fileSorter = new FileSorter(dirPath);
                }
                else
                {
                    SendOutputToRelayer("Directory path error has occurred");
                }

                List<string> files = fileSorter.FindAndSort(fileExt);
                if (files.Count > 0)
                {
                    videoData = new VideoData();
                    videoData.FilesInDirectory = files;
                    // Since the files are sorted, we should be able
                    // to get the date from just the first file.
                    videoData.StartDateTime = DetermineStartTime(files[0]);
                    videoData.OutputFileName = "outputVideo.avi";
                    SendOutputToRelayer("Directory " + dirPath + " and files added.");
                    bSuccess = true;
                }
                else
                {
                    SendOutputToRelayer("No files were found in directory " + dirPath);
                }
            }
            else
            {
                SendOutputToRelayer("Can not analyze directory while running conversion");
            }
            return bSuccess;
        }

        private DateTime DetermineStartTime(string filePath)
        {
            DateTime starttime = DateTime.MinValue;
            if(File.Exists(filePath))
            { 
                starttime = ffmpegDriver.RetrieveTimestampMetadata(filePath);
                // We will resort to using the file creation date if getting the metadata from FFMPEG 
                // driver fails.
                if(starttime == DateTime.MinValue)
                {
                    SendOutputToRelayer("File start time missing or invalid - using file creation time instead");
                    starttime = File.GetCreationTime(filePath);
                }
            }
            return starttime;
        }

        public bool BeginFileConversion()
        {
            bool bSuccess = true;
            if (convertVideoThread == null)
            {
                convertVideoThread = new Thread(new ThreadStart(ConvertFiles));
                convertVideoThread.Start();
            }
            return bSuccess;
        }

        public void ConvertFiles()
        {
            if(ffmpegDriver.CreateListFilesToAppend(videoData.FilesInDirectory))
            {
                if(ffmpegDriver.AppendVideoFiles())
                {
                    SendOutputToRelayer("Concatenated " + videoData.FilesInDirectory.Count + " video files in directory " + fileSorter.GetDirectory());
                    ffmpegDriver.AddTimeStampOverlay(videoData.StartDateTime, videoData.OutputFileName, videoData.PatientName);
                    SendOutputToRelayer("Timestamp overlay added" + fileSorter.GetDirectory());
                    SendOutputToRelayer("****COMPLETE: " + fileSorter.GetDirectory() + "\\" + videoData.OutputFileName);
                }
            }
            convertVideoThread = null;
            VideoConversionComplete();
        }

        public VideoData GetFilesList()
        {
            return videoData;
        }

        public string GetInputDirectory()
        {
            return fileSorter.GetDirectory();
        }

        public void SetNewFileExt(string newExt)
        {
            this.fileExt = newExt;
        }

        public bool HadErrors()
        {
            return ffmpegDriver.HadErrors();
        }

        public void VideoConversionComplete()
        {
            if(OnVideoConversionComplete != null)
            {
                OnVideoConversionComplete(this, new EventArgs());
            }
        }

        private void SendOutputToRelayer(string output)
        {
            if(outputLogRelayer != null)
            {
                List<string> lstOutput = new List<string>();
                lstOutput.Add((new DirectoryInfo(fileSorter.GetDirectory()).Name) + ": " + output);
                outputLogRelayer.RelayTextOutput(lstOutput);
            }
        }
    }
}
