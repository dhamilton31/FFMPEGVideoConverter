using System;
using System.Collections.Generic;
using System.IO;

namespace FFMPEGVideoConverter
{
    /// <summary>
    /// Resposible for sorting files in a single directory
    /// by name, then maintaining a list
    /// </summary>
    public class FileSorter
    {
        private List<string> SortedFiles;
        private string dirPath;

        /// <summary>
        /// Stores the directory path for finding 
        /// specified filetype with a call to FindAndSort
        /// </summary>
        /// <param name="dirPath">Directory path to search for files</param>
        public FileSorter(string dirPath)
        {
            this.dirPath = dirPath;
            SortedFiles = new List<string>();
        }

        /// <summary>
        /// Sorts the file in the directory by name.
        /// </summary>
        /// <param name="fileExt"> File extension for the
        /// type of file to search for and sort.</param>
        /// <returns>List of sorted file names</returns>
        public List<string> FindAndSort(string fileExt = "*")
        {
            List<string> filesInDir = new List<string>();
            string fileFilter = "*." + fileExt;
            if (Directory.Exists(dirPath))
            {
                filesInDir.AddRange(Directory.GetFiles(dirPath, fileFilter));
                filesInDir.Sort();
                SortedFiles = filesInDir;
            }
            return filesInDir;
        }

        /// <summary>
        /// Get the list of sorted files. Will be empty unless
        /// FindAndSort has been called first.
        /// </summary>
        /// <returns></returns>
        public List<string> GetSortedFileList()
        {
            return SortedFiles;
        }

        public string GetDirectory()
        {
            return dirPath;
        }
    }
}
