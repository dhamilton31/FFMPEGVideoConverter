﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FFMPEGVideoConverter;
using System.IO;
using System.Collections.Generic;

namespace FFMPEGVideoConverterUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        string dirPath = System.IO.Path.GetFullPath(@"..\..\UnitTestDirectory");

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
            Assert.AreEqual(dirPath, fs.GetDirectory());
        }

        [TestMethod]
        public void FileConverterMp4Test()
        {
            FileConverter fc = new FileConverter(dirPath);
            Assert.IsTrue(fc.AnalyzeDirectory());
            VideoData vd = fc.GetFilesList();
            Assert.AreEqual(vd.FilesInDirectory[0], "File06.mp4");
        }

        [TestMethod]
        public void FileConverterTxtTest()
        {
            FileConverter fc = new FileConverter(dirPath);
            fc.SetNewFileExt("txt");
            Assert.IsTrue(fc.AnalyzeDirectory());
            VideoData vd = fc.GetFilesList();
            int i = 1;
            foreach (string file in vd.FilesInDirectory)
            {
                string fileName = "File" + i.ToString("D2") + ".txt";
                i++;
                Assert.AreEqual(file, fileName);
            }
            fc.SetNewFileExt("avi");
            Assert.IsTrue(fc.AnalyzeDirectory());
            vd = fc.GetFilesList();
            Assert.AreEqual(vd.FilesInDirectory[0], "File05.avi");
        }

        [TestMethod]
        public void FileConverterDirNameTest()
        {
            FileConverter fc = new FileConverter(dirPath);
            Assert.IsTrue(fc.AnalyzeDirectory());
            Assert.AreEqual(dirPath, fc.GetInputDirectory());
        }

        [TestMethod]
        public void FFMPEGDriverFileListTest()
        {
            FFMPEGDriver driver = new FFMPEGDriver(dirPath, null);
            List<string> filesList = new List<string>();
            filesList.Add("file01.mp4");
            filesList.Add("file02.mp4");
            filesList.Add("file03.mp4");
            filesList.Add("file04.mp4");
            driver.CreateListFilesToAppend(filesList);

            int counter = 0;
            string line;

            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader(dirPath + "Files_for_Append.txt");
            while ((line = file.ReadLine()) != null)
            {
                counter++;
                Assert.AreEqual(line, "file \'file" + counter.ToString("D2") +".mp4\'");
                
            }

            file.Close();
        }
    }
}
