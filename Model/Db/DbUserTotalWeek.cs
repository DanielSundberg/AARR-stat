using System;
using Amazon.DynamoDBv2.DataModel;

namespace AARR_stat.Model.Db
{
    [DynamoDBTable("aarrstat-user-total-week")]
    class DbUserTotalWeek : CommonAggregationFields
    {
        [DynamoDBHashKey]
        public string UserYearWeek { get; set; }
        [DynamoDBProperty("Timestamp", typeof(DateTimeUtcConverter))]
        public DateTime Timestamp { get; set; }
        public int Week { get; set; }
    }
}