using System.Collections.Generic;
using System.Linq;

namespace RocksmithToolkitLib.ToolkitTone
{
    public enum PedalType
    {
        Amp,
        Cabinet,
        Pedal,
        Rack
    }

    public interface IToolkitPedal
    {
        string Name { get; set; }
        string Type { get; set; }
        string Category { get; set; }
        string Key { get; set; }
        IList<ToolkitKnob> Knobs { get; set; }
        PedalType TypeEnum { get; }
        string DisplayName { get; }
    }
}