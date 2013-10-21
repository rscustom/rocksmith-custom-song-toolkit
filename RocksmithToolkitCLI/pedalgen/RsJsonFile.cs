using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pedalgen
{
    public class RsJsonFile<T>
    {
        public string ModelName { get; set; }
        public int IterationVersion { get; set; }
        public IList<T> Entries { get; set; }
    }
}
