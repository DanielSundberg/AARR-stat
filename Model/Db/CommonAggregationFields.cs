using System;
using Amazon.DynamoDBv2.DataModel;

namespace AARR_stat.Model.Db
{
    
    abstract class CommonAggregationFields
    {
        
        public int NofSessions { get; set; }
        public int TotalMS { get; set; }
        public int AvgMS { get; set; }
        public int NofLongSession { get; set; }
        public int TotalLongSessionMS { get; set; }
        public int AvgLongSessionMS { get; set; }
    }
}