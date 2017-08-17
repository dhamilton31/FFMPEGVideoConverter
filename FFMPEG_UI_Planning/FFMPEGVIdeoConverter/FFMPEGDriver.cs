using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FFMPEGVideoConverter
{
    /// <summary>
    /// Acts as the low level interface between the ffmpeg and ffprobe exe's and the FileConverter.
    /// </summary>
    public class FFMPEGDriver
    {
        private string pathToFFPROBE = "FFMPEG\\ffprobe.exe";
        private string pathToFFMPEG = "FFMPEG\\ffmpeg.exe";
        private string filesListToAppendFileName = @"Files_for_Append.txt";
        private string tempOutputFileName = @"temp_output.avi";
        private string pathToDirectory;
        private System.Diagnostics.Process process;
        private bool filesToAppendListCreated;
        private OutputTextRelayer outputLogRelayer;
        private string errorLog = "ErrorLog.log";
        StreamWriter errorLogFile;
        bool hadError = false;
        private bool hadConcatErrors = false;
        private bool hadTimestampErrors = false;
        // Change logging from error to verbose, debug, ect in order to 
        // get more details in output logs
        private string loggingLevel = "error"; 

        public FFMPEGDriver(string pathToDirectory, OutputTextRelayer outputLogRelayer)
        {
            this.pathToDirectory = pathToDirectory;
            filesToAppendListCreated = false;
            this.outputLogRelayer = outputLogRelayer;
            errorLogFile = new StreamWriter(pathToDirectory + "\\" + errorLog);
            string pathToExe = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            pathToFFPROBE = "\"" + pathToExe + "\\" + pathToFFPROBE + "\""; //+ "\""
            SendOutputToRelayer("Actual path to FFPROBE: " + pathToFFPROBE);
            pathToFFMPEG = "\"" + pathToExe + "\\" + pathToFFMPEG + "\"";
            SendOutputToRelayer("Actual path to FFPROBE: " + pathToFFPROBE);
        }

        /// <summary>
        /// Attempts to use ffprobe to get the time metadata. 
        /// </summary>
        /// <param name="pathToFile"></param>
        /// <returns>the start date and time if avilable, otherwise the min value for datetime</returns>
        public DateTime RetrieveTimestampMetadata(string pathToFile)
        {
            string timestampCommand = " -v error -select_streams v:0 -show_entries stream_tags=timecode:format=timecode:  -of default=noprint_wrappers=1:nokey=1 -i \"" + pathToFile + "\"";
            string outputTime = ExecuteFFMPEGCommand(pathToFFPROBE, timestampCommand);
            return ConvertTimeStampToDateTime(outputTime);
        }

        /// <summary>
        /// Uses ffprobe to get the length of the video
        /// </summary>
        /// <param name="pathToFile">File path to video</param>
        /// <returns></returns>
        public double GetVideoDuration(string pathToFile)
        {
            string durationCommand = " -v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"" + pathToFile + "\"";
            string outputTime = ExecuteFFMPEGCommand(pathToFFPROBE, durationCommand);
            double output = 0;
            if(double.TryParse(outputTime, out output))
            {
                return output;
            }
            return 0;
        }


        /// <summary>
        /// Runs FFMPEG command to append files in the video list together.
        /// </summary>
        /// <returns>True if sucessful</returns>
        public bool AppendVideoFiles()
        {
            bool bSuccess = false;
            hadConcatErrors = false;
            hadError = false;
            DeleteTempOutputFile();
            if (filesToAppendListCreated)
            {
                string dirPathToFileList = pathToDirectory + "\\" + filesListToAppendFileName;
                string dirPathToTempOutput = pathToDirectory + "\\" + tempOutputFileName;
                string commandOut = " -v " + loggingLevel +" -safe 0 -f concat -i \"" + dirPathToFileList + "\" -q 10 \"" + dirPathToTempOutput + "\"";
                // We will wait up to 3 hoursin case of a lot of files.
                SendOutputToRelayer(ExecuteFFMPEGCommand(pathToFFMPEG, commandOut, true));
                if(VerifyTempFileWasCreated())
                {
                    bSuccess = true;
                }
                else
                {
                    SendOutputToRelayer("ERROR: Could not concatenate video files! Abandoning video creation.");
                }
            }
            if(hadError)
            {
                hadConcatErrors = true;
                List<string> errorOutput = new List<string>();
                errorOutput.Add("!!!!!!!!!!!!!!!!");
                errorOutput.Add("ERROR - video concatination output may not be complete!!!");
                errorOutput.Add("please check " + pathToDirectory + "\\" + errorLog + " for more details!!");
                errorOutput.Add("!!!!!!!!!!!!!!!!");
                SendOutputToRelayer(errorOutput);
            }
            try
            {
                errorLogFile.Flush();
            }
            catch(ObjectDisposedException ioEx)
            {
                SendOutputToRelayer("Couldn't write output due to error: " + ioEx.ToString());
            }
            return bSuccess;
        }

        /// <summary>
        /// Adds overlay burn in on top of video using FFMPEG.exe
        /// </summary>
        /// <param name="timestamp">Starting date and time of the video</param>
        /// <param name="outputName">Name of the file output with timestamp overlay</param>
        /// <param name="patientName">Name of the patient to be added to the overlay</param>
        public void AddTimeStampOverlay(DateTime timestamp, string outputName, string patientName)
        {
            hadError = false;
            hadTimestampErrors = false;
            DeleteOldOutputFile(outputName);
            string dirPathToTempOutput = pathToDirectory + "\\" + tempOutputFileName;
            string timeStamp = timestamp.Hour.ToString("D2") + "\\:" + timestamp.Minute.ToString("D2") + "\\:" +
                timestamp.Second.ToString("D2") + "\\:" + timestamp.Millisecond.ToString("D2");
            string dateStamp = timestamp.ToString("MM/dd/yyyy");
            string finalOutputFile = pathToDirectory + "\\" + outputName;
            // The fontfile part is weird. it needs single quotes (fails with double) and needs \\ rather than just \ in the 
            // interpreted string. This means FFMPEG\\\\arial.ttf. If in a future release this is fixed, try to 
            // adjust down to just FFMPEG\\arial.ttf and see if it works.
            string dirPathToFontFile = CreateFontPath("FFMPEG\\arial.ttf");
            string addTimeStampCommand = " -i \"" + dirPathToTempOutput + "\" -v " + loggingLevel + " -vf drawtext=\"fontsize = 15:fontfile = '"+ dirPathToFontFile + "':timecode = '" + timeStamp + "':rate = 30:text = '" + dateStamp +" " + patientName + "\\  ':fontsize = 44:fontcolor = 'white':boxcolor = 0x000000AA:box = 1:x = 400 - text_w / 2:y = 960\" -q 10 \"" + finalOutputFile + "\"";
            string output = ExecuteFFMPEGCommand(pathToFFMPEG, addTimeStampCommand, true);
            DeleteTempOutputFile();
            DeleteOldOutputFile(filesListToAppendFileName);
            if (hadError)
            {
                hadTimestampErrors = true;
                List<string> errorOutput = new List<string>();
                errorOutput.Add("!!!!!!!!!!!!!!!!");
                errorOutput.Add("ERROR with adding timestamp! Video output may not be complete!!!");
                errorOutput.Add("please check " + pathToDirectory + "\\" + errorLog + " for more details!!");
                errorOutput.Add("!!!!!!!!!!!!!!!!");
                SendOutputToRelayer(errorOutput);
            }
            try
            {
                errorLogFile.Flush();
            }
            catch (ObjectDisposedException ioEx)
            {
                SendOutputToRelayer("Couldn't write output due to error: " + ioEx.ToString());
            }
        }

        /// <summary>
        /// this funciton is created to add in the extra \ needed in the path to the font file.
        /// </summary>
        /// <param name="pathFromExecutingDirectory"></param>
        /// <returns></returns>
        private string CreateFontPath(string pathFromExecutingDirectory)
        {
            string pathToFont = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            pathToFont = pathToFont + "\\" + pathFromExecutingDirectory;
            // Add in extra \ so FFMPEG will be happy
            pathToFont = pathToFont.Replace("\\", "\\\\");
            return pathToFont;
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
                    using (StreamWriter file =
                        new StreamWriter(pathToDirectory + "\\" + filesListToAppendFileName))
                        foreach (string fileName in files)
                        {
                            file.WriteLine("file '" + pathToDirectory + "\\" + fileName + "'");
                        }
                    filesToAppendListCreated = true;
                    bSuccess = true;
                }
                catch(IOException ioEx)
                {
                    SendOutputToRelayer("Error creating files list: " + ioEx.ToString());
                }
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

        private string ExecuteFFMPEGCommand(string programPath, string command, bool logOutput = false)
        {
            string output = "";
            try {
                SendOutputToRelayer("Executing command: " + programPath + " with arguments: " + command);
                //command = "/C " + "\"" + command + "\"";
                process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = programPath;
                startInfo.Arguments = command;
                startInfo.RedirectStandardOutput = true;
                if (logOutput)
                {
                    startInfo.RedirectStandardError = true;
                }
                startInfo.CreateNoWindow = true;
                
                if (logOutput)
                {
                    process.OutputDataReceived += (sender, args) => SendOutputToRelayer(args.Data);
                    process.ErrorDataReceived += (sender, args) => SendOutputToLog(args.Data);
                }
                startInfo.UseShellExecute = false;
                process.StartInfo = startInfo;
                //process.OutputDataReceived += Process_OutputDataReceived;
                //process.ErrorDataReceived += Process_OutputDataReceived;
                process.Start();
                if (logOutput)
                {
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                }
                // Do not wait for the child process to exit before
                // reading to the end of its redirected stream.
                // p.WaitForExit();
                // Read the output stream first and then wait.
                else
                {
                    output = process.StandardOutput.ReadToEnd();
                }
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                // catchall just in case
                output = "An unexpected excetpion occurred: " + ex.ToString();
                SendOutputToLog(output);
            }
            return output;
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

        private void SendOutputToLog(string output)
        {
            if (!String.IsNullOrEmpty(output))
            {
                errorLogFile.WriteLine(output);
                hadError = true;
            }
        }

        public bool HadErrors()
        {
            return hadTimestampErrors || hadConcatErrors;
        }

        /// <summary>
        /// Attempts to kill any remaining running FFMPEG processes.
        /// </summary>
        public void Destroy()
        {
            if (process != null && !process.HasExited)
            {
                process.Kill();
            }
            errorLogFile.Close();
        }

        private void SendOutputToRelayer(List<string> output)
        {
            if (outputLogRelayer != null && output != null && output.Count > 0)
            {
                outputLogRelayer.RelayTextOutput(output);
            }
        }

        private void SendOutputToRelayer(string output)
        {
            try
            {
                if (outputLogRelayer != null)
                {
                    List<string> lstOutput = new List<string>();
                    lstOutput.Add(output);
                    outputLogRelayer.RelayTextOutput(lstOutput);
                }
            }
            catch (Exception ex)
            {
                // We dont' want to get here!
            }
        }

        private int HoursToMs(double hours)
        {
            return (int)TimeSpan.FromHours(hours).TotalMilliseconds;
        }
    }
}
