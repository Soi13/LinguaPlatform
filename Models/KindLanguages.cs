using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LinguaPlatform.Models
{
    public class KindLanguages
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Length of level of knowledge can be between 3 and 50 symbols.")]
        public string Name { get; set; }

        public bool Blocked { get; set; }
    }
}
