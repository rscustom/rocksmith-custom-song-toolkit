#!/bin/bash

echo Mac package

rm -rf "./RocksmithTookitGUI/bin/RocksmithCustomSongToolkit.app/Contents/Resources/gitkeep.txt"
cp -f "./RocksmithTookitGUI/bin/Release/*" "./RocksmithTookitGUI/bin/RocksmithCustomSongToolkit.app/Contents/Resources/"
cp -f "./RocksmithTookitGUI/songcreator.icns" "./RocksmithTookitGUI/bin/RocksmithCustomSongToolkit.app/Contents/Resources/"
cp -f "./RocksmithTookitGUI/bin/Release/RocksmithToolkitGUI.exe" "./RocksmithTookitGUI/bin/RocksmithCustomSongToolkit.app/Contents/Resources/RocksmithCustomSongToolkit.exe"
rm -rf "./RocksmithTookitGUI/bin/RocksmithCustomSongToolkit.app/Contents/Resources/RocksmithToolkitGUI.exe"
tar -cvzf Mac.tar "./RocksmithTookitGUI/bin/RocksmithCustomSongToolkit.app/"

echo Done
#exit /b 0