using System;
using Amazon.DynamoDBv2.DataModel;

namespace AARR_stat.Model.Db
{
    [DynamoDBTable("aarrstat-grand-total-week")]
    class DbGrandTotalWeek : CommonAggregationFields
    {
        [DynamoDBHashKey]
        public string YearWeek { get; set; }
        [DynamoDBProperty("Timestamp", typeof(DateTimeUtcConverter))]
        public DateTime Timestamp { get; set; }
        public int Week { get; set; }

        public int UserCount { get; set; }
    }
}