using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.DLCPackage.Manifest
{
    // public Dictionary<string, Dictionary<string, List<int>>> Chords { get; set; }
    public class Chord
    {
        public string DiffLevelID { get; set; }
        public string SectionID { get; set; }
        public List<int> ChordID { get; set; }
    }
}

//"Chords" : {
//     "DiffLevelID" : {//used to display which techs are set at current lvl.
//         "SectionID" : [// > 0
//             ChordID, //required base tech for extended tech(?)
//             ChordID
//         ]
//     },
// }


/*
"Chords": {
"4": {
 "0": [
   0
 ],
 "1": [
   0
 ],
 "3": [
   0
 ],
 "5": [
   0
 ],
 "7": [
   0
 ],
 "9": [
   0
 ],
 "11": [
   0
 ],
 "13": [
   0
 ],
 "20": [
   0
 ],
 "21": [
   0
 ]
},
*/
