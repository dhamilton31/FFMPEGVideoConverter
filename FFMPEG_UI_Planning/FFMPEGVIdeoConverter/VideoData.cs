﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FFMPEGVideoConverter
{
    /// <summary>
    /// Container class for video files in the directory,
    /// video start date and time, and the patient name
    /// </summary>
    public class VideoData
    {
        private List<string> filesInDirectory;
        private DateTime startDateTime;
        private string patientName;
        private string outputFileName;
        private string testName;

        public List<string> FilesInDirectory
        {
            get
            {
                List<string> FilesNamesOnly = new List<string>();
                foreach(string path in filesInDirectory)
                {
                    FilesNamesOnly.Add(Path.GetFileName(path));
                }
                return FilesNamesOnly;
            }

            set
            {
                filesInDirectory = value;
            }
        }

        public DateTime StartDateTime
        {
            get
            {
                return startDateTime;
            }

            set
            {
                startDateTime = value;
            }
        }

        public string PatientName
        {
            get
            {
                return patientName;
            }

            set
            {
                patientName = value;
            }
        }

        public string OutputFileName
        {
            get
            {
                return outputFileName;
            }

            set
            {
                outputFileName = value;
            }
        }

        public string TestName
        {
            get
            {
                return testName;
            }

            set
            {
                testName = value;
            }
        }

        public VideoData()
        {
            FilesInDirectory = new List<string>();
            StartDateTime = DateTime.Now;
            PatientName = "PatientName";
        }

        public VideoData(VideoData videoData)
        {
            this.FilesInDirectory = videoData.FilesInDirectory;
            this.StartDateTime = videoData.StartDateTime;
            this.PatientName = videoData.PatientName;
            this.outputFileName = videoData.outputFileName;
        }

        public string GenerateStandardOutputName()
        {
            string outputName = "outputVideo.avi";
            // Output name is in format Patient_Name_MM_DD_YYYY_Test_Name
            outputName = PatientName;
            outputName += " " + StartDateTime.ToString("MM_dd_yyyy");
            outputName += " " + TestName;
            outputName = outputName.Replace(' ', '_');
            return outputName;
        }
    }
}
