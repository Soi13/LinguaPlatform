using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LinguaPlatform.Models
{
    public class Login
    {
        [Required]        
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
                
        public string RememberMe { get; set; }        

        public string ReturnUrl { get; set; }
    }
}
