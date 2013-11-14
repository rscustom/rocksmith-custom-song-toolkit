using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using RocksmithToolkitLib.Xml;
using System.Xml.Serialization;
using System.Text;

namespace RocksmithToolkitLib.Sng2014HSL
{
    public class Sng2014File : Sng {
        public Sng2014File() { }

        public Sng2014File(string file) {
            using (FileStream fs = new FileStream(file, FileMode.Open)) {
                BinaryReader r = new BinaryReader(fs);
                this.Read(r);
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

        public void Write(BinaryWriter w) {
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

        private void writeStruct(BinaryWriter w, object obj) {
            string[] order = (string[])getPropertyValue(obj, "order");
            foreach (string name in order) {
                var value = getPropertyValue(obj, name);
                Console.WriteLine("{0} = {1}", name, value);
                if (value.GetType().IsArray || value.GetType().IsPrimitive)
                    writeField(w, value);
                else
                    writeStruct(w, value);
            }
        }

        private void writeField(BinaryWriter w, object value) {
            Type type = value.GetType();
            string type_name = type.Name;

            if (type.IsArray) {
                if (type.GetElementType().IsPrimitive) {
                    foreach (var v in (IEnumerable)value) {
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
