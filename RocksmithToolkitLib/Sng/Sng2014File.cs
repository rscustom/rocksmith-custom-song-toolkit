using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using RocksmithToolkitLib.Xml;
using System.Xml.Serialization;
using System.Text;

namespace RocksmithToolkitLib.Sng
{
    public class Sng2014File : Sng2014
    {
        public Sng2014File() {}
        
        public Sng2014File(string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Open)) {
                BinaryReader r = new BinaryReader(fs);
                this.Read(r);
            }
        }
        
        new public void Read(BinaryReader r) {
            this.BPMs = new BpmSection(); this.BPMs.Read(r);
            this.Phrases = new PhraseSection(); this.Phrases.Read(r);
            this.Chords = new ChordSection(); this.Chords.Read(r);
            this.ChordNotes = new ChordNotesSection(); this.ChordNotes.Read(r);
            this.Vocals = new VocalSection(); this.Vocals.Read(r);
            
            if (this.Vocals.Count > 0) {
                this.SymbolsHeader = new SymbolsHeaderSection(); this.SymbolsHeader.Read(r);
                this.SymbolsTexture = new SymbolsTextureSection(); this.SymbolsTexture.Read(r);
                this.SymbolsDefinition = new SymbolDefinitionSection(); this.SymbolsDefinition.Read(r);
            }
            
            this.PhraseIterations = new PhraseIterationSection(); this.PhraseIterations.Read(r);
            this.PhraseExtraInfo = new PhraseExtraInfoByLevelSection(); this.PhraseExtraInfo.Read(r);
            this.NLD = new NLinkedDifficultySection(); this.NLD.Read(r);
            this.Actions = new ActionSection(); this.Actions.Read(r);
            this.Events = new EventSection(); this.Events.Read(r);
            this.Tones = new ToneSection(); this.Tones.Read(r);
            this.DNAs = new DnaSection(); this.DNAs.Read(r);
            this.Sections = new SectionSection(); this.Sections.Read(r);
            this.Arrangements = new ArrangementSection(); this.Arrangements.Read(r);
            this.Metadata = new Metadata2014(); this.Metadata.Read(r);
        }
        
        public void write(BinaryWriter w)
        {
            write_struct(w, this.BPMs);
            write_struct(w, this.Phrases);
            write_struct(w, this.Chords);
            write_struct(w, this.ChordNotes);
            write_struct(w, this.Vocals);
        
            if (this.Vocals.Count > 0) {
                write_struct(w, this.SymbolsHeader);
                write_struct(w, this.SymbolsTexture);
                write_struct(w, this.SymbolsDefinition);
            }
        
            write_struct(w, this.PhraseIterations);
            write_struct(w, this.PhraseExtraInfo);
            write_struct(w, this.NLD);
            write_struct(w, this.Actions);
            write_struct(w, this.Events);
            write_struct(w, this.Tones);
            write_struct(w, this.DNAs);
            write_struct(w, this.Sections);
            write_struct(w, this.Arrangements);
            write_struct(w, this.Metadata);
        }

        public static void write_struct(BinaryWriter w, object obj)
        {
            string[] order = (string[])GetPropertyValue(obj, "order");

            foreach (string name in order) {
                var value = GetPropertyValue(obj, name);
                Console.WriteLine("{0} = {1}", name, value);

                if (value.GetType().IsArray || value.GetType().IsPrimitive)
                    write_field(w, value);
                else
                    write_struct(w, value);
            }
        }

        public static void write_field(BinaryWriter w, object value)
        {
            Type type = value.GetType();
            string type_name = type.Name;
        
            if (type.IsArray) {
                if (type.GetElementType().IsPrimitive) {
                    foreach (var v in (IEnumerable) value) {
                        Console.WriteLine("{0}", v);
                        write_field(w, v);
                    }
                }
                else
                {
                    foreach (var v in (IEnumerable) value)
                        write_struct(w, v);
                }
            }
            else
            {
                switch (type_name) {
                    case "Int32":
                        w.Write((Int32) value);
                        break;
                    case "Int16":
                        w.Write((Int16) value);
                        break;
                    case "Byte":
                        w.Write((Byte) value);
                        break;
                    case "Single":
                        w.Write((float) value);
                        break;
                    case "Double":
                        w.Write((double) value);
                        break;
                    default:
                        Console.WriteLine("Unhandled type {0} (value: {1})", type_name, value);
                        throw new System.Exception("Unhandled type");
                }
            }
        }

        public static object GetPropertyValue(object obj, string propertyName)
        {
            Type t = obj.GetType();
            PropertyInfo prop = t.GetProperty(propertyName);
            if(null != prop)
                return prop.GetValue(obj, null);
        
            Console.WriteLine("Unknown property {0} in {1}", propertyName, obj);
            throw new System.Exception("Unknown or unaccessible property");
        }
    }
}
