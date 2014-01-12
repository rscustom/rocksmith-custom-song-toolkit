using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.DLCPackage;
using System.Xml.Serialization;
using System.Text;
using MiscUtil.IO;
using MiscUtil.Conversion;
using zlib;
using DamienG.Security.Cryptography;

namespace RocksmithToolkitLib.Sng2014HSL
{
    public class Sng2014File : Sng {
        private bool consoleMode = !Environment.UserInteractive;

        public static Sng2014File ConvertXML(string xml_path, ArrangementType type)
        {
            if (type != ArrangementType.Vocal) {
                return Sng2014File.ConvertSong(xml_path);
            } else {
                return Sng2014FileWriter.read_vocals(xml_path);
            }
        }

        public Sng2014File() { }

        // Easy, Medium, Hard = 0, 1, 2
        public int[] NoteCount { get ; set; }
        // none, solo, riff, chord
        public int[] DNACount { get ; set ; }

        // this is platform independent SNG object
        public static Sng2014File ConvertSong(string xml_file) {
            Song2014 song = Song2014.LoadFromFile(xml_file);
            var parser = new Sng2014FileWriter();
            Sng2014File sng = new Sng2014File();
            parser.read_song(song, sng);
            sng.NoteCount = parser.NoteCount;
            sng.DNACount = parser.DNACount;
            return sng;
        }

        /// <summary>
        /// Raw SNG data reader, pretty much useless but it's here.
        /// Works for PC for default.
        /// </summary>
        /// <param name="sng_file"></param>
        /// <returns></returns>
        public static Sng2014File readSng(string sng_file) {
            using (FileStream fs = new FileStream(sng_file, FileMode.Open)) {
                Sng2014File sng = readSng(fs, new Platform(GamePlatform.Pc, GameVersion.RS2014));
                return sng;
            }
        }

        public static Sng2014File readSng(Stream input, Platform platform) 
        {
            Sng2014File sng = new Sng2014File();
            EndianBitConverter conv = EndianBitConverter.Little;
            byte[] key = RijndaelEncryptor.SngKeyPC;
            switch (platform.platform) {
                case GamePlatform.Pc:
                    conv = EndianBitConverter.Little;
                    key = RijndaelEncryptor.SngKeyPC;
                    break;
                case GamePlatform.Mac:
                    conv = EndianBitConverter.Little;
                    key = RijndaelEncryptor.SngKeyPC;
                    break;
                case GamePlatform.XBox360:
                case GamePlatform.PS3:
                    // xbox
                    conv = EndianBitConverter.Big;
                    break;
            }
            using (var ms = new MemoryStream())
            using (var r = new BinaryReader(ms))
            {
                if (conv == EndianBitConverter.Big) sng.Read(r);
                else {
                    RijndaelEncryptor.DecryptSng(input, ms, key, conv);
                    sng.Read(r);
                }
            }
            return sng;
        }

        public Sng2014File(Stream data) {
            BinaryReader r = new BinaryReader(data);
            this.Read(r);
        }

        public void writeSng(Stream output, Platform platform)
        {
            EndianBitConverter conv;
            Int32 platform_header;
            switch (platform.platform) {
                case GamePlatform.Pc:
                case GamePlatform.Mac:
                // PC
                    conv = EndianBitConverter.Little;
                    platform_header = 3;
                    break;
                case GamePlatform.XBox360:
                case GamePlatform.PS3:
                // xbox
                    conv = EndianBitConverter.Big;
                    platform_header = 1;
                    break;
                default: conv = EndianBitConverter.Little; platform_header = 3; break;
            }
            using (EndianBinaryWriter w = new EndianBinaryWriter(conv, output)) {
                w.Write((Int32) 0x4A);
                w.Write(platform_header);

                // pack with zlib
                byte[] chart_data = this.getChartData(platform);
                MemoryStream zData = new MemoryStream();
                ZOutputStream zOut = new ZOutputStream(zData, zlib.zlibConst.Z_BEST_COMPRESSION);
                zOut.Write(chart_data, 0, chart_data.Length);
                zOut.finish();
                byte[] packed = zData.ToArray();

                if (platform_header == 3) {
                    MemoryStream encrypted = new MemoryStream();
                    MemoryStream plain = new MemoryStream();
                    var encw = new EndianBinaryWriter(conv, plain);
                    // write size of uncompressed data and packed data itself
                    encw.Write((Int32) chart_data.Length);
                    encw.Write(packed);
                    encw.Flush();
                    MemoryStream input = new MemoryStream(plain.ToArray());
                    // choose key
                    byte[] key;
                    if (platform.platform == GamePlatform.Mac)
                        key = RijndaelEncryptor.SngKeyMac;
                    else
                        key = RijndaelEncryptor.SngKeyPC;
                    // encrypt (writes 16B IV and encrypted data)
                    RijndaelEncryptor.EncryptSngData(input, encrypted, key);
                    w.Write(encrypted.ToArray());
                    // append zero signature
                    w.Write(new Byte[56]);
                } else {
                    // unencrypted and unsigned
                    w.Write((Int32) chart_data.Length);
                    w.Write(packed);
                }

                output.Flush();
            }
        }

