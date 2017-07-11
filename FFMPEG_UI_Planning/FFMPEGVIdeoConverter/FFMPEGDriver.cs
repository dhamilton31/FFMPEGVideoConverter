using System;
using System.Collections.Generic;
using System.Text;

namespace FFMPEGVideoConverter
{
    public class FFMPEGDriver
    {
        private int maxProcessWaitTimeMs = 5000;

        public string RetrieveTimestampMetadata(string pathToFile)
        {
            string timestampCommand = @"ffprobe -v error -select_streams v:0 -show_entries stream_tags=timecode:format=timecode:  -of default=noprint_wrappers=1:nokey=1 -i " + pathToFile;
            string outputTime = ExecuteFFMPEGCommand(timestampCommand);
            return outputTime;
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
