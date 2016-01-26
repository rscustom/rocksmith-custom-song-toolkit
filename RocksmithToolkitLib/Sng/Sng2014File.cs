using System;
using System.Collections;
using System.IO;
using System.Reflection;
using MiscUtil.Conversion;
using MiscUtil.IO;

using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Xml;

namespace RocksmithToolkitLib.Sng2014HSL
{
    public class Sng2014File : Sng {
        private bool consoleMode = !Environment.UserInteractive;

        public static Sng2014File ConvertXML(string xmlPath, ArrangementType type, string cdata = null)
        {
            if (type == ArrangementType.Vocal) {
                return Sng2014FileWriter.ReadVocals(xmlPath, cdata);
            }
            return Sng2014File.ConvertSong(xmlPath);
        }

        public Sng2014File() { }

        // Easy, Medium, Hard = 0, 1, 2
        public int[] NoteCount { get ; set; }
        // none, solo, riff, chord
        public int[] DNACount { get ; set ; }

        // this is platform independent SNG object
        public static Sng2014File ConvertSong(string xmlFile) {
            var song = Song2014.LoadFromFile(xmlFile);
            var parser = new Sng2014FileWriter();
            var sng = new Sng2014File();
            parser.ReadSong(song, sng);
            sng.NoteCount = parser.NoteCount;
            sng.DNACount = parser.DNACount;
            return sng;
        }

        /// <summary>
        /// Raw SNG data reader.
        /// </summary>
        /// <param name="inputFile">Packed and encrypted SNG file</param>
        /// <returns><see cref="Sng2014File"/> exemplar.</returns>
        /// <param name = "platform"></param>
        public static Sng2014File LoadFromFile(string inputFile, Platform platform) {
            using (var fs = new FileStream(inputFile, FileMode.Open)) {
                return ReadSng(fs, platform);
            }
        }

        public static Sng2014File ReadSng(Stream input, Platform platform) 
        {
            var sng = new Sng2014File();

            using (var ms = new MemoryStream())
            using (var r = new EndianBinaryReader(platform.GetBitConverter, ms))
            {
                UnpackSng(input, ms, platform);
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                sng.Read(r);
            }

            return sng;
        }

        public static void UnpackSng(Stream input, Stream output, Platform platform) {
            EndianBitConverter conv = platform.GetBitConverter;

            using (var decrypted = new MemoryStream())
            using (var ebrDec = new EndianBinaryReader(conv, decrypted)) {
                byte[] key;
                switch (platform.platform) {
                    case GamePlatform.Mac:
                        key = RijndaelEncryptor.SngKeyMac;
                        break;
                    case GamePlatform.Pc:
                        key = RijndaelEncryptor.SngKeyPC;
                        break;
                    default:
                        key = null;
                        break;
                }
                if (key != null)
                    RijndaelEncryptor.DecryptSngData(input, decrypted, key, conv);
                else {
                    input.CopyTo(decrypted);
                    decrypted.Seek(8, SeekOrigin.Begin);
                }
                //unZip
                long plainLen = ebrDec.ReadUInt32();
                ushort xU = ebrDec.ReadUInt16();
                decrypted.Position -= 2;
                if (xU == 0x78DA || xU == 0xDA78) {//LE 55928 //BE 30938
                    RijndaelEncryptor.Unzip(decrypted, output, false);
                }
            }
        }

        public Sng2014File(Stream data) {
            var r = new EndianBinaryReader(EndianBitConverter.Little, data);
            this.Read(r);
        }

        public void WriteSng(Stream output, Platform platform) {
            byte[] chartData = this.getChartData(platform);
            using (Stream input = new MemoryStream(chartData))
            {
                PackSng(input, output, platform);
            }
        }

        public static void PackSng(Stream input, Stream output, Platform platform)
        {
            EndianBitConverter conv = platform.GetBitConverter;
            Int32 platformHeader = (conv == EndianBitConverter.Big)? 1 : 3;

            using( var w = new EndianBinaryWriter(conv, output) )
            using( var zData = new MemoryStream() )
            using( var plain = new MemoryStream() )
            using( var encrypted = new MemoryStream() )
            using( var encw = new EndianBinaryWriter(conv, plain) )
            {
                w.Write((Int32) 0x4A);
                w.Write(platformHeader);

                // pack with zlib TODO: better packing required!
                RijndaelEncryptor.Zip(input, zData, input.Length);

                if (platformHeader == 3) {
                    // write size of uncompressed data and packed data itself | already there
                    encw.Write((Int32)input.Length);
                    encw.Write(zData.GetBuffer());
                    encw.Flush();

                    // choose key
                    byte[] key;
                    switch (platform.platform) {
                        case GamePlatform.Mac:
                            key = RijndaelEncryptor.SngKeyMac;
                            break;
                        default: //PC
                            key = RijndaelEncryptor.SngKeyPC;
                            break;
                    }
                    // encrypt (writes 16B IV and encrypted data)
                    plain.Position = 0;
                    RijndaelEncryptor.EncryptSngData(plain, encrypted, key);
                    w.Write(encrypted.GetBuffer());
                    w.Write(new Byte[56]); // append zero signature
                } else {
                    // unencrypted and unsigned
                    w.Write((Int32)input.Length);
                    w.Write(zData.GetBuffer());
                }

                output.Flush();
            }
        }

        private byte[] chartLE = null;
        private byte[] chartBE = null;
        private byte[] getChartData(Platform platform)
        {
            using (var stream = new MemoryStream()) {
                var conv = platform.GetBitConverter;

                // cached result
                if (conv == EndianBitConverter.Little && chartLE != null)
                    return chartLE;
                if (conv == EndianBitConverter.Big && chartBE != null)
                    return chartBE;

                using (var w = new EndianBinaryWriter(conv, stream)) {
                    this.Write(w);
                }
                stream.Flush();

                var data = stream.ToArray();
                if (conv == EndianBitConverter.Little)
                    chartLE = data;
                else
                    chartBE = data;

                return data;
            }
        }

        public void WriteChartData(string outfile, Platform platform)
        {
            var data = getChartData(platform);
            using (var fs = new FileStream(outfile, FileMode.Create)){
                fs.Write(data, 0, data.Length);
            }
        }

        public bool IsCustomFont()
        {
            return !this.SymbolsTexture.SymbolsTextures[0].Font.ToNullTerminatedUTF8().Contains("lyrics.dds");
        }

        public void Read(EndianBinaryReader r) {
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

        public MemoryStream CopyStruct(object obj) {
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

        public UInt32 HashStruct(object obj) {
            MemoryStream data = CopyStruct(obj);
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
            string typeName = type.Name;

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
                switch (typeName) {
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
                            Console.WriteLine("Unhandled type {0} (value: {1})", typeName, value);
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
