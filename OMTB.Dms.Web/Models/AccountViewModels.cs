using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OMTB.Dms.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "验证码")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "电话号码")]
        public string PhoneNumber { get; set; }
    }
}
