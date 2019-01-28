using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace CreateToolkitShortcut
{
    public static class SelfDestruct
    {
        /// <summary>
        /// WARNING - The application will self destruct after this method is called ...
        /// <para>WARNING - Appliction executable is permenatly deleted.  This is not a joke!!!</para>
        /// </summary>
        public static void DoIt()
        {
            Process procDestruct = new Process();
            string strName = "destruct.bat";
            string strPath = Path.Combine(Directory.GetCurrentDirectory(), strName);
            string strExe = new FileInfo(Application.ExecutablePath).Name;

            StreamWriter swDestruct = new StreamWriter(strPath);
            swDestruct.WriteLine("attrib \"" + strExe + "\"" + " -a -s -r -h");
            swDestruct.WriteLine(":Repeat");
            swDestruct.WriteLine("del " + "\"" + strExe + "\"");
            swDestruct.WriteLine("if exist \"" + strExe + "\"" + " goto Repeat");
            swDestruct.WriteLine("del \"" + strName + "\"");
            swDestruct.Close();

            procDestruct.StartInfo.FileName = "destruct.bat";
            procDestruct.StartInfo.CreateNoWindow = true;
            procDestruct.StartInfo.UseShellExecute = false;

            try
            {
                procDestruct.Start();
            }
            catch (Exception)
            {
                Environment.Exit(1);
            }
        }


    }
}
