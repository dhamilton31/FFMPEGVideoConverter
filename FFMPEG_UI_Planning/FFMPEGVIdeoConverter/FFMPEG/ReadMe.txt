Steps for Successful Concatenation (Demuxer/Append of Files), Conversion of Video to AVI, and Timestamp Implementation:

1. Upload all files from the day from the Camera into the proper folder 
	(for ex- all video files from Camera 1 from one specific day all upload into Camera One's folder)

2. Run the script "retrieve_timestamp_metadata".
	Open retrieve_timestamp_metadata.txt first and enter the first file recorded's file name into the end of the script
	Run the txt file after doing this 
	This will output the beginning time of the video record from the first video for use in creating a timestamp for the entire file set
	Copy the output from the command prompt into Concatenate_Files_Convert_Add_Timestamp.bat into the space labeled timecode=' input timestamp from first file of set here '
		Add \ as needed to keep the timestamp in 00\:00\:00\:00 format
	Make sure to write the date the video was recorded into the space labeled text='date here\ into the same .bat file (if you don't remember when the video was recorded,
		any of the video files should have the created date under "properties" by right clicking on the file name and looking)
		Write the date in mm/dd/yyyy format

3. Create the file list needed for concatentation while running the batch script
	After all videos for the day's batch are uploaded into the file, run the file_list_generator.bat script
	Open the txt file created from running this program (entitled Files_for_Append_Camera_x.txt)
	Add file ' ' around each file name in the txt file so that it is in the same format as the txt file ex called FileList.txt
	Remove any files generated from the list that are NOT video files you wish to append (the ffmpeg, ffplay, ffprobe applications, as well as the .bat and .txt files will
		automatically generate with this file list and should be deleted)

4. Ensure that the Camera_x_Temp.avi file is deleted before running the .bat script for the day
	This file will be created temporarily each day after running a batch of files overnight.  In order to prevent the batch script from asking if you are OK with overwriting
		the file, it is best to delete this file prior to re-running the batch script.  This will keep the program from stopping during processing.

5. Open Concatenate_Files_Convert_Add_Timestamp.bat
	Ensure that the final file output title is the desired file name (ex- Camera_1_Day_1.avi, Camera_2_day_1.avi, etc.)

6. Run Concatentate_Files_Convert_Add_Timestamp batch script
	The files in the folder for this camera will append together into one larger file, convert from .mp4 to .avi, and a timestamp accurately displaying the time recorded will
		be created.  This process may take several hours and should be started overnight to run while you are sleeping!  Setup, once familiar with this ReadMe and the 
		program, should only take 5-10 minutes, even with 20 to even 50 files!