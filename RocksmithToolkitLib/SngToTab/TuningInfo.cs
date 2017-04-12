using System.Text;

namespace RocksmithToolkitLib.SngToTab
{
    public enum Tuning
    {
        Standard = 0,
        DropD = 1,
        EFlat = 2,
        OpenG = 3
    }

    public class TuningInfo
    {
        private readonly TabFile TabFile;
        private readonly Tuning Tuning;
        private readonly string Name;
        private readonly string[] Notes;
        private string[] lines;

        public string[] StartLines
        {
            get
            {
                if (lines != null)
                    return (string[])lines.Clone();

                int length = 0;
                foreach (string n in Notes)
                {
                    if (n.Length > length)
                        length = n.Length;
                }

                lines = new string[TabFile.LineCount];
                lines[0] = "".PadLeft(length + 1, TabFile.PADDING_INFO);
                lines[1] = "".PadLeft(length + 1, TabFile.PADDING_INFO);
                lines[2] = "".PadLeft(length + 1, TabFile.PADDING_INFO);
                lines[TabFile.FIRST_STRING + 0] = Notes[0].PadLeft(length, TabFile.PADDING_INFO) + "|";
                lines[TabFile.FIRST_STRING + 1] = Notes[1].PadLeft(length, TabFile.PADDING_INFO) + "|";
                lines[TabFile.FIRST_STRING + 2] = Notes[2].PadLeft(length, TabFile.PADDING_INFO) + "|";
                lines[TabFile.FIRST_STRING + 3] = Notes[3].PadLeft(length, TabFile.PADDING_INFO) + "|";
                lines[TabFile.FIRST_STRING + 4] = Notes[4].PadLeft(length, TabFile.PADDING_INFO) + "|";
                lines[TabFile.FIRST_STRING + 5] = Notes[5].PadLeft(length, TabFile.PADDING_INFO) + "|";

                return (string[])lines.Clone();
            }
        }

        public string Description
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Name);
                sb.Append(" (");
                for (int i = Notes.Length - 1; i >= 0; i--)
                {
                    sb.Append(Notes[i]);
                    if (i != 0)
                        sb.Append(" ");
                }
                sb.Append(")");

                return sb.ToString();
            }
        }

        public TuningInfo(TabFile tabFile, Tuning tuning, string name, string[] notes)
        {
            TabFile = tabFile;
            Tuning = tuning;
            Name = name;
            Notes = notes;
        }
    }
}
