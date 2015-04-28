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
                return (Time.GetHashCode() * 397) ^ Note;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Showlight) obj);
        }
        public bool Equals(Showlight other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Note == other.Note && Time.Equals(other.Time) || Time + 2.0D > other.Time;
        }

        #endregion
    }
}