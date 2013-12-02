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
    }
}
