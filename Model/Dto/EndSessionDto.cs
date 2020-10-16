using System.ComponentModel.DataAnnotations;

namespace AARR_stat.Model.Dto
{
    public class EndSessionDto
    {
        [Required(AllowEmptyStrings=false)]
        public string Session { get; set; }
    }
}
