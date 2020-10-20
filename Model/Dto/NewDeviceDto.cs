namespace AARR_stat.Model.Dto
{
    public class NewDeviceDto
    {
        public string Id { get; set; }
        public string User { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; } = true;
        public bool Internal { get; set; }
    }
}