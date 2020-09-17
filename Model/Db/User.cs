using System;
using PetaPoco;

namespace AARR_stat.Model.Db
{
    [TableName("user")]
    [PrimaryKey("Id")]
    public class User
    {
        [Column(Name = "id")]
        public string Id { get; set; }
    }
}