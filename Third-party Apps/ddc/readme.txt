-------------------------------------------------------------
-- DDC v1.8 (c) 2013, by Chlipouni
-------------------------------------------------------------

DDC is a command line tool conceived to add Dynamic Difficulty Levels on Custom Downloadable Contents.

Installation :
--------------
Just decompress the "zip" file in a folder of your choice.

Use :
-----
- Open a Windows command-line interpreter
- Execute the "ddc.exe" tool with the following parameters :

  C:\ddc_v1.8>ddc.exe
  -----------------------------------------------------------------------
  -- D Y N A M I C   D I F F I C U L T Y   C R E A T O R   v 1.8       --
  -----------------------------------------------------------------------
  ddc.exe <arrangement> [-l <phraseLength>] [-s <removeSustain>] [-m <ramp-up>]

  Parameters :
  ------------
    <arrangement>      : XML arrangement input file (mandatory)
    -l <phraseLength>  : Length of phrases in number of measures (optional, default : 2)
    -s <removeSustain> : Remove sustain for notes with length < 1/4 of measure (optional, default : Y)
    -m <ramp-up>       : XML file with the specific ramp-up model to apply (optional, default : internal ramp-up model)

  Example :
  ------------
  C:\mySongs>C:\ddc_v1.8\ddc.exe "PART REAL_GUITAR.xml" -l 4 -s Y

Result Files :
--------------
- The result XML file is named "DDC_<fileName>.xml"
- A log file is automatically created as "DDC_<fileName>.log"

Return Error Codes :
--------------------
0 : Ends normally with no error
1 : Ends with system error
2 : Ends with application error
