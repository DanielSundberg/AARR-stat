using System;
using Amazon.DynamoDBv2.DataModel;

namespace AARR_stat.Model.Db
{
    [DynamoDBTable("aarrstat-user")]
    class DbUser
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
    }
}