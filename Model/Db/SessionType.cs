using System;
using PetaPoco;

namespace AARR_stat.Model.Db
{
    [TableName("sessiontype")]
    [PrimaryKey("Id")]
    public class SessionType
    {
        [Column(Name = "id")]
        public int Id { get; set; }
        [Column(Name = "name")]
        public string Name { get; set; }
        [Column(Name = "description")]
        public string Description { get; set; }
        
    }
}