using Amazon.DynamoDBv2.DataModel;

namespace AARR_stat.Model.Db
{
    [DynamoDBTable("aarrstat-user-total-day")]
    class DynamoDbUserTotalDay
    {
        [DynamoDBHashKey]
        public string UserYearDay { get; set; }
        public int NofSessions { get; set; }
        public int TotalMS { get; set; }
        public int AvgMS { get; set; }
        // Special fields for sessions over 60s
        public int NofSessionsOver60s { get; set; }
        public int TotalOver60sMS { get; set; }
        public int AvgOver60sMS { get; set; }
    }
}