using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.DLCPackage.Manifest
{
    public class ChordTemplate
    {
        public int ChordId { get; set; }
        public string ChordName { get; set; }
        public List<int> Fingers { get; set; }
        public List<int> Frets { get; set; }
    }
}

/*
  "ChordTemplates": [
          {
            "ChordId": 0,
            "ChordName": "G5",
            "Fingers": [
              1,
              3,
              -1,
              -1,
              -1,
              -1
            ],
            "Frets": [
              3,
              5,
              -1,
              -1,
              -1,
              -1
            ]
          },
 */
