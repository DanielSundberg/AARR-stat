using System;
using Amazon.DynamoDBv2.DataModel;

namespace AARR_stat.Model.Db
{
    [DynamoDBTable("aarrstat-device")]
    class DynamoDbDevice
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
        public string User { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Internal { get; set; }
    }
}