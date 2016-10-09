using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.DLCPackage.Manifest
{
    // public Dictionary<string, Dictionary<string, List<int>>> Techniques { get; set; }
    public class Technique
    {
        public string DiffLevelID { get; set; }
        public string SectionID { get; set; }
        public List<int> TechID { get; set; }
    }
}

//"Techniques" : {
//     "DiffLevelID" : {//used to display which techs are set at current lvl.
//         "SetionID" : [// > 0
//             TechID, //required base tech for extended tech(?)
//             TechID
//         ]
//     },
// }

/*
Techniques": {
      "4": {
          "0": [
            35
          ],
          "1": [
            35
          ],
          "3": [
            35
          ],
          "5": [
            35
          ],
          "7": [
            35
          ],
          "9": [
            35
          ],
          "11": [
            35
          ],
          "13": [
            35
          ],
          "20": [
            35
          ],
          "21": [
            35
          ]
        },
*/
