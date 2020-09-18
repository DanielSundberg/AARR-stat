using System.ComponentModel.DataAnnotations;

namespace AARR_stat.Model.Dto
{
    public class StartSessionDto
    {
        [Required(AllowEmptyStrings=false)]
        public string Type { get; set; }
        [Required(AllowEmptyStrings=false)]
        public string User { get; set; }
        [Required(AllowEmptyStrings=false)]
        public string Device { get; set; }
        [Required(AllowEmptyStrings=false)]
        public string Session { get; set; }
    }
}
