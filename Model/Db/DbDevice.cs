using System;
using Amazon.DynamoDBv2.DataModel;

namespace AARR_stat.Model.Db
{
    [DynamoDBTable("aarrstat-device")]
    class DbDevice
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
        public string User { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        [DynamoDBProperty("RegisterDate", typeof(DateTimeUtcConverter))]
        public DateTime RegisterDate { get; set; }
        [DynamoDBProperty("UpdateDate", typeof(DateTimeUtcConverter))]
        public DateTime UpdateDate { get; set; }
        public bool Internal { get; set; }
    }
}