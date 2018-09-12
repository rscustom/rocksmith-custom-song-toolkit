RD /Q /S "Template\.cache"
RD /Q /S "Template\GeneratedSoundBanks"
DEL /Q ""Template\Template.Administrator.validationcache"

Pause "Deleted .cache and GenerateSoundBanks directories ..."

"D:\Program Files\Audiokinetic\Wwise v2010.3.3 build 3773\Authoring\Win32\Release\bin\WwiseCLI.exe" "Template\Template.wproj" -GenerateSoundBanks -Platform Windows -Language English(US) -NoWwiseDat -ClearAudioFileCache -Save -Verbose

Pause "Done"