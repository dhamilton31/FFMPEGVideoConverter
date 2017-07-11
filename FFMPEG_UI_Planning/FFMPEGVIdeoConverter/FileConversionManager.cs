using System;
using System.Collections.Generic;
using System.Text;

namespace FFMPEGVideoConverter
{
    public class FileConversionManager
    {
        /// <summary>
        /// We will have one file converter object for each directory containing a list 
        /// of files we wish to convert
        /// </summary>
        private List<FileConverter> fileConverters;

        public FileConversionManager()
        {
            fileConverters = new List<FileConverter>();
        }

        public VideoData AddNewDirectory(string newDirPath)
        {
            VideoData retVideoData = null;
            // Make sure we haven't already added this directory
            foreach (FileConverter fc in fileConverters)
            {
                if (fc.GetInputDirectory() == newDirPath)
                {
                    return retVideoData;
                }
            }
            FileConverter newFileConverter = new FileConverter(newDirPath);
            newFileConverter.AnalyzeDirectory(newDirPath);
            retVideoData = newFileConverter.GetFilesList();
            if(retVideoData != null)
            {
                fileConverters.Add(newFileConverter);
            }
            return retVideoData;
        }

        /// <summary>
        /// Input the directory in which the video files are stored 
        /// and returns the video data for that directory.
        /// </summary>
        /// <param name="dir">Directory that has already been added with AddNewDirectory</param>
        /// <returns>VideoData for the specified directory</returns>
        public VideoData GetVideoDataFromDirectory(string dir)
        {
            foreach(FileConverter fc in fileConverters)
            {
                if(fc.GetInputDirectory() == dir)
                {
                    return fc.GetFilesList();
                }
            }
            return new VideoData();
        }
    }
}
