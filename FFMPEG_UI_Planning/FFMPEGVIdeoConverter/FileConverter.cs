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
        public event VideoConversionComplete OnVideoConversionStepComplete;
        // There are 2 main steps (concatination and timestamp burn in)
        // per video output. Each FileConverter will report in
        // when these two steps are completed
        public const int CONVERSION_STEPS = 2;

        /// <summary>
        /// FileConverter contains all the data for the 
        /// videos to be converted in a particular directory
        /// as well as interfaces with ffmpeg
        /// </summary>
        /// <param name="dirPath">Path to the directory where the 
        /// files are locateed</param>
        /// <param name="outputLogRelayer">Output logger to log messages
        /// to the UI</param>
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
                    videoData.TestName += "Baseline";
                    videoData.GenerateStandardOutputName();
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
            DateTime startTime = DateTime.MinValue;
            if(File.Exists(filePath))
            { 
                startTime = ffmpegDriver.RetrieveTimestampMetadata(filePath);
                // We are getting LastWriteTime because copy-pasting the video
                // can overwrite the creation time.
                DateTime StartDate = File.GetLastWriteTime(filePath);
                // We will resort to using the file creation date if getting the metadata from FFMPEG 
                // driver fails.
                if (startTime == DateTime.MinValue)
                {
                    SendOutputToRelayer("File start time missing or invalid - using file creation time instead");
                    startTime = StartDate;
                    double videoDuration = ffmpegDriver.GetVideoDuration(filePath);
                    // We want to subtract the duration to get the video's start time.
                    startTime.AddSeconds((-1) * videoDuration);
                }
                startTime = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, startTime.Hour,
                    startTime.Minute, startTime.Second, startTime.Millisecond);
            }
            return startTime;
        }

        /// <summary>
        /// Start converting the files into a single output
        /// video and burn in timestamp
        /// </summary>
        /// <returns>returns true on success</returns>
        public bool BeginFileConversion()
        {
            bool bSuccess = true;
            if (convertVideoThread == null)
            {
                convertVideoThread = new Thread(new ThreadStart(ConvertFiles));
                convertVideoThread.IsBackground = true;
                convertVideoThread.Start();
            }
            return bSuccess;
        }

        public void ConvertFiles()
        {
            try
            {
                bool fileListCreatedSuccessfully = ffmpegDriver.CreateListFilesToAppend(videoData.FilesInDirectory);
                if (fileListCreatedSuccessfully)
                {
                    bool AppendVideoFilesSucessful = ffmpegDriver.AppendVideoFiles();
                    // Report step 1 complete
                    VideoConversionStepComplete();
                    if (AppendVideoFilesSucessful)
                    {
                        SendOutputToRelayer("Concatenated " + videoData.FilesInDirectory.Count + " video files in directory " + fileSorter.GetDirectory());
                        ffmpegDriver.AddTimeStampOverlay(videoData.StartDateTime, videoData.OutputFileName, videoData.PatientName);
                        SendOutputToRelayer("Timestamp overlay added" + fileSorter.GetDirectory());
                        SendOutputToRelayer("****COMPLETE: " + fileSorter.GetDirectory() + "\\" + videoData.OutputFileName);
                    }
                }
                else
                {
                    SendOutputToRelayer("Creating file list for directory " + fileSorter.GetDirectory() +
                        " has failed. Abandoning process...");
                    // Report step 1 complete. We did not try because file list creation failed, step 1 is done/skipped.
                    VideoConversionStepComplete();
                }
                // Report step 2 complete.
                VideoConversionStepComplete();
                convertVideoThread = null;
            }
            catch(ThreadAbortException abort)
            {
                SendOutputToRelayer(fileSorter.GetDirectory() + " Thread terminated unexpectedly. Program may be unstable. Please restart.");
                SendOutputToRelayer("Error details: " + abort.ToString());
            }
        }

        public void Destroy()
        {
            if(convertVideoThread != null)
            {
                if(convertVideoThread.Join(1000))
                {
                    convertVideoThread.Abort();
                }
            }
            ffmpegDriver.Destroy();
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

        public void VideoConversionStepComplete()
        {
            if(OnVideoConversionStepComplete != null)
            {
                OnVideoConversionStepComplete(this, new EventArgs());
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
