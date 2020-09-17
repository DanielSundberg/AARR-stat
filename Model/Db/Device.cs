using System;
using PetaPoco;

namespace AARR_stat.Model.Db
{
    [TableName("device")]
    [PrimaryKey("id")]
    public class Device
    {
        [Column(Name = "id")]
        public string Id { get; set; }
        [Column(Name = "user")]
        public string User { get; set; }
    }
}