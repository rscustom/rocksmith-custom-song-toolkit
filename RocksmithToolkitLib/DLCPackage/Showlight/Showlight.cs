using System.Xml.Serialization;

namespace RocksmithToolkitLib.DLCPackage.Showlight
{
    [XmlType("showlight")]
    public class Showlight : System.IEquatable<Showlight>
    {
        [XmlAttribute("time")]
        public float Time { get; set; }

        [XmlAttribute("note")]
        public int Note { get; set; }

        #region IEquatable implementation

        public static bool operator ==(Showlight left, Showlight right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Showlight left, Showlight right)
        {
            return !Equals(left, right);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                //Force Equals on each same note element, because we need near equal case for the time.
                return Note.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as Showlight;
            return other != null && Equals(other);
        }
        public bool Equals(Showlight other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Note == Note && Time.Equals(other.Time) || Time + 2.0F > other.Time; //will work only if collection is ordered by time.
        }

        #endregion
    }
}