﻿using FFMPEGVIdeoConverter;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFMPEGVideoConverter
{
    public delegate void OutputTextHandler(object sender, OutputTextEventArgs e);

    public class FileConversionManager
    {
        public event OutputTextHandler OnOutputTextReceived;

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

        public bool DeleteFileConverter(string dir)
        {
            for (int i = 0; i < fileConverters.Count; i++)
            {
                if (fileConverters[i].GetInputDirectory() == dir)
                {
                    fileConverters.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Update the name of the patient for this
        /// group of videos
        /// </summary>
        /// <param name="dir">Directory holding these videos</param>
        /// <param name="newName">New patient name</param>
        public void UpdatePatientName(string dir, string newName)
        {
            VideoData vd = GetVideoDataFromDirectory(dir);
            if(vd != null)
            {
                vd.PatientName = newName;
                WriteOutputText("Patient name updated");
            }
        }

        public void UpdateOutputVideoFileName(string dir, string newName)
        {
            VideoData vd = GetVideoDataFromDirectory(dir);
            if (vd != null && !String.IsNullOrEmpty(newName))
            {
                vd.OutputFileName = newName;
            }
        }

        public void WriteOutputText(OutputTextEventArgs outEventArgs)
        {
            if (OnOutputTextReceived != null && outEventArgs != null)
            {
                OnOutputTextReceived(this, outEventArgs);
            }
        }

        public void WriteOutputText(string line)
        {
            OutputTextEventArgs outEventArgs = new OutputTextEventArgs();
            outEventArgs.AddTextToOutput(line);
            if (OnOutputTextReceived != null)
            {
                OnOutputTextReceived(this, outEventArgs);
            }
        }
    }
}
