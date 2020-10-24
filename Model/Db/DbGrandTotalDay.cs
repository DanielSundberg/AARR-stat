using System;
using Amazon.DynamoDBv2.DataModel;

namespace AARR_stat.Model.Db
{
    [DynamoDBTable("aarrstat-grand-total-day")]
    class DbGrandTotalDay : CommonAggregationFields
    {
        [DynamoDBHashKey]
        public string YearDay { get; set; }
        [DynamoDBProperty("Timestamp", typeof(DateTimeUtcConverter))]
        public DateTime Timestamp { get; set; }
        public int Day { get; set; }
        public int UserCount { get; set; }
    }
}