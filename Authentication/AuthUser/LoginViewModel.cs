using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AuthUser
{
    public class LoginViewModel
    {
        public LoginViewModel()
        {
            Claims = new List<string>();
        }
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string EMail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public List<string> Claims { get; set; }
    }
}