        private byte[] chart_LE = null;
        private byte[] chart_BE = null;
        public byte[] getChartData(Platform platform)
        {
            using (MemoryStream stream = new MemoryStream()) {
                EndianBitConverter conv;
                if (platform.platform == GamePlatform.Pc ||
                    platform.platform == GamePlatform.Mac)
                    conv = EndianBitConverter.Little;
                else
                    conv = EndianBitConverter.Big;

                // cached result
                if (conv == EndianBitConverter.Little && chart_LE != null)
                    return chart_LE;
                if (conv == EndianBitConverter.Big && chart_BE != null)
                    return chart_BE;

                using (EndianBinaryWriter w = new EndianBinaryWriter(conv, stream)) {
                    this.Write(w);
                }
                stream.Flush();

                var data = stream.ToArray();
                if (conv == EndianBitConverter.Little)
                    chart_LE = data;
                else
                    chart_BE = data;

                return data;
            }
        }

        public void Read(BinaryReader r) {
            this.BPMs = new BpmSection(); this.BPMs.read(r);
            this.Phrases = new PhraseSection(); this.Phrases.read(r);
            this.Chords = new ChordSection(); this.Chords.read(r);
            this.ChordNotes = new ChordNotesSection(); this.ChordNotes.read(r);
            this.Vocals = new VocalSection(); this.Vocals.read(r);
            if (this.Vocals.Count > 0) {
                this.SymbolsHeader = new SymbolsHeaderSection(); this.SymbolsHeader.read(r);
                this.SymbolsTexture = new SymbolsTextureSection(); this.SymbolsTexture.read(r);
                this.SymbolsDefinition = new SymbolDefinitionSection(); this.SymbolsDefinition.read(r);
            }
            this.PhraseIterations = new PhraseIterationSection(); this.PhraseIterations.read(r);
            this.PhraseExtraInfo = new PhraseExtraInfoByLevelSection(); this.PhraseExtraInfo.read(r);
            this.NLD = new NLinkedDifficultySection(); this.NLD.read(r);
            this.Actions = new ActionSection(); this.Actions.read(r);
            this.Events = new EventSection(); this.Events.read(r);
            this.Tones = new ToneSection(); this.Tones.read(r);
            this.DNAs = new DnaSection(); this.DNAs.read(r);
            this.Sections = new SectionSection(); this.Sections.read(r);
            this.Arrangements = new ArrangementSection(); this.Arrangements.read(r);
            this.Metadata = new Metadata(); this.Metadata.read(r);
        }

        public void Write(EndianBinaryWriter w) {
            writeStruct(w, this.BPMs);
            writeStruct(w, this.Phrases);
            writeStruct(w, this.Chords);
            writeStruct(w, this.ChordNotes);

            writeStruct(w, this.Vocals);
            if (this.Vocals.Count > 0) {
                writeStruct(w, this.SymbolsHeader);
                writeStruct(w, this.SymbolsTexture);
                writeStruct(w, this.SymbolsDefinition);
            }

            writeStruct(w, this.PhraseIterations);
            writeStruct(w, this.PhraseExtraInfo);
            writeStruct(w, this.NLD);
            writeStruct(w, this.Actions);
            writeStruct(w, this.Events);
            writeStruct(w, this.Tones);
            writeStruct(w, this.DNAs);
            writeStruct(w, this.Sections);
            writeStruct(w, this.Arrangements);
            writeStruct(w, this.Metadata);
        }

        public MemoryStream copyStruct(object obj) {
            EndianBitConverter conv = EndianBitConverter.Little;
            MemoryStream data = new MemoryStream();
            var w = new EndianBinaryWriter(conv, data);
            bool mode = this.consoleMode;
            this.consoleMode = false;
            writeStruct(w, obj);
            this.consoleMode = mode;
            w.Flush();
            data.Position = 0;
            return data;
        }

        public UInt32 hashStruct(object obj) {
            MemoryStream data = copyStruct(obj);
            UInt32 crc = Crc32.Compute(data.ToArray());
            return crc;
        }

        private void writeStruct(EndianBinaryWriter w, object obj) {
            string[] order = (string[])getPropertyValue(obj, "order");
            foreach (string name in order) {
                var value = getPropertyValue(obj, name);
                if (consoleMode)
                    Console.WriteLine("{0} = {1}", name, value);
                if (value.GetType().IsArray || value.GetType().IsPrimitive)
                    writeField(w, value);
                else
                    writeStruct(w, value);
            }
        }

        private void writeField(EndianBinaryWriter w, object value) {
            Type type = value.GetType();
            string type_name = type.Name;

            if (type.IsArray) {
                if (type.GetElementType().IsPrimitive) {
                    foreach (var v in (IEnumerable)value) {
                        if (consoleMode)
                            Console.WriteLine("{0}", v);
                        writeField(w, v);
                    }
                } else {
                    foreach (var v in (IEnumerable)value)
                        writeStruct(w, v);
                }
            } else {
                switch (type_name) {
                    case "UInt32":
                        w.Write((UInt32)value);
                        if (consoleMode)
                            Console.WriteLine("mask: {0:x}", value);
                        break;
                    case "Int32":
                        w.Write((Int32)value);
                        break;
                    case "Int16":
                        w.Write((Int16)value);
                        break;
                    case "Byte":
                        w.Write((Byte)value);
                        break;
                    case "Single":
                        w.Write((float)value);
                        break;
                    case "Double":
                        w.Write((double)value);
                        break;
                    default:
                        if (consoleMode)
                            Console.WriteLine("Unhandled type {0} (value: {1})", type_name, value);
                        throw new System.Exception("Unhandled type");
                }
            }
        }

        private object getPropertyValue(object obj, string propertyName) {
            Type t = obj.GetType();
            PropertyInfo prop = t.GetProperty(propertyName);
            if (prop != null)
                return prop.GetValue(obj, null);
            else
                throw new System.Exception("Unknown or unaccessible property");
        }
    }
}
