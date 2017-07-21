using FFMPEGVIdeoConverter;
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
        private OutputTextRelayer textRelayer;
        private int ConversionStepsCompletedCount;

        public FileConversionManager()
        {
            fileConverters = new List<FileConverter>();
            textRelayer = new OutputTextRelayer();
            textRelayer.OnOutputTextReceived += TextRelayer_OnOutputTextReceived;
        }

        private void TextRelayer_OnOutputTextReceived(object sender, OutputTextEventArgs e)
        {
            WriteOutputText(e);
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
            FileConverter newFileConverter = new FileConverter(newDirPath, textRelayer);
            if(newFileConverter.AnalyzeDirectory(newDirPath))
            {
                WriteOutputText("Directory info sucessfully updated");
            }
            else
            {
                WriteOutputText("Error in adding directory. Please ensure directory exists and " +
                    "has valid video files for converting.");
            }
            retVideoData = newFileConverter.GetFilesList();
            if(retVideoData != null)
            {
                newFileConverter.OnVideoConversionStepComplete += NewFileConverter_OnVideoConversionComplete; ;
                fileConverters.Add(newFileConverter);
            }
            return retVideoData;
        }

        private void NewFileConverter_OnVideoConversionComplete(object sender, EventArgs e)
        {
            ConversionStepsCompletedCount++;
            WriteOutputText("Steps completed count: " + ConversionStepsCompletedCount);
        }

        public int GetCompletedVideoConversion()
        {
            return ConversionStepsCompletedCount;
        }

        public int GetTotalNumberOfConversionSteps()
        {
            return fileConverters.Count * FileConverter.CONVERSION_STEPS;
        }

        public bool ErrorsOccured()
        {
            foreach(FileConverter con in fileConverters)
            {
                if(con.HadErrors())
                {
                    return true;
                }
            }
            return false;
        }

        public bool BeginFileConversion()
        {
            bool bSuccess = false;
            // Reset the number of FileConverters that have 
            // completed their video conversion
            ConversionStepsCompletedCount = 0;
            foreach (FileConverter fc in fileConverters)
            {
                fc.BeginFileConversion();
            }
            return bSuccess;
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

        /// <summary>
        /// Removes the selected directory from the list
        /// </summary>
        /// <param name="dir">Directory to remove</param>
        /// <returns>True if directory is found and sucessfully removed</returns>
        public bool DeleteFileConverter(string dir)
        {
            for (int i = 0; i < fileConverters.Count; i++)
            {
                if (fileConverters[i].GetInputDirectory() == dir)
                {
                    fileConverters[i].Destroy();
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
                WriteOutputText("Patient name saved");
            }
        }

        /// <summary>
        /// Update the date for the video to be displayed on the 
        /// burn in
        /// </summary>
        /// <param name="dir">Directory holding these videos</param>
        /// <param name="newDate">New date to replace Day, Month, and Year</param>
        public void UpdateVidieoDate(string dir, DateTime newDate)
        {
            VideoData vd = GetVideoDataFromDirectory(dir);
            if (vd != null)
            {
                DateTime replacement = new DateTime(newDate.Year, newDate.Month, newDate.Day, vd.StartDateTime.Hour,
                    vd.StartDateTime.Minute, vd.StartDateTime.Second, vd.StartDateTime.Millisecond);
                vd.StartDateTime = replacement;
            }
        }

        /// <summary>
        /// Update the output video file name of the selected directory
        /// </summary>
        /// <param name="dir">Directory to update the output file for</param>
        /// <param name="newName">New name of the output video file</param>
        public void UpdateOutputVideoFileName(string dir, string newName)
        {
            VideoData vd = GetVideoDataFromDirectory(dir);
            if (vd != null && !String.IsNullOrEmpty(newName))
            {
                vd.OutputFileName = newName;
                WriteOutputText("Output name saved");
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

    // The purprose of this class is to relay output messages from other classes created
    // by the file conversion manager back to the FileConversionManager
    public class OutputTextRelayer
    {
        public event OutputTextHandler OnOutputTextReceived;

        public void RelayTextOutput(List<string> output)
        {
            if(OnOutputTextReceived != null && output != null)
            {
                OutputTextEventArgs args = new OutputTextEventArgs();
                foreach (string s in output)
                {
                    args.AddTextToOutput(s);
                }
                OnOutputTextReceived(this, args);
            }
        }
    }
}
