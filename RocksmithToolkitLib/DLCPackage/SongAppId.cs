using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.DLCPackage
{
    public class SongAppId
    {
        private SongAppId() { }

        public string Name;
        public string AppId;
        public override string ToString()
        {
            return string.Format("{0} - {1}", Name, AppId);
        }

        public static IEnumerable<SongAppId> GetSongAppIds()
        {
            yield return new SongAppId { Name = "FREE Holiday Song Pack", AppId = "206175" };
            yield return new SongAppId { Name = "Albert King with Stevie Ray Vaughan - Born Under A Bad Sign", AppId = "206123" };
            yield return new SongAppId { Name = "Avenged Sevenfold - Afterlife", AppId = "206175" };
            yield return new SongAppId { Name = "Avenged Sevenfold - Beast and the Harlot", AppId = "206174" };
            yield return new SongAppId { Name = "Avenged Sevenfold - Nightmare", AppId = "206176" };
            yield return new SongAppId { Name = "B.B. King - The Thrill Is Gone", AppId = "206121" };
            yield return new SongAppId { Name = "Blink 182 - All The Small Things", AppId = "206114" };
            yield return new SongAppId { Name = "Blink 182 - Dammit", AppId = "206113" }; ;
            yield return new SongAppId { Name = "Blink 182 - Whats My Age Again", AppId = "206115" };
            yield return new SongAppId { Name = "Blue Oyster Cult - (Don't Fear) The Reaper", AppId = "206108" };
            yield return new SongAppId { Name = "Blue Oyster Cult - Godzilla", AppId = "206149" };
            yield return new SongAppId { Name = "Blue Oyster Cult - Godzilla", AppId = "206149" };
            yield return new SongAppId { Name = "Boston - More Than A Feeling", AppId = "206093" };
            yield return new SongAppId { Name = "Creedence Clearwater Revival - Born On The Bayou", AppId = "206158" };
            yield return new SongAppId { Name = "David Bowie - Space Oddity", AppId = "206110" };
            yield return new SongAppId { Name = "David Bowie - The Man Who Sold World", AppId = "206162" };
            yield return new SongAppId { Name = "Deep Purple - Smoke On The Water", AppId = "206095" };
            yield return new SongAppId { Name = "Europe - Final Countdown", AppId = "206166" };
            yield return new SongAppId { Name = "Europe - The Final Countdown", AppId = "222056" };
            yield return new SongAppId { Name = "Evanescence - Bring Me To Life", AppId = "206125" };
            yield return new SongAppId { Name = "Finger Eleven - Paralyzer", AppId = "206152" };
            yield return new SongAppId { Name = "Foster The People - Pumped Up Kicks", AppId = "206124" };
            yield return new SongAppId { Name = "Gary Clark Jr. - Bright Lights", AppId = "206167" };
            yield return new SongAppId { Name = "Grace Potter and the Nocturnals - Paris (Ooh La La)", AppId = "206168" };
            yield return new SongAppId { Name = "Heart - Barracuda", AppId = "206109" };
            yield return new SongAppId { Name = "Iron Butterfly - In A Gadda Da Vida", AppId = "206159" };
            yield return new SongAppId { Name = "Iron Butterfly - In-A-Gadda-Da-Vida", AppId = "206159" };
            yield return new SongAppId { Name = "Judas Priest - Breaking The Law", AppId = "206127" };
            yield return new SongAppId { Name = "Judas Priest - Living After Midnight", AppId = "206128" };
            yield return new SongAppId { Name = "Judas Priest - Painkiller", AppId = "206129" };
            yield return new SongAppId { Name = "Lamb Of God - Redneck", AppId = "206153" };
            yield return new SongAppId { Name = "Maroon 5 - This Love", AppId = "206126" };
            yield return new SongAppId { Name = "Marvin Gaye - What's Going On", AppId = "206156" };
            yield return new SongAppId { Name = "Megadeth - Hangar 18", AppId = "206100" };
            yield return new SongAppId { Name = "Megadeth - Public Enemy Number One", AppId = "206101" };
            yield return new SongAppId { Name = "Megadeth - Symphony Of Destruction", AppId = "206099" };
            yield return new SongAppId { Name = "Mountain - Mississippi Queen", AppId = "206163" };
            yield return new SongAppId { Name = "My Chemical Romance - NaNaNa", AppId = "206140" };
            yield return new SongAppId { Name = "My Chemical Romance - Planetary Go", AppId = "206141" };
            yield return new SongAppId { Name = "My Chemical Romance - Welcome To The Black Parade", AppId = "206139" };
            yield return new SongAppId { Name = "Otis Redding - (Sittin' On) The Dock Of The Bay", AppId = "206157" };
            yield return new SongAppId { Name = "Pantera - Cowboys From Hell", AppId = "222054" };
            yield return new SongAppId { Name = "Pantera - Domination", AppId = "222055" };
            yield return new SongAppId { Name = "Pantera - Walk", AppId = "222056" };
            yield return new SongAppId { Name = "Pat Benetar - Hit Me With Your Best Shot", AppId = "206150" };
            yield return new SongAppId { Name = "Pearl Jam - Black", AppId = "206111" };
            yield return new SongAppId { Name = "Queen - Bohemian Rhapsody", AppId = "206143" };
            yield return new SongAppId { Name = "Queen - Bohemian Rhapsody", AppId = "206143" };
            yield return new SongAppId { Name = "Queen - Fat Bottomed Girls", AppId = "206145" };
            yield return new SongAppId { Name = "Queen - Keep Yourself Alive", AppId = "206146" };
            yield return new SongAppId { Name = "Queen - Killer Queen", AppId = "206147" };
            yield return new SongAppId { Name = "Queen - Stone Cold Crazy", AppId = "206144" };
            yield return new SongAppId { Name = "Rush - Headlong Flight", AppId = "" };
            yield return new SongAppId { Name = "Rush - Limelight", AppId = "206179" };
            yield return new SongAppId { Name = "Rush - Red Barchetta", AppId = "222042" };
            yield return new SongAppId { Name = "Rush - Subdivisions", AppId = "206113" };
            yield return new SongAppId { Name = "Rush - Tom Sawyer", AppId = "206178" };
            yield return new SongAppId { Name = "Rush - YYZ", AppId = "222041" };
            yield return new SongAppId { Name = "T Rex - 20th Century Boy", AppId = "206096" };
            yield return new SongAppId { Name = "The Allman Brothers Band - Whipping Post", AppId = "206160" };
            yield return new SongAppId { Name = "The Black Keys - Gold On The Celing", AppId = "206104" };
            yield return new SongAppId { Name = "The Black Keys - Just Got To Be", AppId = "206106" };
            yield return new SongAppId { Name = "The Black Keys - Mind Eraser", AppId = "206105" };
            yield return new SongAppId { Name = "The Blues Brothers - Soul Man", AppId = "206122" };
            yield return new SongAppId { Name = "The Darkness - I Believe In A Thing Called Love", AppId = "206154" };
            yield return new SongAppId { Name = "The Darkness - I Believe In A Thing Called Love", AppId = "206154" };
            yield return new SongAppId { Name = "The Knack - My Sharona", AppId = "206151" };
            yield return new SongAppId { Name = "The Offspring - Come Out And Play", AppId = "206137" };
            yield return new SongAppId { Name = "The Offspring - Gone Away", AppId = "206136" };
            yield return new SongAppId { Name = "The Offspring - Self Esteem", AppId = "206135" };
            yield return new SongAppId { Name = "The Police - Message In A Bottle", AppId = "206132" };
            yield return new SongAppId { Name = "The Police - Roxanne", AppId = "206131" };
            yield return new SongAppId { Name = "The Police - SynchronicityII", AppId = "206133" };
            yield return new SongAppId { Name = "The Shins - Caring Is Creepy", AppId = "206169" };
            yield return new SongAppId { Name = "Three Days Grace - I Hate Everything About You", AppId = "206097" };
            yield return new SongAppId { Name = "Three Doors Down - Kryptonite", AppId = "206118" };
            yield return new SongAppId { Name = "Three Doors Down - Loser", AppId = "206119" };
            yield return new SongAppId { Name = "Three Doors Down - When I'm Gone", AppId = "206117" };
            yield return new SongAppId { Name = "Twisted Sister - Were Not Gonna take It", AppId = "206164" };
            yield return new SongAppId { Name = "Vampire Weekend - Cousins", AppId = "206098" };
            yield return new SongAppId { Name = "Whitesnake - Is This Love", AppId = "206165" };

        }
    }
}