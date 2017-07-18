using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FFMPEGVideoConverter
{
    public class FFMPEGDriver
    {
        private string pathToFFPROBE = @"FFMPEG\ffprobe.exe";
        private string pathToFFMPEG = @"FFMPEG\ffmpeg.exe";
        private string filesListToAppendFileName = @"Files_for_Append.txt";
        private string tempOutputFileName = @"temp_output.avi";
        private string pathToDirectory;
        private bool filesToAppendListCreated;
        private OutputTextRelayer outputLogRelayer;

        public FFMPEGDriver(string pathToDirectory, OutputTextRelayer outputLogRelayer)
        {
            this.pathToDirectory = pathToDirectory;
            filesToAppendListCreated = false;
            this.outputLogRelayer = outputLogRelayer;
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
            DeleteTempOutputFile();
            if (filesToAppendListCreated)
            {
                string dirPathToFileList = pathToDirectory + "\\" + filesListToAppendFileName;
                string dirPathToTempOutput = pathToDirectory + "\\" + tempOutputFileName;
                string commandOut = pathToFFMPEG + " -v verbose  -safe 0 -f concat -i \"" + dirPathToFileList + "\" -q 10 \"" + dirPathToTempOutput + "\"";
                // We will wait up to two minutes in case of a lot of files.
                SendOutputToRelayer(ExecuteFFMPEGCommand(commandOut, 120000));
                if(VerifyTempFileWasCreated())
                {
                    bSuccess = true;
                }
                else
                {
                    SendOutputToRelayer("ERROR: Could not concatenate video files! Abandoning video creation.");
                }
            }
            return bSuccess;
        }

        public void AddTimeStampOverlay(DateTime timestamp, string outputName)
        {
            DeleteOldOutputFile(outputName);
            string dirPathToTempOutput = pathToDirectory + "\\" + tempOutputFileName;
            string timeStamp = timestamp.Hour.ToString("D2") + "\\:" + timestamp.Minute.ToString("D2") + "\\:" +
                timestamp.Second.ToString("D2") + "\\:" + timestamp.Millisecond.ToString("D2");
            string dateStamp = timestamp.ToString("MM/dd/yyyy");
            string finalOutputFile = pathToDirectory + "\\" + outputName;
            // The fontfile part is weird. it needs single quotes (fails with double) and needs \\ rather than just \ in the 
            // interpreted string. This means FFMPEG\\\\arial.ttf. If in a future release this is fixed, try to 
            // adjust down to just FFMPEG\\arial.ttf and see if it works.
            string addTimeStampCommand = pathToFFMPEG + " -i \"" + dirPathToTempOutput + "\" -vf drawtext=\"fontsize = 15:fontfile = 'FFMPEG\\\\arial.ttf':timecode = '" + timeStamp + "':rate = 30:text = '" + dateStamp +" CCF Otis\\  ':fontsize = 44:fontcolor = 'white':boxcolor = 0x000000AA:box = 1:x = 400 - text_w / 2:y = 960\" -q 10 \"" + finalOutputFile + "\"";
            ExecuteFFMPEGCommand(addTimeStampCommand);
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
                        new System.IO.StreamWriter(pathToDirectory + "\\" + filesListToAppendFileName))
                        foreach (string fileName in files)
                        {
                            file.WriteLine("file '" + pathToDirectory + "\\" + fileName + "'");
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

        private void DeleteOldOutputFile(string outputFile)
        {
            string pathToFile = pathToDirectory + "\\" + outputFile;
            if (File.Exists(pathToFile))
            {
                File.Delete(pathToFile);
            }
        }

        private void DeleteTempOutputFile()
        {
            string pathToFile = pathToDirectory + "\\" + tempOutputFileName;
            if (File.Exists(pathToFile))
            {
                File.Delete(pathToFile);
            }
        }

        private bool VerifyTempFileWasCreated()
        {
            bool fileCreated = false;
            string pathToFile = pathToDirectory + "\\" + tempOutputFileName;
            if (File.Exists(pathToFile))
            {
                fileCreated = true;
            }
            return fileCreated;
        }

        private string ExecuteFFMPEGCommand(string command, int maxProcessWaitTimeMs = 5000)
        {
            SendOutputToRelayer("Executing command: " + command);
            command = "/C " + command;
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = command;
            startInfo.RedirectStandardOutput = true;
            //startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            process.StartInfo = startInfo;
            //process.OutputDataReceived += Process_OutputDataReceived;
            //process.ErrorDataReceived += Process_ErrorDataReceived;
            process.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit(maxProcessWaitTimeMs);
            return output;
        }

        private void Process_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            SendOutputToRelayer(e.Data);
        }

        private void Process_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            SendOutputToRelayer(e.Data);
        }

        private void SendOutputToRelayer(string output)
        {
            if (outputLogRelayer != null)
            {
                List<string> lstOutput = new List<string>();
                lstOutput.Add(output);
                outputLogRelayer.RelayTextOutput(lstOutput);
            }
        }

    }
}
