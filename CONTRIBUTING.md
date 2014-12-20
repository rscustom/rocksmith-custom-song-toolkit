# How to contribute

## Getting Started

* Make sure you have a [GitHub account](https://github.com/signup/free)
* Create an [issue](https://github.com/rscustom/rocksmith-custom-song-toolkit/issues) 
  for the bug/feature if doesn't already exist
* Fork the rocksmith-custom-song-toolkit repository to your GitHub account
* Clone your fork to a local repository on your computer ... save it to ... 
  \My Documents\Visual Studio 2010\Projects\rocksmith-custom-song-toolkit ... 
  or anywhere else that you prefer.
* Run FixToolkitVersion.bat or open the Visual Studio 
  2010\Projects\rocksmith-custom-song-toolkit\RocksmithToolkitLib folder then 
  make a copy of the ToolkitVersion.cs_dist file and rename it to ToolkitVersion.cs 
* Open your local repository folder and double click on "RocksmithCustomSongCreator.sln"
* Wait for the repository to load in VS2010 Solution Explorer.
* Click on the RocksmithToolkitLib icon then right click and select "Build".
* Click on the Solution 'RocksmithCustomSongCreator' (12 projects) icon at the top 
  of the Solution Explorer list.
* Right click the icon, select "Properties", change Single startup project to 
  RocksmithToolkitGUI, click "Apply".
* Click on "Debug" in the toolbar at the top of VS2010 IDE and select 
  "Start Debugging" or just press "F5".
* The RocksmithCustomSongCreator.sln should now build in VS2010. Have fun.

## Submitting Changes

* For minor changes (single line fixes, typos etc) an issue isn't needed. Only create an issue for more substantial changes.

* Push your changes to your fork of the repository.
  * Include the issue number somewhere in the first line of the commit message
  * While the issue should have details about what you're doing, make your commit message meaningful rather than something like "Fixes #1234"
  * A good commit message might be something like `Adds support for Drop D tuning (#1234)`
* Submit a [pull request](https://help.github.com/articles/using-pull-requests) to the rscustom repository.

## Code Style

* Use spaces for indentation, and Windows line endings, enable unprintable symbols to make sure that you're doing fine.