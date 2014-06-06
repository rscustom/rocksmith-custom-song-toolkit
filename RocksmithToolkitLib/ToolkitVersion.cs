using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Text; 
using System.Reflection; 
namespace RocksmithToolkitLib 
{ 
    public class ToolkitVersion 
    { 
        public static string commit = "e483488a"; 
        public static string version 
        { 
            get 
            { 
                return String.Format("{0}.{1}.{2}.{3}-{4}", 
                    Assembly.GetExecutingAssembly().GetName().Version.Major, 
                    Assembly.GetExecutingAssembly().GetName().Version.Minor, 
                    Assembly.GetExecutingAssembly().GetName().Version.Build, 
                    Assembly.GetExecutingAssembly().GetName().Version.Revision, 
                    commit); 
            } 
        } 
    } 
} 
