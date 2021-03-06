using System;
using Amazon.DynamoDBv2.DataModel;

namespace AARR_stat.Model.Db
{
    [DynamoDBTable("aarrstat-user-total-day")]
    class DbUserTotalDay : CommonAggregationFields
    {
        [DynamoDBHashKey]
        public string UserYearDay { get; set; }
        [DynamoDBProperty("Timestamp", typeof(DateTimeUtcConverter))]
        public DateTime Timestamp { get; set; }
        public int Day { get; set; }
    }
}