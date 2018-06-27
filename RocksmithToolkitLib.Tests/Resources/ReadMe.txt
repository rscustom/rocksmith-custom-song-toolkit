NOTE FOR DEVELOPERS
===================
The 'Resources' folder contents are globally shared and used by all toolkit 
unit tests, e.g., RocksmithToolkitLib.Tests and RocksmithToolkitGUI.Tests

Drop any CDLC file(s) into the Resources folder that you would like to use
for unit testing.  Files from different platforms may be tested together.

Change the Properties settings of each new 'Resources' file added to:
=====================================================================
Build Action = None
Copy to Output Directory = Copy always


HOW TO QUICKLY RUN ALL SOLUTION UNIT TESTS
==========================================
Before committing a revision to Github, while in Debug mode ...

Right Mouse Click on 'Solution RocksmithCustomSongCreator'
then select 'Run Unit Test' from dropdown list, or use Ctrl+U,R

Note: Unit tests should always be run from Debug mode because the
RocksmithToolkit*.Test projects are only built while in Debug mode.