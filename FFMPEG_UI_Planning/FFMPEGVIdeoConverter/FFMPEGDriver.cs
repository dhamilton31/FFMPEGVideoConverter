using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FFMPEGVideoConverter
{
    public class FFMPEGDriver
    {
        private int maxProcessWaitTimeMs = 5000;
        private string pathToFFPROBE = @"C:\Users\dh185148\Documents\FFMPEGVideoConverter\FFMPEG_UI_Planning\ffmpeg\ffprobe.exe";

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
                    dt = DateTime.ParseExact(timeString, "HH:mm:ss:ff", null);
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
