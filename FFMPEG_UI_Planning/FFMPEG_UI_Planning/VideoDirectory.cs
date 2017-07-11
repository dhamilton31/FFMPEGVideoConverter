using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMPEG_UI_Planning
{
    public class VideoDirectory
    {
        private string directoryName;
        private string fullPath;

        public VideoDirectory(string fullPath)
        {
            this.fullPath = fullPath;
            directoryName = GetDirectoryName(fullPath);
        }

        private string GetDirectoryName(string path)
        {
            return new DirectoryInfo(path).Name;
        }

        public string DirectoryName
        {
            get
            {
                return directoryName;
            }

            set
            {
                directoryName = value;
            }
        }

        public string FullPath
        {
            get
            {
                return fullPath;
            }

            set
            {
                fullPath = value;
            }
        }

        public override string ToString()
        {
            return directoryName;
        }
    }
}
