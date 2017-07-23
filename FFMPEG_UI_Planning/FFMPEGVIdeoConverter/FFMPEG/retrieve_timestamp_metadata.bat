REM # ensure the first file name is present at the end of this script for proper timestamp retrieval of the final file output
ffprobe -v error -select_streams v:0 -show_entries stream_tags=timecode:format=timecode:  -of default=noprint_wrappers=1:nokey=1 -i IMG_8439.mp4

"C:\\Users\Daniel\Pictures\Camera Roll\Camera One\ffprobe.exe -v error -select_streams v:0 -show_entries stream_tags=timecode:format=timecode:  -of default=noprint_wrappers=1:nokey=1 -i C:\\Users\Daniel\Pictures\Camera Roll\Camera One\.OUTPUT.mp4"
