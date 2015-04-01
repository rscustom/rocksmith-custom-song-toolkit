using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.Sng
{
    class Constants
    {
        // ****************************************************
        // DO NOT CHANGE FORMAT OF THIS CODE - NO CTRL-E,CTRL-D
        // PROTECTED BEAUTIFICATION CODE AREA
        // ****************************************************
        // more constants: RocksmithSngHSL/RocksmithSng_constants.txt
        // unknown constant
        public const UInt32 NOTE_TURNING_BPM_TEMPO      = 0x00000004;

        // chord template Mask (displayName ends with "arp" or "nop")
        public const UInt32 CHORD_MASK_ARPEGGIO         = 0x00000001;
        public const UInt32 CHORD_MASK_NOP              = 0x00000002;

        // NoteFlags:
        public const UInt32 NOTE_FLAGS_NUMBERED         = 0x00000001;

        // NoteMask:
        public const UInt32 NOTE_MASK_UNDEFINED         = 0x0;
        // missing - not used in lessons/songs            0x01
        public const UInt32 NOTE_MASK_CHORD             = 0x02;
        public const UInt32 NOTE_MASK_OPEN              = 0x04;
        public const UInt32 NOTE_MASK_FRETHANDMUTE      = 0x08;
        public const UInt32 NOTE_MASK_TREMOLO           = 0x10;
        public const UInt32 NOTE_MASK_HARMONIC          = 0x20;
        public const UInt32 NOTE_MASK_PALMMUTE          = 0x40;
        public const UInt32 NOTE_MASK_SLAP              = 0x80;
        public const UInt32 NOTE_MASK_PLUCK             = 0x0100;
        public const UInt32 NOTE_MASK_POP               = 0x0100;
        public const UInt32 NOTE_MASK_HAMMERON          = 0x0200;
        public const UInt32 NOTE_MASK_PULLOFF           = 0x0400;
        public const UInt32 NOTE_MASK_SLIDE             = 0x0800;
        public const UInt32 NOTE_MASK_BEND              = 0x1000;
        public const UInt32 NOTE_MASK_SUSTAIN           = 0x2000;
        public const UInt32 NOTE_MASK_TAP               = 0x4000;
        public const UInt32 NOTE_MASK_PINCHHARMONIC     = 0x8000;
        public const UInt32 NOTE_MASK_VIBRATO           = 0x010000;
        public const UInt32 NOTE_MASK_MUTE              = 0x020000;
        public const UInt32 NOTE_MASK_IGNORE            = 0x040000;   // ignore=1
        public const UInt32 NOTE_MASK_LEFTHAND          = 0x00080000;
        public const UInt32 NOTE_MASK_RIGHTHAND         = 0x00100000;
        public const UInt32 NOTE_MASK_HIGHDENSITY       = 0x200000;
        public const UInt32 NOTE_MASK_SLIDEUNPITCHEDTO  = 0x400000;
        public const UInt32 NOTE_MASK_SINGLE            = 0x00800000; // single note
        public const UInt32 NOTE_MASK_CHORDNOTES        = 0x01000000; // has chordnotes exported
        public const UInt32 NOTE_MASK_DOUBLESTOP        = 0x02000000;
        public const UInt32 NOTE_MASK_ACCENT            = 0x04000000;
        public const UInt32 NOTE_MASK_PARENT            = 0x08000000; // linkNext=1
        public const UInt32 NOTE_MASK_CHILD             = 0x10000000; // note after linkNext=1
        public const UInt32 NOTE_MASK_ARPEGGIO          = 0x20000000;
        // missing - not used in lessons/songs            0x40000000
        public const UInt32 NOTE_MASK_STRUM             = 0x80000000; // handShape defined at chord time

        public const UInt32 NOTE_MASK_ARTICULATIONS_RH  = 0x0000C1C0;
        public const UInt32 NOTE_MASK_ARTICULATIONS_LH  = 0x00020628;
        public const UInt32 NOTE_MASK_ARTICULATIONS     = 0x0002FFF8;
        public const UInt32 NOTE_MASK_ROTATION_DISABLED = 0x0000C1E0;

    }
}
