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

namespace RocksmithToolkitLib.Sng2014HSL
{
    public class Sng2014File : Sng {
        private bool consoleMode = !Environment.UserInteractive;

        public Sng2014File() { }

        // this is platform independent SNG object
        public Sng2014File(string xml_file, ArrangementType arrangementType) {
            Song2014 song = Song2014.LoadFromFile(xml_file);
            var parser = new Sng2014FileWriter();
            parser.readXml(song, this, arrangementType);
        }

        // raw SNG data reader, pretty much useless but it's here
        public Sng2014File(string sng_file) {
            using (FileStream fs = new FileStream(sng_file, FileMode.Open)) {
                BinaryReader r = new BinaryReader(fs);
                this.Read(r);
            }
        }

        public void writeSng(Stream output, Platform platform)
        {
            EndianBitConverter conv;
            Int32 platform_header;
            if (platform.platform == GamePlatform.Pc ||
                platform.platform == GamePlatform.Mac) {
                // PC
                conv = EndianBitConverter.Little;
                platform_header = 3;
            } else {
                // xbox
                conv = EndianBitConverter.Big;
                platform_header = 1;
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
