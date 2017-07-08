using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FFMPEGVideoConverter;
using System.IO;
using System.Collections.Generic;

namespace FFMPEGVideoConverterUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        string dirPath = @"C:\Users\Daniel\Documents\Work\Contract\FFMPEG_UI_Planning\FFMPEGVideoConverterUnitTests\UnitTestDirectory";
        [TestMethod]
        public void FileSorterTest()
        {
            Assert.IsTrue(Directory.Exists(dirPath));
            FileSorter fs = new FileSorter(dirPath);
            // Find and sort .txt files by name;
            List<string> lsFiles = fs.FindAndSort("txt");
            Assert.AreEqual(4, lsFiles.Count);
            for(int i = 0; i < lsFiles.Count; i++)
            {
                string fileName = "File" + (i + 1).ToString("D2") + ".txt";
                Assert.AreEqual(Path.GetFileName(lsFiles[i]), fileName);
            }
        }

        public void FileConverterTest()
        {
            FileConverter fc = new FileConverter(dirPath);
            VideoData vd = fc.GetFilesList();
            foreach (string file in vd.FilesInDirectory)
            {

            }
        }
    }
}
