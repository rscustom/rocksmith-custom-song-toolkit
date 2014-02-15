-------------------------------------------------------------
-- DDC v2.1 (c) 2013, by Chlipouni
-------------------------------------------------------------

DDC is a command line tool conceived to add Dynamic Difficulty Levels on Custom Downloadable Contents.

Installation :
--------------
Just decompress the "zip" file in a folder of your choice.

Use :
-----
- Open a Windows command-line interpreter
- Execute the "ddc.exe" tool with the following parameters :

  C:\ddc_v2.1>ddc.exe
  -----------------------------------------------------------------------
  -- D Y N A M I C   D I F F I C U L T Y   C R E A T O R   v 2.1       --
  -----------------------------------------------------------------------
  ddc.exe <arrangement> [-l <phraseLength>] [-s {Y | N}] [-m <ramp-up>] [-p {Y | N}] [-t {Y | N}]

  Parameters :
  ------------
    <arrangement>      : XML arrangement input file (mandatory)
    -l <phraseLength>  : Length of phrases in number of measures (optional; default : 2)
    -s {Y | N}         : Remove sustain for notes with length < 1/4 of measure (optional; default : N)
    -m <ramp-up>       : XML file with the specific ramp-up model to apply (optional; default : internal ramp-up model)
    -p {Y | N}         : Preserve the XML file name, so existing content is overwritten (optional; default : N)
    -t {Y | N}         : Trace the DDC process and generate log files (optional; default : Y)

  Example :
  ------------
  C:\mySongs>C:\ddc_v2.1\ddc.exe "PART REAL_GUITAR.xml" -l 4 -s Y

Result Files :
--------------
- If "-p" equals "N", the result XML file is named "DDC_<fileName>.xml" otherwise, the input XML file is overwritten
- If "-t" equals "Y", a log file is automatically created as "DDC_<fileName>.log"

Return Error Codes :
--------------------
0 : Ends normally with no error
1 : Ends with system error
2 : Ends with application error
