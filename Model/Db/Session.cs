using System;
using PetaPoco;

namespace AARR_stat.Model.Db
{
    [TableName("session")]
    [PrimaryKey("id")]
    public class Session
    {
        [Column(Name = "id")]
        public string Id { get; set; }
        [Column(Name = "type_id")]
        public int Type { get; set; }

        [Column(Name = "user_id")]
         public string User { get; set; }
        [Column(Name = "device_id")]
        public string Device { get; set; }
        
        [Column(Name = "start")]
        public DateTime Start { get; set; }
        [Column(Name = "end")]
        public DateTime? End { get; set; }
    }
}