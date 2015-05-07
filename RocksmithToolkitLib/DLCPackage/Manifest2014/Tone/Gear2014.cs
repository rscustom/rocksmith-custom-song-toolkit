using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace RocksmithToolkitLib.DLCPackage.Manifest.Tone
{
    public class Gear2014
    {
        public Pedal2014 Rack1 { get; set; }
        public Pedal2014 Rack2 { get; set; }
        public Pedal2014 Rack3 { get; set; }
        public Pedal2014 Rack4 { get; set; }

        public Pedal2014 Amp { get; set; }
        public Pedal2014 Cabinet { get; set; }

        public Pedal2014 PrePedal1 { get; set; }
        public Pedal2014 PrePedal2 { get; set; }
        public Pedal2014 PrePedal3 { get; set; }
        public Pedal2014 PrePedal4 { get; set; }

        public Pedal2014 PostPedal1 { get; set; }
        public Pedal2014 PostPedal2 { get; set; }
        public Pedal2014 PostPedal3 { get; set; }
        public Pedal2014 PostPedal4 { get; set; }

        public Pedal2014 this[string propertyName] {
            get {
                Type myType = typeof(Gear2014);
                PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                return (myPropInfo != null) ? (Pedal2014)myPropInfo.GetValue(this, null) : null;
            }
            set {
                Type myType = typeof(Gear2014);
                PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                myPropInfo.SetValue(this, value, null);
            }
        }
        public bool IsNull()
        {
            return ReferenceEquals(Amp, null) && ReferenceEquals(Cabinet, null);
        }
        public int SlotsUsed()
        {
            int i = 0;
            i += Amp != null ? 1 : 0;
            i += Cabinet != null ? 1 : 0;

            i += Rack1 != null ? 1 : 0; i += Rack2 != null ? 1 : 0;
            i += Rack3 != null ? 1 : 0; i += Rack4 != null ? 1 : 0;

            i += PrePedal1 != null ? 1 : 0; i += PrePedal2 != null ? 1 : 0;
            i += PrePedal3 != null ? 1 : 0; i += PrePedal4 != null ? 1 : 0;

            i += PostPedal1 != null ? 1 : 0; i += PostPedal2 != null ? 1 : 0;
            i += PostPedal3 != null ? 1 : 0; i += PostPedal4 != null ? 1 : 0;
            return i;
        }
    }
}
