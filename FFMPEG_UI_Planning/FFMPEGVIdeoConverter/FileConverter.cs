﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FFMPEGVideoConverter
{
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
        private OutputTextRelayer fileRelayer;



        public FileConverter(string dirPath, OutputTextRelayer fileRelayer = null)
        {
            fileSorter = new FileSorter(dirPath);
            ffmpegDriver = new FFMPEGDriver();
            this.fileRelayer = fileRelayer;
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
            if(!String.IsNullOrEmpty(dirPath))
            {
                fileSorter = new FileSorter(dirPath);
            }

            List<string> files = fileSorter.FindAndSort(fileExt);
            if (files.Count > 0)
            {
                videoData = new VideoData();
                videoData.FilesInDirectory = files;
                // Since the files are sorted, we should be able
                // to get the date from just the first file.
                videoData.StartDateTime = DetermineStartTime(files[0]);
                videoData.OutputFileName = "outputVideo";
                bSuccess = true;
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
                    starttime = File.GetCreationTime(filePath);
                }
            }
            return starttime;
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

        private void SendOutputToRelayer(string output)
        {
            if(fileRelayer != null)
            {
                List<string> lstOutput = new List<string>();
                lstOutput.Add(output);
                fileRelayer.RelayTextOutput(lstOutput);
            }
        }
    }
}
