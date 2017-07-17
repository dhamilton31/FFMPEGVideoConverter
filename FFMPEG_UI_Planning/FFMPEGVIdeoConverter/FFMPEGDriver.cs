using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FFMPEGVideoConverter
{
    public class FFMPEGDriver
    {
        private int maxProcessWaitTimeMs = 5000;
        private string pathToFFPROBE = @"FFMPEG\ffprobe.exe";
        private string pathToFFMPEG = @"FFMPEG\ffmpeg.exe";
        private string filesListToAppendFileName = @"Files_for_Append.txt";
        private string tempOutputFileName = @"temp_output.avi";
        private string pathToDirectory;
        private bool filesToAppendListCreated;

        public FFMPEGDriver(string pathToDirectory)
        {
            this.pathToDirectory = pathToDirectory;
            filesToAppendListCreated = false;
        }

        /// <summary>
        /// Attempts to use ffprobe to get the time metadata. 
        /// </summary>
        /// <param name="pathToFile"></param>
        /// <returns>the start date and time if avilable, otherwise the min value for datetime</returns>
        public DateTime RetrieveTimestampMetadata(string pathToFile)
        {
            string timestampCommand = pathToFFPROBE + " -v error -select_streams v:0 -show_entries stream_tags=timecode:format=timecode:  -of default=noprint_wrappers=1:nokey=1 -i \"" + pathToFile + "\"";
            string outputTime = ExecuteFFMPEGCommand(timestampCommand);
            return ConvertTimeStampToDateTime(outputTime);
        }


        /// <summary>
        /// Runs FFMPEG command to append files in the video list together.
        /// </summary>
        /// <returns>True if sucessful</returns>
        public bool AppendVideoFiles()
        {
            bool bSuccess = false;
            if(filesToAppendListCreated)
            {
                string dirPathToFileList = pathToDirectory + filesListToAppendFileName;
                string dirPathToTempOutput = pathToDirectory + filesListToAppendFileName;
                string commandOut = pathToFFMPEG + " - f concat - i " + dirPathToFileList + " - q 10 " + dirPathToTempOutput;
                ExecuteFFMPEGCommand(commandOut);
            }
            return bSuccess;
        }

        /// <summary>
        /// Will create a temporary text file for files 
        /// to append together.
        /// </summary>
        /// <param name="files">List of file names to be appended</param>
        /// <returns>True on sucessful creation of text file</returns>
        public bool CreateListFilesToAppend(List<string> files)
        {
            bool bSuccess = false;
            if (Directory.Exists(pathToDirectory))
            {
                try
                {
                    using (System.IO.StreamWriter file =
                        new System.IO.StreamWriter(pathToDirectory + filesListToAppendFileName))
                        foreach (string fileName in files)
                        {
                            file.WriteLine("file \'" + fileName + "\'");
                        }
                    filesToAppendListCreated = true;
                    bSuccess = true;
                }
                catch(IOException ioEx)
                { }
            }
            return bSuccess;
        }

        private DateTime ConvertTimeStampToDateTime(string timeString)
        {
            string pattern = @"[0-9]+:[0-9]+:[0-9]+:[0-9]+";
            DateTime dt = DateTime.MinValue;
            if (!string.IsNullOrEmpty(timeString))
            {
                Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                MatchCollection matches = rgx.Matches(timeString);
                if (matches.Count > 0)
                {
                    Match match = matches[0];
                    dt = DateTime.ParseExact(match.Value, "HH:mm:ss:ff", null);
                }
            }
            return dt;
        }

        public string ConcatenateFilesConvertAddTimestamp(DateTime time)
        {
            return "";
        }

        private string ExecuteFFMPEGCommand(string command)
        {
            command = "/C " + command;
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = command;
            process.StartInfo = startInfo;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            process.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit(maxProcessWaitTimeMs);
            return output;
        }

    }
}
