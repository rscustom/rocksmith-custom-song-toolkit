using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RocksmithToolkitLib.Tone;

namespace RocksmithToolkitLib
{
    public static class GameData
    {
        public static IList<Pedal> GetPedalData()
        {
            var pedalsJson = Encoding.ASCII.GetString(Properties.Resources.pedals);
            return JsonConvert.DeserializeObject<IList<Pedal>>(pedalsJson);
        }
    }
}
