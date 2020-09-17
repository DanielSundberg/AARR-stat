using System;

namespace AARR_stat
{
    public class AARRStatSessionItem
    {
        public string User { get; set; }
        public string Device { get; set; }
        public string Session { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
    }
}
