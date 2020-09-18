using System;
using PetaPoco;

namespace AARR_stat.Model.Dto
{
    public class SessionViewDto
    {
        [Column(Name = "type")]
        public string Type { get; set; }

        [Column(Name = "user_id")]
        public string User { get; set; }
        [Column(Name = "device_id")]
        public string Device { get; set; }
        [Column(Name = "id")]
        public string Session { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
    }
}
