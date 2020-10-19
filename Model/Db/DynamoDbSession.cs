using System;
using Amazon.DynamoDBv2.DataModel;

namespace AARR_stat.Model.Db
{
    [DynamoDBTable("aarrstat-session")]
    class DynamoDbSession
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
        public string Type { get; set; }
        public string User { get; set; }
        public string Device { get; set; }
        [DynamoDBProperty("Start", typeof(DateTimeUtcConverter))]
        public DateTime Start { get; set; }
        [DynamoDBProperty("End", typeof(DateTimeUtcConverter))]
        public DateTime? End { get; set; }
    }
}