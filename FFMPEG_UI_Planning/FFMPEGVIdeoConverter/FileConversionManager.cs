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
            FileConverter newFileConverter = new FileConverter(newDirPath);
            VideoData retVideoData = newFileConverter.GetFilesList();
            if(retVideoData != null)
            {
                fileConverters.Add(newFileConverter);
            }
            return retVideoData;
        }
    }
}
