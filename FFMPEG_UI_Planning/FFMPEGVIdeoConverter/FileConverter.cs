using System;
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
        private string fileExt = "mp4";
        
        public FileConverter(string dirPath)
        {
            fileSorter = new FileSorter(dirPath);
        }

        /// <summary>
        /// Will read files out of directory and
        /// fill out the VideoData object with 
        /// start datetime and file info.
        /// </summary>
        /// <param name="dirPath">Path to directory, leave blank to use
        /// the directory passed in through constructor</param>
        /// <returns></returns>
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
                bSuccess = true;
            }
            return bSuccess;
        }

        private DateTime DetermineStartTime(string filePath)
        {
            DateTime starttime = DateTime.Now;
            if(File.Exists(filePath))
            {
                starttime = File.GetCreationTime(filePath);
            }
            return starttime;
        }

        public VideoData GetFilesList()
        {
            return videoData;
        }
    }
}
