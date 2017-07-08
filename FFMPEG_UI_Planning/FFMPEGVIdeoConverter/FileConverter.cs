using System;
using System.Collections.Generic;
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
        
        public FileConverter(string dirPath)
        {
            fileSorter = new FileSorter(dirPath);
        }

        public VideoData GetFilesList()
        {
            List<string> files = fileSorter.FindAndSort();
            VideoData retData;
            if (files.Count > 0)
            {
                videoData = new VideoData();
                videoData.FilesInDirectory = files;
            }
            retData = new VideoData(videoData);
            return retData;
        }
    }
}
