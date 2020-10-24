using System;
using Amazon.DynamoDBv2.DataModel;

namespace AARR_stat.Model.Db
{
    [DynamoDBTable("aarrstat-user")]
    class DbUser
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
        [DynamoDBProperty("RegisterDate", typeof(DateTimeUtcConverter))]
        public DateTime RegisterDate { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}