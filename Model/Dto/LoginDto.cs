using System.ComponentModel.DataAnnotations;

namespace AARR_stat.Model.Dto
{
    public class LoginDto
    {
        [Required(AllowEmptyStrings=false)]
        public string Username { get; set; }
        [Required(AllowEmptyStrings=false)]
        public string Password { get; set; }
    }
}
