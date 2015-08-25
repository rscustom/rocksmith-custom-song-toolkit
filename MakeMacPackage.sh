#!/bin/bash

echo Mac package

rm -rf "./RocksmithToolkitGUI/bin/RocksmithCustomSongToolkit.app/Contents/Resources/gitkeep.txt"
cp -f "./RocksmithToolkitGUI/bin/Release/*" "./RocksmithToolkitGUI/bin/RocksmithCustomSongToolkit.app/Contents/Resources/"
cp -f "./RocksmithToolkitGUI/songcreator.icns" "./RocksmithToolkitGUI/bin/RocksmithCustomSongToolkit.app/Contents/Resources/"
cp -f "./RocksmithToolkitGUI/bin/Release/RocksmithToolkitGUI.exe" "./RocksmithToolkitGUI/bin/RocksmithCustomSongToolkit.app/Contents/Resources/RocksmithCustomSongToolkit.exe"
rm -rf "./RocksmithToolkitGUI/bin/RocksmithCustomSongToolkit.app/Contents/Resources/RocksmithToolkitGUI.exe"
tar -cvzf Mac.tar "./RocksmithToolkitGUI/bin/RocksmithCustomSongToolkit.app/"

echo Done
#exit /b 0
